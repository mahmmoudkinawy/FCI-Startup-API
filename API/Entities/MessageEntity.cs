namespace API.Entities;
public sealed class MessageEntity
{
    public Guid Id { get; set; }
    public string? SenderName { get; set; }
    public string? RecipientName { get; set; }
    public string? Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime MessageSentDate { get; set; } = DateTime.UtcNow;
    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }

    public Guid SenderId { get; set; }
    public Guid RecipientId { get; set; }
    public UserEntity Sender { get; set; }
    public UserEntity Recipient { get; set; }
}
