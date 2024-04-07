using System.Drawing;
using Serilog;

namespace Menagerie.Core.Helpers;

public static class ImageHelper
{
    #region Public methods

    public static async Task<Bitmap?> LoadFromWeb(string url)
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync();
            return new Bitmap(new MemoryStream(data));
        }
        catch (HttpRequestException e)
        {
            Log.Warning("An error occurred while downloading image '{Url}' : {Message}", url, e.Message);
            return null;
        }
    }

    #endregion
}