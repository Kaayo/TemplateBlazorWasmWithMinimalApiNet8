using Microsoft.AspNetCore.SignalR.Client;
using TemplateBlazorWasmHostedNet8.Shared.Extensions;

namespace TemplateBlazorWasmHostedNet8.Client.Services;

public class AccountService(
    IHttpClientFactory _httpClientFactory,
    ILocalStorageService _localStorageService,
    TokenService _tokenService
    ) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        #region Verifica se há tokens salvos com o estado atual do usuario
        var tokenJwtSavedOnBrowser = await VerifyTokensExistsOnBrowserAndReturnTokenJWTAsync();

        // Se token JWT não existe, retorna um estado "vazio (anônimo)"
        if (tokenJwtSavedOnBrowser is null) return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var authenticationState = await CreateAndReturnAuthenticationStateAsync(tokenJwtSavedOnBrowser);
        return authenticationState;
        #endregion
    }

    public async Task LoginAsync(string username, string password)
    {
        try
        {
            using var httpClient = _httpClientFactory.CreateClient(EnumHttpClient.Api.ToString());

            var json = new Token_RefreshTokenDto(username, password);
            var contentString = httpClient.GetStringContentFromJson(json);

            using var response = await httpClient.PostAsync("/account/login", contentString);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Erro ao fazer login");
            }

            var result = await response.GetJson<Token_RefreshTokenDto>();

            #region Cria o estado para o usuário logado atualmente e notifica a UI
            await StoreUserAuthenticatedAsync(
                result.Token,
                result.RefreshToken
            );
            #endregion
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task SaveTokensOnLocalStorageBrowserAsync(string tokenJwt, string refreshToken)
    {
        try
        {
            await _localStorageService.SetItemAsStringAsync(EnumTokenService.TokenJWT.ToString(), tokenJwt);
            await _localStorageService.SetItemAsStringAsync(EnumTokenService.RefreshToken.ToString(), refreshToken);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task ClearTokensOnLocalStorageBrowserAsync()
    {
        try
        {
            IEnumerable<string> itemsToRemoveOnLocalStorageBrowser = new string[]
            {
                EnumTokenService.TokenJWT.ToString(),
                EnumTokenService.RefreshToken.ToString()
            };

            await _localStorageService.RemoveItemsAsync(itemsToRemoveOnLocalStorageBrowser);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task StoreUserAuthenticatedAsync(string tokenJwt, string refreshToken)
    {
        try
        {
            #region Salva o Token JWT e Refresh Token no LocalStorage do Browser
            await SaveTokensOnLocalStorageBrowserAsync(
                tokenJwt,
                refreshToken
            );
            #endregion

            #region Cria o estate do usuario e retorna o estado
            var authenticationState = await CreateAndReturnAuthenticationStateAsync(tokenJwt);
            #endregion

            #region Notificando os Componentes da UI do app para se adequarem ao novo estado
            NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
            #endregion
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task LogOutUserAuthenticatedAsync(HubConnection? hubConnection = null)
    {
        try
        {
            #region Limpa o localStorage do Browser onde os tokens estão armazenados
            await ClearTokensOnLocalStorageBrowserAsync();
            #endregion

            #region Cria um estado "Vazio e Anônimo"
            var anonimous = new ClaimsPrincipal(new ClaimsIdentity());
            var authenticationState = new AuthenticationState(anonimous);
            #endregion

            #region Verifica e encerra conexão com websocket caso exista
            if (
                hubConnection is not null &&
                hubConnection.State == HubConnectionState.Connected
            )
            {
                await hubConnection.StopAsync();
                //await hubConnection.DisposeAsync();
            }
            else if (hubConnection is not null)
            {
               //await hubConnection.DisposeAsync();
            }
            #endregion

            #region Notificando os Componentes da UI do app para se adequarem ao novo estado
            NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
            #endregion
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<string?> VerifyTokensExistsOnBrowserAndReturnTokenJWTAsync()
    {
        try
        {
            var existTokeJwt = await _localStorageService.ContainKeyAsync(EnumTokenService.TokenJWT.ToString());
            var existRefreshToken = await _localStorageService.ContainKeyAsync(EnumTokenService.RefreshToken.ToString());

            if (!existTokeJwt || !existRefreshToken) return null;

            var tokenJwt = await _localStorageService.GetItemAsStringAsync(EnumTokenService.TokenJWT.ToString());
            return tokenJwt;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<AuthenticationState> CreateAndReturnAuthenticationStateAsync(string tokenJwt)
    {
        try
        {
            #region Extrai as Claims do Token JWT
            var claims = _tokenService.GetClaimsFromTokenJWT(tokenJwt);
            #endregion

            #region Criando State para o usuário logado
            var claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, EnumTokenService.AliasAuthenticationStateTokenJwt.ToString()));
            var authenticationState = new AuthenticationState(claimPrincipal);
            #endregion 

            return await Task.FromResult(authenticationState);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}

