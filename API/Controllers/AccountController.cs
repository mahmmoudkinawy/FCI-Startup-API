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

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] UserLoginProcess.Request request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return Ok(response.Value);
    }

    [HttpPost("register")]
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

    [HttpOptions]
    public void GetOptions() => Response.Headers.Add("Allowed", "POST");
}
