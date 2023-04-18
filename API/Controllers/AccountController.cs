namespace API.Controllers;

[Route("api/v{version:apiVersion}/account")]
[ApiVersion("1.0")]
[ApiController]
[AllowAnonymous]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// login endpoint to authenticate the user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the full name with the token as a plain text</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /login
    ///     {
    ///        "email":"bob@test.com",
    ///        "password":"Pa$$w0rd"
    ///     }
    /// </remarks>
    /// <response code="200">Returns the user if exists with full name and token.</response>
    /// <response code="401">User does not exist.</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] UserLoginProcess.Request request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        if (!response.IsSuccess)
        {
            return Unauthorized(response.Errors);
        }

        return Ok(response.Value);
    }


    /// <summary>
    /// register endpoint to authenticate the user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the full name with the token as a plain text</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /register
    ///     {
    ///        "firstName": "bob",
    ///        "lastName":"bob",
    ///        "gender":"Male",
    ///        "dateOfBirth":"1996-02-22T19:07:28.906Z",
    ///        "graduationYear":"2016-02-22T19:07:28.906Z",
    ///        "email":"bob@test.com",
    ///        "password":"Pa$$w0rd"
    ///     }
    /// </remarks>
    /// <response code="200">Returns the newly created user full name and token.</response>
    /// <response code="400">If the item is got validation errors.</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] UserRegisterProcess.Request request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return Ok(response.Value);
    }

}
