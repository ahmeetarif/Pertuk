using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pertuk.Entities.Models;

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
        public virtual DbSet<AnswerReplies> AnswerReplies { get; set; }
        public virtual DbSet<Answers> Answers { get; set; }
        public virtual DbSet<BannedUsers> BannedUsers { get; set; }
        public virtual DbSet<DeletedQuestions> DeletedQuestions { get; set; }
        public virtual DbSet<DeletedUsers> DeletedUsers { get; set; }
        public virtual DbSet<Questions> Questions { get; set; }
        public virtual DbSet<StudentUsers> StudentUsers { get; set; }
        public virtual DbSet<TeacherUsers> TeacherUsers { get; set; }
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnswerReplies>(entity =>
            {
                entity.Property(e => e.AnswerId).HasColumnName("Answer_Id");

                entity.Property(e => e.CreatedAt).HasColumnName("Created_at");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.UpdatedAt).HasColumnName("Updated_at");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("User_Id")
                    .HasMaxLength(450);

                entity.HasOne(d => d.Answer)
                    .WithMany(p => p.AnswerReplies)
                    .HasForeignKey(d => d.AnswerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AnswerReplies_Answers_AnswerId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AnswerReplies)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AnswerReplies_AspNetUsers_UserId");
            });

            modelBuilder.Entity<Answers>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnName("Created_at");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("Is_Active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsRight).HasColumnName("Is_Right");

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

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.TokenId);

                entity.Property(e => e.TokenId).HasColumnName("token_id");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasColumnName("token")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id")
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RefreshToken)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__RefreshTo__user___2EDAF651");
            });

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnName("Created_at");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.Fullname).HasMaxLength(100);

                entity.Property(e => e.IsFrom)
                    .HasColumnName("Is_From")
                    .HasMaxLength(50);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.Points).HasDefaultValueSql("((1))");

                entity.Property(e => e.ProfileImagePath).IsRequired();

                entity.Property(e => e.UpdatedAt).HasColumnName("Updated_at");

                entity.Property(e => e.UserName).HasMaxLength(256);
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

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("Is_Active")
                    .HasDefaultValueSql("((1))");

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

                entity.Property(e => e.AcademicOf)
                    .HasColumnName("Academic_Of")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedAt).HasColumnName("Created_at");

                entity.Property(e => e.DepartmentOf)
                    .HasColumnName("Department_Of")
                    .HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.ForGrade).HasColumnName("For_Grade");

                entity.Property(e => e.ForStudent)
                    .HasColumnName("For_Student")
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("Is_Active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Subject)
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

                entity.Property(e => e.Department).HasMaxLength(100);

                entity.Property(e => e.Grade).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.StudentUsers)
                    .HasForeignKey<StudentUsers>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<TeacherUsers>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).HasColumnName("User_Id");

                entity.Property(e => e.AcademicOf)
                    .HasColumnName("Academic_Of")
                    .HasMaxLength(150);

                entity.Property(e => e.DepartmentOf)
                    .HasColumnName("Department_Of")
                    .HasMaxLength(150);

                entity.Property(e => e.ForStudent)
                    .HasColumnName("For_Student")
                    .HasMaxLength(100);

                entity.Property(e => e.IsVerified).HasColumnName("Is_Verified");

                entity.Property(e => e.Subject).HasMaxLength(100);

                entity.Property(e => e.UniversityName)
                    .HasColumnName("University_Name")
                    .HasMaxLength(256);

                entity.Property(e => e.YearsOfExperience).HasColumnName("Years_Of_Experience");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.TeacherUsers)
                    .HasForeignKey<TeacherUsers>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            base.OnModelCreating(modelBuilder);
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
