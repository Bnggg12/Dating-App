using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<
    AppUser, AppRole, int, 
    IdentityUserClaim<int>, 
    AppUserRole, 
    IdentityUserLogin<int>, 
    IdentityRoleClaim<int>, 
    IdentityUserToken<int>>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>(entity =>
        {
            entity.ToTable("Users");

            entity.Property(e => e.PhoneNumber).HasMaxLength(10).IsFixedLength().IsUnicode(false);
            entity.Property(e => e.RefreshToken).HasMaxLength(255);
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(6);
            entity.Property(e => e.LookingFor).HasMaxLength(6);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.City).HasMaxLength(20);
            entity.Property(e => e.Mbti).HasMaxLength(4).IsFixedLength().IsUnicode(false);
            entity.Property(e => e.EducationLevel).HasMaxLength(30);
            entity.Property(e => e.FieldOfStudy).HasMaxLength(100);
            entity.Property(e => e.Institution).HasMaxLength(100);

            entity.HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AppRole>(entity =>
        {
            entity.ToTable("Roles");

            entity.HasMany(ur => ur.UserRoles)
                .WithOne(r => r.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasData(
                new AppRole { Id = 1, Name = "Member", NormalizedName = "MEMBER", ConcurrencyStamp = "member-role" },
                new AppRole { Id = 2, Name = "Moderator", NormalizedName = "MODERATOR", ConcurrencyStamp = "moderator-role" },
                new AppRole { Id = 3, Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "admin-role" }
            );
        });

        builder.Entity<AppUserRole>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        builder.Entity<Photo>(entity =>
        {
            entity.Property(p => p.Url).HasMaxLength(500);
            entity.Property(p => p.PublicId).HasMaxLength(255);
        });

        builder.Entity<UserLike>(entity =>
        {
            entity.HasKey(x => new { x.SourceMemberId, x.TargetMemberId });

            entity.HasOne(s => s.SourceMember)
            .WithMany(t => t.LikedMembers)
            .HasForeignKey(s => s.SourceMemberId)
            .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.TargetMember)
            .WithMany(t => t.LikeByMembers)
            .HasForeignKey(s => s.TargetMemberId)
            .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Message>(entity =>
        {
            entity.Property(p => p.Content).HasMaxLength(500);

            entity.HasOne(m => m.Recipient)
                .WithMany(u => u.MessagesReceived)
                .HasForeignKey(m => m.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Sender)
                .WithMany(u => u.MessagesSent)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    public DbSet<Photo> Photos { get; set; }
    public DbSet<UserLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }
}