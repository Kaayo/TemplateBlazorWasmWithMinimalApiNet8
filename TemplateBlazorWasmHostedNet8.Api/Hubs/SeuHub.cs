using TemplateBlazorWasmHostedNet8.Api.Hubs.ManagerHubs;
using TemplateBlazorWasmHostedNet8.Api.Hubs.Models;

namespace TemplateBlazorWasmHostedNet8.Api.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SeuHub : Hub
{
    //private static ManagerHubSeuHub _managerHubSeuHub;

    //public SeuHub()
    //{
    //    _managerHubSeuHub ??= new();
    //}

    //public override Task OnConnectedAsync()
    //{
    //    var user = Context.User;
    //    if (user is not null && user.Identity.IsAuthenticated)
    //    {
    //        var obj = new ClientSeuHub();
    //        _managerHubSeuHub.Add()
    //    }

    //    return base.OnConnectedAsync();
    //}

    public async IAsyncEnumerable<DateTime> Streaming(CancellationToken cancellationToken)
    {
        while (true)
        {
            yield return DateTime.Now;
            await Task.Delay(1000, cancellationToken);
        }
    }
}
