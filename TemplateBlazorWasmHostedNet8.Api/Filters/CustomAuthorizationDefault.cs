//using Microsoft.AspNetCore.Mvc.Filters;
//using System.Net;

//namespace TemplateBlazorWasmHostedNet8.Api.Filters;

//[AttributeUsage(AttributeTargets.All)]
//public class CustomAuthorizationDefaultAttribute : Attribute, IAsyncAuthorizationFilter
//{
//    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//    {
//       await next();
//    }

//    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
//    {
//        throw new NotImplementedException();
//    }
//}
