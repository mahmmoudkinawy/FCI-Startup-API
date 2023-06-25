namespace API.Hubs;

/// <summary>
/// Authorized Comment Hub that's accessed via 'hubs/comment'.
/// </summary>
[SignalRHub]
[Authorize]
public sealed class CommentHub : Hub
{
    private readonly IMediator _mediator;

    public CommentHub(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// LoadComments method method that's used for getting comments for post.
    /// <returns>Returns the comments for the given post.</returns>
    /// </summary>
    [SignalRMethod("LoadComments")]
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var postId = httpContext.Request.Query["postId"];
        await Groups.AddToGroupAsync(Context.ConnectionId, postId);
        var result = await _mediator.Send(
            new GetCommentsByPostIdProcess.Request
            {
                PostId = Guid.Parse(postId)
            });

        await Clients.Caller.SendAsync("LoadComments", result);
    }

    /// <summary>
    /// CreateComment method that's used for create a comment for post.
    /// Triggered by calling CreateComment.
    /// <returns>Returns the created comment.</returns>
    /// </summary>
    [SignalRMethod("ReceiveComment")]
    public async Task CreateComment(
        CreateCommentForPostProcess.Request request)
    {
        var comment = await _mediator.Send(request);

        await Clients.Group(request.PostId.ToString())
            .SendAsync("ReceiveComment", comment.Value);
    }

}
