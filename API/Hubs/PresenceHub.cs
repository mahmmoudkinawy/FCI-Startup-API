namespace API.Hubs;

[Authorize]
public sealed class PresenceHub : Hub
{
    [HubMethodName("UserIsOnline")]
    public override async Task OnConnectedAsync()
    {
        await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserFullName());
    }

    [HubMethodName("UserIsOffline")]
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserFullName());

        await base.OnDisconnectedAsync(exception);
    }

}
