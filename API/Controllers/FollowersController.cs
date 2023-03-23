namespace API.Controllers;

[Route("api/v{version:apiVersion}/followers")]
[ApiController]
[ApiVersion("1.0")]
public sealed class FollowersController : ControllerBase
{
    private readonly IMediator _mediator;

    public FollowersController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// follow some user endpoint to follow another user.
    /// </summary>
    /// <param name="destinationUserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns success or not if you followed the user</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /daa04b47-6c3d-4823-8ded-17e0f524d355
    /// </remarks>
    /// <response code="200">Returns success or not if you followed the user.</response>
    /// <response code="401">User you want to follow does.</response>
    [HttpPost("{destinationUserId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> FollowerUser(
        [FromRoute] Guid destinationUserId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new FollowerUserProcess.Request
        {
            DestinationUserId = destinationUserId,
        }, cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return Ok("Followed The User with the gives id successfully.");
    }


    /// <summary>
    /// end point for getting the current user followers.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns current user followers</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /followers/current-user-followers
    /// </remarks>
    /// <response code="200">Returns current user followers.</response>
    /// <response code="401">User does not exist.</response>
    [HttpGet("current-user-followers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUserFollowers(
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetCurrentUserFollowersProcess.Request { },
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// end point for getting the followed by users for the current user.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns current user followers</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /followers/current-user-followers
    /// </remarks>
    /// <response code="200">Returns current user followers.</response>
    /// <response code="401">User does not exist.</response>
    [HttpGet("current-user-followed-by")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFollowedByUsersForCurrentUser(
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetCurrentUserFollowedByUsersProcess.Request { },
            cancellationToken);

        return Ok(response);
    }

}
