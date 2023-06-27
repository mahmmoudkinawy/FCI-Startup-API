namespace API.Controllers;

[Route("api/v{version:apiVersion}/posts")]
[ApiVersion("1.0")]
[ApiController]
public sealed class PostsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostsController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// endpoint for returning all the posts
    /// </summary>
    /// <param name="postsParams">Params for pagination searching etc.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Endpoint for returning all the posts</returns>
    /// <response code="200">Returns all posts in the db.</response>
    /// <response code="401">User does not exist.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllPosts(
        [FromQuery] PostsParams postsParams,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetAllPostsProcess.Request
            {
                PageSize = postsParams.PageSize,
                PageNumber = postsParams.PageNumber,
                Keyword = postsParams.Keyword
            },
            cancellationToken);

        Response.AddPaginationHeader(response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }

    /// <summary>
    /// endpoint for get a post by id
    /// </summary>
    /// <param name="request">postId</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the post with the matches id from the db</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /posts/b87a6d86-958d-4647-cc27-08db165fc0f0
    /// </remarks>
    /// <response code="200">Returns the post that matches the id.</response>
    /// <response code="404">Post with the given Id does not exist.</response>
    /// <response code="401">User does not exist.</response>
    [HttpGet("{postId}", Name = "GetPost")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPost(
        [FromRoute] Guid postId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetPostByIdProcess.Request
            {
                PostId = postId
            }, cancellationToken);

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        return Ok(response.Value);
    }

    /// <summary>
    /// endpoint for creating a comment by post id - only for testing
    /// </summary>
    [HttpPost("comments/{postId}")]
    public async Task<IActionResult> CreateComment(
       [FromBody] CreateCommentForPostProcess.Request request,
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
    /// endpoint for loading all comments for post by post id - only for testing
    /// </summary>
    [HttpGet("comments/{postId}")]
    public async Task<IActionResult> GetCommentsForPost(
       Guid postId,
       CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetCommentsByPostIdProcess.Request
            {
                PostId = postId
            }, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// endpoint for creating a post.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns no content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /posts/
    ///     
    ///         Content-Type: multipart/form-data;
    ///         Content-Disposition: form-data; name="content"
    ///
    ///         This is the content of the post.
    ///         Content-Disposition: form-data; name="image"; filename="example.jpg"
    ///         Content-Type: image/jpeg
    ///     
    /// </remarks>
    /// <response code="201">Returns no content.</response>
    /// <response code="400">There exist validation errors.</response>
    /// <response code="401">User does not exist.</response>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreatePost(
        [FromForm] CreatePostProcess.Request request,
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
    /// endpoint for updating a post.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the updated post</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /posts/
    ///     
    ///         Content-Type: multipart/form-data;
    ///         Content-Disposition: form-data; name="content"
    ///
    ///         This is the content of the post updated.
    ///         Content-Disposition: form-data; name="image"; filename="example2.jpg"
    ///         Content-Type: image/jpeg
    ///     
    /// </remarks>
    /// <response code="200">Returns updated post.</response>
    /// <response code="400">There exist validation errors.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="404">Post does not exist.</response>
    [HttpPut("{postId:guid}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePost(
        [FromForm] UpdatePostProcess.Request request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            request,
            cancellationToken);

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// endpoint for deleting a post.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns no content</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /posts/b87a6d86-958d-4647-cc27-08db165fc0f0
    /// </remarks>
    /// <response code="201">Returns redirection to the post that got created.</response>
    /// <response code="404">There does not exist.</response>
    /// <response code="401">User does not exist.</response>
    [HttpDelete("{postId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeletePost(
        [FromRoute] Guid postId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new DeletePostProcess.Request
            {
                PostId = postId
            }, cancellationToken);

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// endpoint for like a post by post id.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns no content</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /posts/db9504d4-78bc-4ca4-9fb4-994babc74354/like
    /// </remarks>
    /// <response code="201">Returns no content.</response>
    /// <response code="400">There exist validation errors.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="404">Post with the given Id does not exist.</response>
    [HttpPost("{postId:guid}/like")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateLikeForPost(
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new CreateLikeForPostProcess.Request { },
            cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return Ok(response.Value);
    }

}
