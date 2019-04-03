using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Interactive_Internship_Application.Models
{
    public partial class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApplicationData> ApplicationData { get; set; }
        public virtual DbSet<ApplicationTemplate> ApplicationTemplate { get; set; }
        public virtual DbSet<EmployerLogin> EmployerLogin { get; set; }
        public virtual DbSet<FacultyInformation> FacultyInformation { get; set; }
        public virtual DbSet<StudentAppNum> StudentAppNum { get; set; }
        public virtual DbSet<StudentInformation> StudentInformation { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity<ApplicationData>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.DataKeyId })
                    .HasName("PK__APPLICAT__71F78C459288457C");

                entity.ToTable("APPLICATION_DATA");

                entity.Property(e => e.RecordId).HasColumnName("record_id");

                entity.Property(e => e.DataKeyId).HasColumnName("data_key_id");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.DataKey)
                    .WithMany(p => p.ApplicationData)
                    .HasForeignKey(d => d.DataKeyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__APPLICATI__data___7C6F7215");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.ApplicationData)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__APPLICATI__recor__7B7B4DDC");
            });

            modelBuilder.Entity<ApplicationTemplate>(entity =>
            {
                entity.ToTable("APPLICATION_TEMPLATE");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ControlType)
                    .HasColumnName("control_type")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.Deleted).HasColumnName("deleted");

                entity.Property(e => e.Entity)
                    .HasColumnName("entity")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.FieldDescription)
                    .HasColumnName("field_description")
                    .HasMaxLength(1024)
                    .IsUnicode(false);

                entity.Property(e => e.FieldName)
                    .IsRequired()
                    .HasColumnName("field_name")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ProperName)
                    .HasColumnName("proper_name")
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EmployerLogin>(entity =>
            {
                entity.ToTable("EMPLOYER_LOGIN");

                entity.HasIndex(e => e.Email)
                    .HasName("UQ__EMPLOYER__AB6E61646C94CE16")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.Pin).HasColumnName("pin");

                entity.Property(e => e.StudentEmail)
                    .HasColumnName("student_email")
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.HasOne(d => d.StudentEmailNavigation)
                    .WithMany(p => p.EmployerLogin)
                    .HasPrincipalKey(p => p.Email)
                    .HasForeignKey(d => d.StudentEmail)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__EMPLOYER___stude__70FDBF69");
            });

            modelBuilder.Entity<FacultyInformation>(entity =>
            {
                entity.HasKey(e => e.CourseName)
                    .HasName("PK__FACULTY___B5B2A66B32DAFA1E");

                entity.ToTable("FACULTY_INFORMATION");

                entity.Property(e => e.CourseName)
                    .HasColumnName("course_name")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.DeptRepEmail)
                    .HasColumnName("dept_rep_email")
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.ProfEmail)
                    .HasColumnName("prof_email")
                    .HasMaxLength(512)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<StudentAppNum>(entity =>
            {
                entity.ToTable("STUDENT_APP_NUM");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.EmployerId).HasColumnName("employer_id");

                entity.Property(e => e.StudentEmail)
                    .IsRequired()
                    .HasColumnName("student_email")
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.HasOne(d => d.Employer)
                    .WithMany(p => p.StudentAppNum)
                    .HasForeignKey(d => d.EmployerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__STUDENT_A__emplo__74CE504D");

                entity.HasOne(d => d.StudentEmailNavigation)
                    .WithMany(p => p.StudentAppNum)
                    .HasPrincipalKey(p => p.Email)
                    .HasForeignKey(d => d.StudentEmail)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__STUDENT_A__stude__73DA2C14");
            });

            modelBuilder.Entity<StudentInformation>(entity =>
            {
                entity.ToTable("STUDENT_INFORMATION");

                entity.HasIndex(e => e.Email)
                    .HasName("UQ__STUDENT___AB6E61645D73A02D")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.LastLogin)
                    .HasColumnName("last_login")
                    .HasColumnType("date");
            });
        }
    }
}
