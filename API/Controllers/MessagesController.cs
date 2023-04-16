using API.Processes.Messages;

namespace API.Controllers;

[Route("api/v{version:apiVersion}/messages")]
[ApiVersion("1.0")]
[ApiController]
public sealed class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessagesController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// endpoint for creating a message.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the create message.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /messages/6fafacd7-80fa-48c2-9dff-f12c01aa25ed
    ///     {
    ///       "content": "hello there"
    ///     }
    /// </remarks>
    /// <response code="200">Returns the created message.</response>
    /// <response code="400">There exist validation errors.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="404">User with the given ID does not exist.</response>
    [HttpPost("{recipientId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateMessage(
        [FromBody] CreateMessageProcess.Request request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            request,
            cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return Ok(response.Value);
    }
}
