using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using LiteDB.Engine;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Enums;
using Menagerie.Core.Models.ML;

namespace Menagerie.Core.Services
{
    public class AppAiService : IService
    {
        #region Constant

        private const string PYTHON_392_ZIP_URL = "https://www.python.org/ftp/python/3.9.2/python-3.9.2-embed-amd64.zip";
        private const string PYTHON_FOLDER = "./ML/python/";
        private const string LOCAL_PYTHON_EXE_PATH = "./ML/python/python.exe";
        private const string ML_SERVER_PATH = "./ML/server.py";
        private const string TRAINED_MODELS_FOLDER = "./ML/trained/";
        private const string TRAINED_MODELS_LATEST_RELEASE_URL = "https://github.com/nomis51/poe-ml/releases/download/v0.1.0/trained_models.zip";
        public const string TEMP_FOLDER = "./ML/.temp/";
        private readonly Uri _pythonServerUrl = new("http://localhost:8302");

        private readonly List<string> _trainedModelsName = new()
        {
            "currency_type",
            "stack_size",
            "item_links",
            "item_sockets",
            "socket_color"
        };

        #endregion

        #region Members

        private readonly bool _useLocalPython = false;
        private readonly ProcessStartInfo _pythonServerProcessInfos;
        private Process _pythonServerProcess;
        private readonly HttpService _httpService;

        #endregion

        #region Constructors

        public AppAiService()
        {
            _httpService = new HttpService(_pythonServerUrl);

            if (!IsPythonInstalled())
            {
                DownloadPython();
                _useLocalPython = true;
            }

            _pythonServerProcessInfos = new ProcessStartInfo()
            {
                FileName = _useLocalPython ? LOCAL_PYTHON_EXE_PATH : "python.exe",
                Arguments = ML_SERVER_PATH,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            EnsureTrainedModelsAvailable();
        }

        #endregion

        #region Private methods

        private void StartPythonServer()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    _pythonServerProcess = Process.Start(_pythonServerProcessInfos);

                    if (_pythonServerProcess != null)
                    {
                        _pythonServerProcess.WaitForExit();
                        _pythonServerProcess.Close();
                    }

                    Thread.Sleep(1000);
                }
            });
            // ReSharper disable once FunctionNeverReturns
        }

        private void EnsureTrainedModelsAvailable()
        {
            if (_trainedModelsName.Any(trainedModelName => !File.Exists($"{TRAINED_MODELS_FOLDER}/{trainedModelName}/{trainedModelName}.h5") ||
                                                           !File.Exists($"{TRAINED_MODELS_FOLDER}/{trainedModelName}/{trainedModelName}-classes.txt")))
            {
                if (Directory.Exists(TRAINED_MODELS_FOLDER))
                {
                    Directory.Delete(TRAINED_MODELS_FOLDER, true);
                }
            
                DownloadTrainedModels();
            }
        }

        private static void DownloadTrainedModels()
        {
            EnsureTrainedFolderExists();

            using var client = new WebClient();
            var zipFilePath = $"{TRAINED_MODELS_FOLDER}/trainedModels.zip";
            client.DownloadFile(TRAINED_MODELS_LATEST_RELEASE_URL, zipFilePath);

            ZipFile.ExtractToDirectory(zipFilePath, $"{TRAINED_MODELS_FOLDER}/");
        }

        private static void EnsureTrainedFolderExists()
        {
            if (!Directory.Exists(TRAINED_MODELS_FOLDER))
            {
                Directory.CreateDirectory(TRAINED_MODELS_FOLDER);
            }
        }

        private static void EnsurePythonFolderExists()
        {
            if (!Directory.Exists(PYTHON_FOLDER))
            {
                Directory.CreateDirectory(PYTHON_FOLDER);
            }
        }

        private static void DownloadPython()
        {
            EnsurePythonFolderExists();

            using var client = new WebClient();
            var zipFilePath = $"{PYTHON_FOLDER}/python392.zip";
            client.DownloadFile(PYTHON_392_ZIP_URL, zipFilePath);

            ZipFile.ExtractToDirectory(zipFilePath, $"{PYTHON_FOLDER}/");
        }

        private static bool IsPythonInstalled()
        {
            var start = new ProcessStartInfo
            {
                FileName = "python.exe",
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(start);

            if (process == null) throw new ApplicationException("Cannot verify python installation.");

            using var reader = process.StandardError;
            var result = reader.ReadToEnd();

            return string.IsNullOrEmpty(result);
        }

        private List<Bitmap> SliceImage(Image image, int rows, int cols, int width, int height)
        {
            List<Bitmap> images = new(rows * cols);

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    var bmp = new Bitmap(width, height);
                    var graphics = Graphics.FromImage(bmp);
                    graphics.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(i * width, j * height, width, height), GraphicsUnit.Pixel);
                    graphics.Dispose();
                    images.Add(bmp);
                }
            }

            return images;
        }

        #endregion

        #region Public methods

        public List<string> ProcessAndSaveTradeWindowImage(Bitmap image)
        {
            var images = SliceImage(image, AppService.Instance.TradeWindowRowsAndColumns.Width, AppService.Instance.TradeWindowRowsAndColumns.Height,
                AppService.Instance.TradeWindowSquareSize.Width, AppService.Instance.TradeWindowSquareSize.Height);
            List<string> imageIds = new();

            images.ForEach(i =>
            {
                var id = Guid.NewGuid().ToString();
                var path = $"{TEMP_FOLDER}{id}.jpeg";
                i.Save(path, ImageFormat.Jpeg);
                imageIds.Add(id);
            });

            return imageIds;
        }

        public async Task<PredictionResponse> Predict(TrainedModelType trainedModelType, IEnumerable<string> imageIds)
        {
            var trainingName = TrainedModelTypeConverter.Convert(trainedModelType);

            if (string.IsNullOrEmpty(trainingName)) return default;

            Dictionary<string, Prediction> results = new();

            var fileIds = imageIds.Where(imageId => !string.IsNullOrEmpty(imageId) && File.Exists($"{TEMP_FOLDER}{imageId}.jpeg"))
                .Aggregate(string.Empty, (current, imageId) => current + "," + imageId).Substring(1);

            try
            {
                var response = await _httpService.Client.GetAsync($"/api/{trainingName}?file_ids={fileIds}");

                if (!response.IsSuccessStatusCode) return default;

                return await HttpService.ReadResponse<PredictionResponse>(response);
            }
            catch (Exception e)
            {
                var g = 0;
            }

            return new PredictionResponse();
        }

        public async Task<Prediction> Predict(TrainedModelType trainedModelType, string imageFileId)
        {
            if (string.IsNullOrEmpty(imageFileId) || !File.Exists($"{TEMP_FOLDER}{imageFileId}.jpeg")) return default;

            var trainingName = TrainedModelTypeConverter.Convert(trainedModelType);

            if (string.IsNullOrEmpty(trainingName)) return default;

            var response = await _httpService.Client.GetAsync($"{trainingName}?file_id={imageFileId}");

            if (!response.IsSuccessStatusCode) return default;

            return await HttpService.ReadResponse<Prediction>(response);
        }

        public void Start()
        {
          //  StartPythonServer();
        }

        #endregion
    }
}