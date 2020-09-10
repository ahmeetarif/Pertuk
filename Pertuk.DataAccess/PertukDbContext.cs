using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Pertuk.Entities.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Pertuk.DataAccess
{
    public partial class PertukDbContext : IdentityDbContext<ApplicationUser>
    {
        public PertukDbContext()
        {
        }

        public PertukDbContext(DbContextOptions options)
            : base(options)
        {

        }
        public virtual DbSet<Answers> Answers { get; set; }
        public virtual DbSet<BannedUsers> BannedUsers { get; set; }
        public virtual DbSet<DeletedQuestions> DeletedQuestions { get; set; }
        public virtual DbSet<DeletedUsers> DeletedUsers { get; set; }
        public virtual DbSet<Questions> Questions { get; set; }
        public virtual DbSet<StudentUsers> StudentUsers { get; set; }
        public virtual DbSet<TeacherUsers> TeacherUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnName("Created_at");

                entity.Property(e => e.Department).HasMaxLength(80);

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.Fullname)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UpdatedAt).HasColumnName("Updated_at");

                entity.Property(e => e.ProfileImagePath).IsRequired();

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<Answers>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnName("Created_at");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.QuestionId).HasColumnName("Question_Id");

                entity.Property(e => e.UpdatedAt).HasColumnName("Updated_at");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("User_Id")
                    .HasMaxLength(450);

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<BannedUsers>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_BannedUsers_1");

                entity.Property(e => e.UserId).HasColumnName("User_Id");

                entity.Property(e => e.BannedAt).HasColumnName("Banned_at");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("Is_Active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.BannedUsers)
                    .HasForeignKey<BannedUsers>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<DeletedQuestions>(entity =>
            {
                entity.HasKey(e => e.QuestionId);

                entity.Property(e => e.QuestionId)
                    .HasColumnName("Question_Id")
                    .ValueGeneratedNever();

                entity.Property(e => e.DeletedAt).HasColumnName("Deleted_at");

                entity.Property(e => e.Reason).HasMaxLength(450);

                entity.HasOne(d => d.Question)
                    .WithOne(p => p.DeletedQuestions)
                    .HasForeignKey<DeletedQuestions>(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<DeletedUsers>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_DeletedUsers_1");

                entity.Property(e => e.UserId).HasColumnName("User_Id");

                entity.Property(e => e.DeletedAt).HasColumnName("Deleted_at");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("Is_Active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Reason).HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.DeletedUsers)
                    .HasForeignKey<DeletedUsers>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Questions>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnName("Created_at");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ImageUrl).IsRequired();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("Is_Active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedAt).HasColumnName("Updated_at");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("User_Id")
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<StudentUsers>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).HasColumnName("User_Id");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.StudentUsers)
                    .HasForeignKey<StudentUsers>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<TeacherUsers>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).HasColumnName("User_Id");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.TeacherUsers)
                    .HasForeignKey<TeacherUsers>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        #region Transaction Configuration

        private IDbContextTransaction _transaction;

        public IDbContextTransaction BeginTransaction()
        {
            return _transaction = Database.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                SaveChanges();
                _transaction.Commit();
            }
            finally
            {
                _transaction.Dispose();
            }
        }

        public void Rollback()
        {
            try
            {
                _transaction.Rollback();
            }
            finally
            {
                _transaction.Dispose();
            }
        }

        #endregion

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
