using System.Security.Claims;

namespace TemplateBlazorWasmHostedNet8.Api.Hubs.Models;

public record ClientSeuHub(string JwtToken, ClaimsPrincipal User);
