using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using TemplateBlazorWasmHostedNet8.Shared.CustomExceptions;

namespace TemplateBlazorWasmHostedNet8.Client.Services;

public class NotificationsGenericHub
{
    private readonly TimeSpan[] _timeReconnects;

    public NotificationsGenericHub()
    {
        _timeReconnects = [
            TimeSpan.FromSeconds(0),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10)
        ];
    }

    private async Task<HubConnection> Core(HubConnection hubConnection, string hubUrl, string? jwtTokenString)
    {
        try
        {
            // Create the chat client
            hubUrl = hubUrl.Trim();

            hubConnection ??= new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(jwtTokenString);
                options.DefaultTransferFormat = TransferFormat.Binary;
                //options.Transports = HttpTransportType.WebSockets;
                options.ApplicationMaxBufferSize = int.MaxValue;
                options.TransportMaxBufferSize = int.MaxValue;
                options.SkipNegotiation = false;
                options.UseStatefulReconnect = false;
            }
             )
            .WithAutomaticReconnect(_timeReconnects)
            .AddMessagePackProtocol()
            .Build();

            if (
                hubConnection?.State == HubConnectionState.Disconnected
            )
            {
                await hubConnection.StartAsync();
            }

            return hubConnection;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Response status code does not indicate success: 401 (Unauthorized)"))
            {
                throw new CustomNotAuthorizeJwtToken();
            }

            throw ex;
        }
    }

    public async Task<HubConnection> NotifyHub_Init(HubConnection hubConnection, string hubUrl, string jwtTokenString)
    {
        return await Core(hubConnection, hubUrl, jwtTokenString);
    }

    public async Task<HubConnection> NotifyHub_OnReceive(HubConnection hubConnection, string hubUrl, string methodClientsInvoke, string tokenString, Func<Task> action)
    {
        hubConnection = await Core(hubConnection, hubUrl, tokenString);

        hubConnection.On(methodClientsInvoke, async () =>
        {
            await action();
        });

        return hubConnection;
    }

    public async Task NotifyHub_Send(HubConnection hubConnection, string methodServerInvoke)
    {
        await hubConnection.SendAsync(methodServerInvoke);
    }

    public async Task NotifyHub_Dispose(HubConnection hubConnection)
    {
        if (
            hubConnection is not null && 
            hubConnection.State == HubConnectionState.Connected
        )
        {
            await hubConnection.StopAsync();
            await hubConnection.DisposeAsync();
        }
        else if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}