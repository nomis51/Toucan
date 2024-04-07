using Menagerie.Core.Models.Setting;

namespace Menagerie.Core.Services.Abstractions;

public interface ISettingsService
{
    Task<Settings> GetSettings();
    Task SaveSettings(Settings settings);
}