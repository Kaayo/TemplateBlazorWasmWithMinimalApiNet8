//using System.Net;

//namespace TemplateBlazorWasmHostedNet8.Api.Filters;

//public class CustomAuthorizationAttribute : IEndpointFilter
//{
//    //protected readonly ILogger Logger;
//    //private readonly string _methodName;

//    //protected CustomAuthorizationAttribute()
//    //{
//    //    ////Logger = loggerFactory.CreateLogger<CustomAuthorizationAttribute>();
//    //    ////_methodName = GetType().Name;
//    //}

//    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
//    {
//        try
//        {

//            var route = context.HttpContext.GetEndpoint()!.DisplayName;
//            if (route!.Contains("account/login"))
//            {
//                return await next(context);

//            }


//            var tokenJwt = context.HttpContext.Request.Headers?.Authorization.ToString()?.Split("Bearer")?[1];
//            if (tokenJwt is null)
//            {
//                return Results.Unauthorized();
//            }

//            //////var x = context.Arguments.OfType<Token_RefreshTokenDto>();
//            //////if (context.Arguments.OfType<Token_RefreshTokenDto>().Count() > 0)
//            //////{

//            //////}

//            var tokenService = context.HttpContext.RequestServices.GetRequiredService<TokenService>();
//            var (IsTokenValid, IsToUpdateToken, ErrorMessage) = tokenService.ValidateTokenJWT(tokenJwt);
//            if (!string.IsNullOrEmpty(ErrorMessage))
//            {
//                return Results.ValidationProblem(new Dictionary<string, string[]> { { "Error", new string[] { ErrorMessage } } },
//                                   statusCode: (int)HttpStatusCode.InternalServerError);
//            }
//            else if (IsToUpdateToken.HasValue && IsToUpdateToken.Value)
//            {
//                var newToken = tokenService.GenerateTokenJwt(1, "a", "a", true);
//                var newRefreshToken = tokenService.GenerateRefreshTokenJWT();

//                var result1 = await next(context);
//                return result1;
//            }


//            // Before
//            var result = await next(context);
//            // After

//            return await next(context);
//        }
//        catch (Exception ex)
//        {

//            throw;
//        }
//    }
//}
