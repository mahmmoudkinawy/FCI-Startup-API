namespace API.Hubs;

[Authorize]
public sealed class PresenceHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserFullName());
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserFullName());

        await base.OnDisconnectedAsync(exception);
    }

}
