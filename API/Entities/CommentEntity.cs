namespace API.Entities;
public sealed class CommentEntity
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public DateTime? CreatedAt { get; set; }

    public PostEntity? Post { get; set; }
    public Guid? PostId { get; set; }

    public UserEntity Owner { get; set; }
    public Guid OwnerId { get; set; }
}
