using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace TemplateBlazorWasmHostedNet8.Shared.Extensions;

public static class HttpClientExtensions
{
    public static StringContent GetStringContentFromJson<T>(this HttpClient httpClient, T json)
    {
        var jsonContent = JsonSerializer.Serialize<T>(json);
        var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return contentString;
    }
    public static async Task<T?> GetJson<T>(this HttpResponseMessage httpResponseMessage)
    {
        return await httpResponseMessage.Content.ReadFromJsonAsync<T>();
    }
}
