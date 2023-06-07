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
    /// 'Deprecated' - endpoint for creating a message.
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

    /// <summary>
    /// endpoint for getting messages for the logged in user.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the create message.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /messages?container=Inbox
    /// </remarks>
    /// <remarks>
    /// Sample containers params are ['Outbox', 'Inbox', 'Unread'] and it's Unread be default.
    /// </remarks>
    /// <response code="200">Returns the created message.</response>
    /// <response code="401">User does not exist.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMessageForUser(
        [FromQuery] MessageParams messageParams,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetMessagesForUserProcess.Request
            {
                Container = messageParams.Container,
                PageNumber = messageParams.PageNumber,
                PageSize = messageParams.PageSize
            }, cancellationToken);

        Response.AddPaginationHeader(response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }


    /// <summary>
    /// endpoint for getting thread messages for the logged in user with some other user.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the thread messages.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /thread/388ec0a8-c9b5-4277-a27f-820e5cf786dd
    /// </remarks>
    /// <response code="200">Returns the thread messages.</response>
    /// <response code="401">User does not exist.</response>
    [HttpGet("thread/{recipientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMessageThread(
        [FromRoute] Guid recipientId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
           new GetMessageThreadProcess.Request
           {
               RecipientId = recipientId
           }, cancellationToken);

        if (response is null)
        {
            return BadRequest("Recipient with the given Id does not exist");
        }

        return Ok(response);
    }


    /// <summary>
    /// endpoint for delete a message for a user.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns no content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /messages/388ec0a8-c9b5-4277-a27f-820e5cf786dd
    /// </remarks>
    /// <response code="200">Returns no content</response>
    /// <response code="400">There exist some validation errors.</response>
    /// <response code="401">User does not exist.</response>
    [HttpDelete("{messageId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteMessage(
        [FromRoute] Guid messageId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new DeleteMessageProcess.Request
            {
                MessageId = messageId
            }, cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

}
