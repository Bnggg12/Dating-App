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
            entity.Property(e => e.RefreshToken).HasMaxLength(255);
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(6);
            entity.Property(e => e.LookingFor).HasMaxLength(6);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.City).HasMaxLength(20);
            entity.Property(e => e.Mbti).HasMaxLength(4).IsFixedLength();
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
            entity.HasMany(ur => ur.UserRoles)
                .WithOne(r => r.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasData(
                new AppRole { Id = 1, Name = "Member", NormalizedName = "MEMBER" },
                new AppRole { Id = 2, Name = "Moderator", NormalizedName = "MODERATOR" },
                new AppRole { Id = 3, Name = "Admin", NormalizedName = "ADMIN" }
            );
        });
    }

    public DbSet<Photo> Photos { get; set; }
}