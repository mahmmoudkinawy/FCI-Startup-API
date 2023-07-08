namespace API.Controllers;

[Route("api/v{version:apiVersion}/account")]
[ApiVersion("1.0")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// reset password endpoint to take the userId and token for the user.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /reset-password?userId=86210802-7771-4c26-b19d-08db7fc11192&token=CfDJ8LqJC4%2FFwC5A4wAIheEe
    ///     {
    ///        "password":"Pa$$w0rd"
    ///     }
    /// </remarks>
    /// <response code="204">Returns no content if succeed.</response>
    /// <response code="401">User does not exist.</response>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordProcess.Request request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            request,
            cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

    /// <summary>
    /// forgot password endpoint to send an email to the users email.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /forgot-password
    ///     {
    ///        "email":"mahmmoudkinawy@gmail.com"
    ///     }
    /// </remarks>
    /// <response code="204">Returns no content if succeed.</response>
    /// <response code="401">User does not exist.</response>
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordProcess.Request request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            request,
            cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }


    /// <summary>
    /// change password endpoint to change user password.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /change-password
    ///     {
    ///        "currentPassword":"Pa$$w0rd",
    ///        "newPassword": "123147"
    ///     }
    /// </remarks>
    /// <response code="204">Returns no content if succeed.</response>
    /// <response code="400">User errors like password does not match ... etc.</response>
    /// <response code="401">User does not exist.</response>
    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordProcess.Request request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            request,
            cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
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
    [AllowAnonymous]
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
    [AllowAnonymous]
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
