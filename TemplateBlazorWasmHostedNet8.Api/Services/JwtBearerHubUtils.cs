namespace TemplateBlazorWasmHostedNet8.Api.Services;

public class JwtBearerHubUtils
{
    public async Task ValidateJwtTokenOnHubByPath(MessageReceivedContext context, string path, Func<Task>? action = null)
    {
        var hasChat = context.Request.Path.Value?.Contains(path);
        if (hasChat is not null && hasChat.Value)
        {
            var accessToken =
             context.Request.Headers?.Authorization.Select(x => x?.Replace("Bearer", string.Empty)?.Trim())?.FirstOrDefault()
             ?? context.Request.Query?.FirstOrDefault(x => x.Key.Contains("access_token")).Value;

            if (string.IsNullOrEmpty(accessToken))
            {
                context.Fail(new CustomNotAuthorizeJwtToken("Token vazio"));
            }
            else
            {
                var tokenService = context.HttpContext.RequestServices.GetRequiredService<TokenService>();
                var (IsTokenValid, IsToUpdateToken, ErrorMessage) = tokenService.ValidateTokenJWT(accessToken);
                if (!IsTokenValid!.Value)
                {
                    context.Fail(new CustomNotAuthorizeJwtToken("Falha ao autenticar Token JWT: Token vazio"));

                    // Executa algum ação de forma assincroniza
                    if (action is not null)
                    {
                        await action();
                    }
                }
            }
            context.Token = accessToken;
        }
    }
}
