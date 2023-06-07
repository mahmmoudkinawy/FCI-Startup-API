namespace API.Entities;
public sealed class GroupEntity
{
    public string Name { get; set; }
    public ICollection<ConnectionEntity> Connections { get; set; } = new List<ConnectionEntity>();
}
