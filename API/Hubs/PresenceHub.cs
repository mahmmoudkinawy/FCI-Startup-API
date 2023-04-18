namespace API.Hubs;

/// <summary>
/// Authorized Presence Hub that's accessed via 'hubs/presence'.
/// </summary>
[SignalRHub]
[Authorize]
public sealed class PresenceHub : Hub
{
    /// <summary>
    /// UserIsOnline Hub method method that's used for tracking the online users.
    /// <returns>Returns the full name with any user that's just got online.</returns>
    /// </summary>
    [SignalRMethod("UserIsOnline")]
    public override async Task OnConnectedAsync()
    {
        await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserFullName());
    }

    /// <summary>
    /// UserIsOffline Hub method method that's used for tracking the offline users.
    /// <returns>Returns the full name with any user that's just got offline.</returns>
    /// </summary>
    [SignalRMethod("UserIsOffline")]
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserFullName());

        await base.OnDisconnectedAsync(exception);
    }

}
