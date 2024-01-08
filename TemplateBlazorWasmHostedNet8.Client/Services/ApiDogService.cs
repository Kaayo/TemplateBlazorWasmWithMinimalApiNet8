using System.Net;
using System.Net.Http.Json;
using TemplateBlazorWasmHostedNet8.Shared.CustomExceptions;
using TemplateBlazorWasmHostedNet8.Shared.Extensions;

namespace TemplateBlazorWasmHostedNet8.Client.Services;

public class ApiDogService(
    IHttpClientFactory httpClientFactory,
    ILocalStorageService localStorageService
)
{
    private HttpClient Factory(IHttpClientFactory httpClientFactory)
    {
        var httpClient = httpClientFactory.CreateClient(EnumHttpClient.Api.ToString());
        return httpClient;
    }

    private async Task<(T Data, ResultsProblemDto Error)> GetAsync<T>(string path)
    {
        using var httpClient = Factory(httpClientFactory);
        var response = await httpClient.GetAsync(path);
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized) throw new CustomNotAuthorizeJwtToken("Sem autorização!");

            var problems = await response.GetJson<ResultsProblemDto>();
            return (default(T), problems);
        }

        var result = await response.Content.ReadFromJsonAsync<T>();
        return (result, null);
    }

    private async Task<(TResult Data, ResultsProblemDto Error)> PostAsync<TPayload, TResult>(string path, TPayload payload, CancellationToken? cancellationToken)
    {
        using var httpClient = Factory(httpClientFactory);

        var response = await httpClient.PostAsJsonAsync(path, payload, cancellationToken.Value);
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized) throw new CustomNotAuthorizeJwtToken("Sem autorização!");

            var problems = await response.GetJson<ResultsProblemDto>();
            return (default(TResult), problems);
        }

        var result = await response.GetJson<TResult>();
        return (result, null);
    }

    //private async Task<(TResult Data, ResultsProblemDto Error)> PostBinaryAsync<TPayload, TResult>(string path, TPayload payload)
    //{
    //    using var httpClient = Factory(httpClientFactory);

    //    var response = await httpClient.Post(path, payload);
    //    if (!response.IsSuccessStatusCode)
    //    {
    //        if (response.StatusCode == HttpStatusCode.Unauthorized) throw new CustomNotAuthorizeJwtToken("Sem autorização!");

    //        var problems = await response.GetJson<ResultsProblemDto>();
    //        return (default(TResult), problems);
    //    }

    //    var result = await response.Content.ReadFromJsonAsync<TResult>();
    //    return (result, null);
    //}

    public async Task<(string Data, ResultsProblemDto Error)> GetOkFromDog()
    {
        var result = await GetAsync<string>("/dog/get");
        return result;
    }
    
    public async Task<(LoginDto Data, ResultsProblemDto Error)> PostBinaryFromDog(CancellationToken? cancellationToken)
    {
        var payload = new DogDto("Binary", "Binary");
        var result = await PostAsync<DogDto, LoginDto>("/dog/getbinary", payload, cancellationToken);
        //var result = await PostBinaryAsync<DogDto, ResultDto<LoginDto>>("/dog/getbinary", payload);
        return result;
    }
}
