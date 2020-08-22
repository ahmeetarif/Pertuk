using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pertuk.Entities.Models;

namespace Pertuk.DataAccess
{
    public partial class PertukDbContext : IdentityDbContext<ApplicationUser>
    {
        public PertukDbContext(DbContextOptions<PertukDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.BannedAt).HasColumnName("Banned_at");

                entity.Property(e => e.CreatedAt).HasColumnName("Created_at");

                entity.Property(e => e.DeletedAt).HasColumnName("Deleted_at");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Grade).HasMaxLength(100);

                entity.Property(e => e.IsBanned).HasColumnName("Is_Banned");

                entity.Property(e => e.IsDeleted).HasColumnName("Is_Deleted");

                entity.Property(e => e.Lastname)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UpdatedAt).HasColumnName("Updated_at");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<BannedUsers>(entity =>
            {
                entity.Property(e => e.BannedUserId)
                    .IsRequired()
                    .HasColumnName("Banned_User_Id")
                    .HasMaxLength(450);

                entity.Property(e => e.CreatedAdminId)
                    .IsRequired()
                    .HasColumnName("Created_Admin_Id")
                    .HasMaxLength(450);

                entity.Property(e => e.CreatedAt).HasColumnName("Created_at");

                entity.Property(e => e.DeletedAt).HasColumnName("Deleted_at");

                entity.Property(e => e.IsDeleted).HasColumnName("Is_Deleted");

                entity.Property(e => e.Reason).IsRequired();

                entity.Property(e => e.UpdateAt).HasColumnName("Update_at");

                entity.HasOne(d => d.BannedUser)
                    .WithMany(p => p.BannedUsersBannedUser)
                    .HasForeignKey(d => d.BannedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("BannedUsers_AspNetUsers_BannedUserIdFK");

                entity.HasOne(d => d.CreatedAdmin)
                    .WithMany(p => p.BannedUsersCreatedAdmin)
                    .HasForeignKey(d => d.CreatedAdminId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("BannedUsers_AspNetUsers_CreatedAdminIdFK");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
