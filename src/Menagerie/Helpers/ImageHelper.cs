using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Serilog;

namespace Menagerie.Helpers;

public static class ImageHelper
{
    #region Members

    private static readonly HttpClient _httpClient = new();

    #endregion
    
    #region Public methods

    public static async Task<Bitmap?> LoadFromWeb(string url, int width = 0)
    {
        try
        {
            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync();
            return width == 0 ? new Bitmap(new MemoryStream(data)) : Bitmap.DecodeToWidth(new MemoryStream(data), width);
        }
        catch (HttpRequestException e)
        {
            Log.Warning("An error occurred while downloading image '{Url}' : {Message}", url, e.Message);
            return null;
        }
    }

    #endregion
}