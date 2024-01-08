using System.Runtime.Serialization;

namespace TemplateBlazorWasmHostedNet8.Shared.CustomExceptions;

[Serializable]
public class CustomNotAuthorizeJwtToken : Exception
{
    public CustomNotAuthorizeJwtToken(string? message = null) : base(string.IsNullOrEmpty(message) ? "Token Jwt inválido" : message)
    {
    }
}
