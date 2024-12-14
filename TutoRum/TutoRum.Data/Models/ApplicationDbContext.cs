using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using static System.Net.WebRequestMethods;

namespace TutoRum.Data.Models
{
    public partial class ApplicationDbContext : IdentityDbContext<AspNetUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
      

        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; } = null!;
        public virtual DbSet<Bill> Bills { get; set; } = null!;
        public virtual DbSet<BillingEntry> BillingEntries { get; set; } = null!;
        public virtual DbSet<Certificate> Certificates { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<TeachingLocation> TeachingLocations { get; set; } = null!;
        public virtual DbSet<Tutor> Tutors { get; set; } = null!;
        public virtual DbSet<TutorLearnerSubject> TutorLearnerSubjects { get; set; } = null!;
        public virtual DbSet<TutorSubject> TutorSubjects { get; set; } = null!;
        public DbSet<TutorTeachingLocations> TutorTeachingLocations { get; set; }
        public virtual DbSet<QualificationLevel> QualificationLevel { get; set; } = null!;
        public virtual DbSet<TutorRequest> TutorRequest { get; set; }
        public virtual DbSet<Faq> Faq { get; set; }
        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<PaymentRequest> PaymentRequests { get; set; }
        public DbSet<TutorRequestTutor> TutorRequestTutors { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<TutorRequestTutor>()
           .HasKey(trt => new { trt.TutorRequestId, trt.TutorId }); // Primary Key là sự kết hợp giữa TutorRequestId và TutorId

            modelBuilder.Entity<TutorRequestTutor>()
                .HasOne(trt => trt.TutorRequest)
                .WithMany(tr => tr.TutorRequestTutors)
                .HasForeignKey(trt => trt.TutorRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TutorRequestTutor>()
                .HasOne(trt => trt.Tutor)
                .WithMany(t => t.TutorRequestTutors)
                .HasForeignKey(trt => trt.TutorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TutorTeachingLocations>()
           .HasKey(tl => new { tl.TutorId, tl.TeachingLocationId });

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);

            modelBuilder.Entity<UserToken>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTokens)
                .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<PaymentRequest>(entity =>
            {
                entity.HasKey(e => e.PaymentRequestId);
                entity.Property(e => e.BankCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Amount).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.HasOne(e => e.Tutor)
                      .WithMany(t => t.PaymentRequests)
                      .HasForeignKey(e => e.TutorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.AspNetUser)  
                .WithOne(u => u.Admin)    
                .HasForeignKey<Admin>(a => a.AdminId);

            modelBuilder.Entity<TutorTeachingLocations>()
                .HasOne(tl => tl.Tutor)
                .WithMany(t => t.TutorTeachingLocations)
                .HasForeignKey(tl => tl.TutorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TutorTeachingLocations_Tutor");

            modelBuilder.Entity<TutorTeachingLocations>()
                .HasOne(tl => tl.TeachingLocation)
                .WithMany(tl => tl.Tutors)
                .HasForeignKey(tl => tl.TeachingLocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TutorTeachingLocations_TeachingLocation");

            modelBuilder.Entity<TutorTeachingLocations>()
                .ToTable("TutorTeachingLocations");

            base.OnModelCreating(modelBuilder);

            // Định nghĩa khóa chính cho IdentityUserLogin<Guid>
            modelBuilder.Entity<IdentityUserLogin<Guid>>()
                .HasKey(login => new { login.LoginProvider, login.ProviderKey });

            // Định nghĩa khóa chính cho IdentityUserRole<Guid>
            modelBuilder.Entity<IdentityUserRole<Guid>>()
                .HasKey(role => new { role.UserId, role.RoleId });

            // Định nghĩa khóa chính cho IdentityUserToken<Guid>
            modelBuilder.Entity<IdentityUserToken<Guid>>()
                .HasKey(token => new { token.UserId, token.LoginProvider, token.Name });

            // Định nghĩa các thực thể khác
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin");
                entity.Property(e => e.AdminId).ValueGeneratedNever();
                entity.Property(e => e.HireDate).HasColumnType("date");
                entity.Property(e => e.Position).HasMaxLength(255);
                entity.Property(e => e.Salary).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.HasKey(e => e.ScheduleId); // Đặt khóa chính

                entity.HasOne(e => e.Tutor) // Liên kết đến bảng Tutor
                      .WithMany(t => t.Schedules) // Giáo viên có thể có nhiều lịch
                      .HasForeignKey(e => e.TutorId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.TutorRequest) // Liên kết đến TutorRequest
                      .WithMany(tr => tr.Schedules) // TutorRequest có thể có nhiều Schedule
                      .HasForeignKey(e => e.TutorRequestID)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.tutorLearnerSubject) // Liên kết đến môn học
                      .WithMany()
                      .HasForeignKey(e => e.TutorLearnerSubjectId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PostCategory>()
                .HasKey(pc => pc.PostType);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.PostCategory)
                .WithMany()
                .HasForeignKey(p => p.PostType)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("[NormalizedUserName] IS NOT NULL");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.AddressDetail).HasMaxLength(256);

                entity.Property(e => e.AddressId)
                    .HasMaxLength(256)
                    .HasColumnName("AddressID");

                entity.Property(e => e.AvatarUrl)
                    .HasMaxLength(250)
                    .IsFixedLength();

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("DOB");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.Status).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);


            });


            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("Bill");

                // Bỏ index này vì chúng ta không còn sử dụng BillingEntryId trong Bill
                // entity.HasIndex(e => e.BillingEntryId, "IX_Bill_BillingEntryID");

                entity.Property(e => e.BillId).HasColumnName("BillID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Discount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.TotalBill).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                // Loại bỏ liên kết với BillingEntryId trong Bill, vì giờ đây mỗi Bill có nhiều BillingEntry
            });


            modelBuilder.Entity<BillingEntry>(entity =>
            {
                entity.ToTable("BillingEntry");

                entity.HasIndex(e => e.TutorLearnerSubjectId, "IX_BillingEntry_TutorLearnerSubjectId");

                entity.Property(e => e.BillingEntryId).HasColumnName("BillingEntryID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EndDateTime).HasColumnType("datetime");

                entity.Property(e => e.Rate).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.StartDateTime).HasColumnType("datetime");

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.TutorLearnerSubject)
                    .WithMany(p => p.BillingEntries)
                    .HasForeignKey(d => d.TutorLearnerSubjectId)
                    .HasConstraintName("FK__BillingEn__Tutor__4F7CD00D");

               
            });


            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.ToTable("Certificate");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ImgUrl).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Tutor)
                    .WithMany(p => p.Certificates)
                    .HasForeignKey(d => d.TutorId)
                    .HasConstraintName("FK_Certificate_Tutor");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedback");

                entity.HasIndex(e => e.TutorLearnerSubjectId, "IX_Feedback_TutorLearnerSubjectId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Rating).HasColumnType("decimal(3, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.TutorLearnerSubject)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.TutorLearnerSubjectId)
                    .HasConstraintName("FK__Feedback__TutorL__49C3F6B7");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.HasIndex(e => e.BillId, "IX_Payment_BillID");

                entity.Property(e => e.PaymentId).HasColumnName("PaymentID");

                entity.Property(e => e.AmountPaid).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BillId).HasColumnName("BillID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Currency).HasMaxLength(10);

                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentMethod).HasMaxLength(50);

                entity.Property(e => e.PaymentStatus).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Bill)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.BillId)
                    .HasConstraintName("FK__Payment__BillID__5535A963");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post");

                entity.HasIndex(e => e.AdminId, "IX_Post_AdminId");

                entity.Property(e => e.PostType).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Thumbnail).HasMaxLength(255);

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.AdminId)
                    .HasConstraintName("FK_Post_Admin");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("Subject");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.SubjectName).HasMaxLength(255);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TeachingLocation>(entity =>
            {
                entity.ToTable("TeachingLocation");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Tutor>(entity =>
            {
                entity.ToTable("Tutor");

                entity.Property(e => e.TutorId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Rating).HasColumnType("decimal(3, 2)");

                entity.Property(e => e.Specialization).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.TutorNavigation)
                    .WithOne(p => p.Tutor)
                    .HasForeignKey<Tutor>(d => d.TutorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tutor_AspNetUsers");

            });

            modelBuilder.Entity<TutorLearnerSubject>(entity =>
            {
                entity.ToTable("TutorLearnerSubject");

                entity.HasIndex(e => e.LearnerId, "IX_TutorLearnerSubject_LearnerId");

                entity.HasIndex(e => e.TutorSubjectId, "IX_TutorLearnerSubject_TutorSubjectId");

                entity.Property(e => e.ContractUrl).HasMaxLength(255);

                entity.Property(e => e.Location).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.TutorSubject)
                    .WithMany(p => p.TutorLearnerSubjects)
                    .HasForeignKey(d => d.TutorSubjectId)
                    .HasConstraintName("FK__TutorLear__Tutor__45F365D3");
            });

            modelBuilder.Entity<TutorSubject>(entity =>
            {
                entity.ToTable("TutorSubject");

                entity.HasIndex(e => e.SubjectId, "IX_TutorSubject_SubjectId");

                entity.HasIndex(e => e.TutorId, "IX_TutorSubject_TutorId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Rate).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.TutorSubjects)
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("FK__TutorSubj__Subje__4316F928");

                entity.HasOne(d => d.Tutor)
                    .WithMany(p => p.TutorSubjects)
                    .HasForeignKey(d => d.TutorId)
                    .HasConstraintName("FK_TutorSubject_Tutor");
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


    }
}