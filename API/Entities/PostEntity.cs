namespace API.Entities;

public sealed class PostEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = null!;
    public string? Content { get; set; }

    public Guid UserId { get; set; }
    public UserEntity User { get; set; }

    public ICollection<ImageEntity> Images { get; set; } = new List<ImageEntity>();
    public ICollection<LikeEntity> Likes { get; set; } = new List<LikeEntity>();
    public ICollection<CommentEntity> Comments { get; set; } = new List<CommentEntity>();
}
