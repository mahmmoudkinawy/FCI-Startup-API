namespace API.Helpers;

public sealed class MessageParams : PaginationParams
{
    public string Container { get; set; } = "Unread";
}
