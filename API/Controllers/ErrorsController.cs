namespace API.Controllers;

[Route("api/v{version:apiVersion}/errors")]
[ApiVersion("1.0")]
[ApiController]
public sealed class ErrorsController : ControllerBase
{
    private readonly AlumniDbContext _context;

    public ErrorsController(AlumniDbContext context)
    {
        _context = context ??
            throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Try's get not found entity
    /// </summary>
    /// <returns>will always return 404</returns>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("not-found")]
    [AllowAnonymous]
    public IActionResult GetNotFound()
    {
        var notFoundUser = _context.Users.Find(Guid.NewGuid());

        if (notFoundUser == null) return NotFound();

        return Ok(notFoundUser);
    }

    /// <summary>
    /// Try's to access authorized information
    /// </summary>
    /// <returns>will always return 401</returns>
    [HttpGet("auth")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetSecret()
    {
        return Unauthorized("Unauthorized - Secret information");
    }

    /// <summary>
    /// Get a Bad Request
    /// </summary>
    /// <returns>will always return 400</returns>
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("bad-request")]
    [AllowAnonymous]
    public IActionResult GetBadRequest()
    {
        return BadRequest("That's a very very BAD REQUEST");
    }

    /// <summary>
    /// Will get server error
    /// </summary>
    /// <returns>will always return 500</returns>
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("server-error")]
    [AllowAnonymous]
    public IActionResult GetServerError()
    {
        var notFoundUser = _context.Users.Find(Guid.NewGuid());

        return Ok(notFoundUser!.ToString());
    }

}
