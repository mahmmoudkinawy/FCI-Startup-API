namespace API.Entities;
public sealed class LikeEntity
{
    public Guid Id { get; set; }
    public DateTime LikedAt { get; set; }

    public Guid PostId { get; set; }
    public PostEntity Post { get; set; }

    public Guid UserId { get; set; }
    public UserEntity User { get; set; }
}
