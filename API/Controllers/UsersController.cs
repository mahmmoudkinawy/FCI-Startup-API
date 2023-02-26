namespace API.Controllers;

[Route("api/v{version:apiVersion}/users")]
[ApiVersion("1.0")]
[ApiController]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// upload image endpoint to make the user upload an image as profile pic.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the url for the created image</returns>
    /// <response code="200">Returns image that got uploaded.</response>
    /// <response code="400">If the uploaded image contains error.</response>
    /// <response code="401">User does not exist.</response>
    [HttpPost("add-image")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadImageForProfile(
        [FromForm] UploadImageProcess.Request request,
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
    /// set main image endpoint to make the user make anothor image as the main pic.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the operation status.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Did not success for some reason.</response>
    /// <response code="401">User does not exist.</response>
    [HttpPut("set-main-photo/{imageId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SetMainImage(
        [FromRoute] Guid imageId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new MainImageProcess.Request
            {
                ImageId = imageId
            }, cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }


    /// <summary>
    /// An endpoint the shows you what you can do with this controller.
    /// </summary>
    [HttpOptions]
    public void GetOptions() => Response.Headers.Add("Allow", "GET,POST,PUT");
}
