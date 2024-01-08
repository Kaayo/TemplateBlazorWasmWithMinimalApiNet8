using TemplateBlazorWasmHostedNet8.Api.Hubs.Models;

namespace TemplateBlazorWasmHostedNet8.Api.Hubs.ManagerHubs;

public class ManagerHubSeuHub
{
    private static readonly List<ClientSeuHub> _connections = [];

    public void Add(ClientSeuHub connection)
    {
        lock (_connections)
        {
            _connections.Add(connection);
        }
    }

    public List<ClientSeuHub> GetConnections()
    {
        lock (_connections)
        {
            return [.. _connections];
        }
    }

    public ClientSeuHub? GetConnectionByObjectClient(ClientSeuHub connection)
    {
        lock (_connections)
        {
            return _connections.SingleOrDefault(conn => conn == connection);
        }
    }

    public ClientSeuHub? GetConnectionByJwtTokenClient(string jwtTokenClient)
    {
        lock (_connections)
        {
            return _connections.SingleOrDefault(conn => conn.JwtToken == jwtTokenClient);
        }
    }

    public void RemoveByObjectClient(ClientSeuHub connection)
    {
        lock (_connections)
        {
            _connections.Remove(connection);
        }
    }

    public void RemoveByJwtTokenClient(string jwtToken)
    {
        lock (_connections)
        {
            var client = _connections.SingleOrDefault(c => c.JwtToken == jwtToken);
            if (client != null)
            {
                _connections.Remove(client);
            }
        }
    }
}
