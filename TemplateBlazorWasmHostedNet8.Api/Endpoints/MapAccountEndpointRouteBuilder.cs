

namespace TemplateBlazorWasmHostedNet8.Api.Endpoints;

public static class MapAccountEndpointRouteBuilder
{
    public static IEndpointConventionBuilder MapAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var accountGroup = endpoints
            .MapGroup("/account")
            .RequireAuthorization();
        //.AddEndpointFilter<CustomAuthorizationAttribute>();


        accountGroup.MapPost("/login", (
            [FromServices] TokenService _token,
            [FromBody] Token_RefreshTokenDto dto) =>
        {


            var tokenJwt = _token.GenerateTokenJwt(1, "Caio", "Caio 1", true);

            return new Token_RefreshTokenDto(tokenJwt, _token.GenerateRefreshTokenJWT());
        })
        .AllowAnonymous();

        accountGroup.MapPost("/authorize", ([FromBody] LoginDto dto) =>
        {


            return new LoginDto("Autorizado", "Autorizado");
        });

        return accountGroup;
    }
}