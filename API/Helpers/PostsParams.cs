namespace API.Helpers;

public sealed class PostsParams : PaginationParams
{
    public string? Keyword { get; set; }
}
