namespace API.Entities;

public sealed class UserFollowEntity
{
    public Guid SourceUserId { get; set; }
    public UserEntity SourceUser { get; set; }

    public Guid DestinationUserId { get; set; }
    public UserEntity DestinationUser { get; set; }
}
