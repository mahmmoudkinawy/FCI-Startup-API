namespace API.Entities;

public sealed class ImageEntity
{
    public Guid Id { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsMain { get; set; } = false;

    public Guid UserId { get; set; }
    public UserEntity User { get; set; }
}
