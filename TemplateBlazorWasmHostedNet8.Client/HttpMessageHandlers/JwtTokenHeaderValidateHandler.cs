
using System.Net;
using System.Net.Http;

namespace TemplateBlazorWasmHostedNet8.Client.HttpMessageHandlers;

public class JwtTokenHeaderValidateHandler(
    ILocalStorageService localStorageService
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tokenJwt = await localStorageService.GetItemAsStringAsync(EnumTokenService.TokenJWT.ToString(), cancellationToken);
        if (!string.IsNullOrEmpty(tokenJwt))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenJwt);
        }

        //#region Verifica se o path do request é diferente do request para login
        ////var isLoginPath = request.RequestUri.LocalPath.Contains("/accout/login");
        ////if (!isLoginPath)
        ////{
        //    #region Verificar se o header de autorização não está nulo
        //    //var existAutorizationHeader = request.Headers.FirstOrDefault(x => x.Key.Contains("Authorization")).Value?.FirstOrDefault()?.Split("Bearer")?[0]?.Trim();
        //    var existAutorizationHeader = request.Headers.FirstOrDefault(x => x.Key.Contains("Authorization")).Value?.FirstOrDefault()?.Split("Bearer")?[0]?.Trim();
        //    if (!string.IsNullOrEmpty(existAutorizationHeader))
        //    {

        //    }
        //    #endregion
        ////} 
        //#endregion

        ////if (!request.Headers.Contains("X-API-KEY"))
        ////{
        ////    return new HttpResponseMessage(HttpStatusCode.BadRequest)
        ////    {
        ////        Content = new StringContent(
        ////            "The API key header X-API-KEY is required.")
        ////    };
        ////}

        return await base.SendAsync(request, cancellationToken);
    }
}
