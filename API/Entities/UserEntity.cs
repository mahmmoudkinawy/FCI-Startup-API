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
}
