namespace API.Entities;
public sealed class UserEntity : IdentityUser<Guid>
{
    public string? Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime GraduationYear { get; set; }

    public ICollection<PostEntity> Posts { get; set; } = new List<PostEntity>();

    public ICollection<ImageEntity> Images { get; set; } = new List<ImageEntity>();

    public ICollection<UserFollowEntity> FollowersUsers { get; set; }
    public ICollection<UserFollowEntity> FollowedByUsers { get; set; }

    public ICollection<MessageEntity> MessagesSent { get; set; } = new List<MessageEntity>();
    public ICollection<MessageEntity> MessagesReceived { get; set; } = new List<MessageEntity>();

    public ICollection<LikeEntity> Likes { get; set; } = new List<LikeEntity>();
}
