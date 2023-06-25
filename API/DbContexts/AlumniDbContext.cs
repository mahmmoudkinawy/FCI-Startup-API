namespace API.DbContexts;
public sealed class AlumniDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid,
    IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public AlumniDbContext(DbContextOptions<AlumniDbContext> options) : base(options)
    { }

    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<ImageEntity> Images { get; set; }
    public DbSet<UserFollowEntity> Followers { get; set; }
    public DbSet<MessageEntity> Messages { get; set; }
    public DbSet<GroupEntity> Groups { get; set; }
    public DbSet<ConnectionEntity> Connections { get; set; }
    public DbSet<LikeEntity> Likes { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CommentEntity>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<CommentEntity>()
            .HasOne(c => c.Owner)
            .WithMany()
            .HasForeignKey(c => c.OwnerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<LikeEntity>()
            .HasOne(u => u.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(k => k.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<LikeEntity>()
            .HasOne(u => u.Post)
            .WithMany(i => i.Likes)
            .HasForeignKey(k => k.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<GroupEntity>()
            .HasKey(g => g.Name);

        builder.Entity<GroupEntity>()
            .HasMany(c => c.Connections)
            .WithOne()
            .HasForeignKey(s => s.ConnectionId);

        builder.Entity<ConnectionEntity>()
            .HasKey(c => c.ConnectionId);

        builder.Entity<PostEntity>()
            .HasMany(c => c.Images)
            .WithOne(p => p.Post)
            .HasForeignKey(p => p.PostId);

        builder.Entity<PostEntity>()
            .HasOne(u => u.User)
            .WithMany(p => p.Posts);

        builder.Entity<ImageEntity>()
            .HasOne(u => u.User)
            .WithMany(p => p.Images);

        builder.Entity<UserFollowEntity>()
            .HasKey(k => new { k.SourceUserId, k.DestinationUserId });

        builder.Entity<UserFollowEntity>()
            .HasOne(u => u.SourceUser)
            .WithMany(u => u.FollowersUsers)
            .HasForeignKey(u => u.SourceUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<UserFollowEntity>()
            .HasOne(u => u.DestinationUser)
            .WithMany(u => u.FollowedByUsers)
            .HasForeignKey(u => u.DestinationUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<MessageEntity>()
            .HasOne(m => m.Sender)
            .WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MessageEntity>()
            .HasOne(m => m.Recipient)
            .WithMany(m => m.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
