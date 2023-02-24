namespace API.Entities;

public sealed class PostEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = null!;
    public string? Content { get; set; }

    public Guid UserId { get; set; }
    public UserEntity User { get; set; }
}
