using Menagerie.Core.Models.Setting;
using Menagerie.Core.Services.Abstractions;
using Newtonsoft.Json;
using Serilog;

namespace Menagerie.Core.Services;

public class SettingsService : ISettingsService
{
    #region Constants

    private const string SettingsFileName = "settings.json";

    #endregion

    #region Public methods

    public async Task<Settings> GetSettings()
    {
        try
        {
            var path = Path.Join(
                AppService.Instance.GetAppFolder(),
                SettingsFileName
            );
            if (!File.Exists(path)) return new Settings();

            return await File.ReadAllTextAsync(path)
                .ContinueWith(x => JsonConvert.DeserializeObject<Settings>(x.Result) ?? new Settings());
        }
        catch (Exception e)
        {
            Log.Warning("Unable to read settings file: {Message}", e.Message);
        }

        return new Settings();
    }

    public async Task SaveSettings(Settings settings)
    {
        try
        {
            await File.WriteAllTextAsync(
                Path.Join(
                    AppService.Instance.GetAppFolder(),
                    SettingsFileName
                ),
                JsonConvert.SerializeObject(settings)
            );
        }
        catch (Exception e)
        {
            Log.Warning("Unable to save settings file: {Message}", e.Message);
        }
    }

    #endregion
}