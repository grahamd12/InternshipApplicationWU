using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace Interactive_Internship_Application.Models
{
    public partial class ApplicationDbContext : IdentityDbContext 
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<Models.ApplicationDbContext> options)
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
                //keep the below comments, this is how to extract appSettings to get dbconnection out of source code. 
          /*      string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string appRoot = System.IO.Path.GetDirectoryName(location);

                var builder = new ConfigurationBuilder()
                    .SetBasePath(appRoot)
                    .AddJsonFile("appsettings.Development.json");

                var config = builder.Build();
            */
                optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=IIP;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            //    optionsBuilder.UseSqlServer(config.GetConnectionString("LocalServer"));

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity<ApplicationData>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.DataKeyId })

                    .HasName("PK__APPLICAT__71F78C459BE1EFDE");


                entity.ToTable("APPLICATION_DATA");

                entity.Property(e => e.RecordId).HasColumnName("record_id");

                entity.Property(e => e.DataKeyId).HasColumnName("data_key_id");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.DataKey)
                    .WithMany(p => p.ApplicationData)
                    .HasForeignKey(d => d.DataKeyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__APPLICATI__data___2759D01A");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.ApplicationData)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)

                    .HasConstraintName("FK__APPLICATI__recor__2665ABE1");

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

                entity.Property(e => e.RequiredField).HasColumnName("required_field");
            });

            modelBuilder.Entity<EmployerLogin>(entity =>
            {
                entity.ToTable("EMPLOYER_LOGIN");

                entity.HasIndex(e => e.Email)

                    .HasName("UQ__EMPLOYER__AB6E6164B0FD1D1F")

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
                
                entity.Property(e => e.LastLogin).HasColumnName("last_login");


                entity.HasOne(d => d.StudentEmailNavigation)
                    .WithMany(p => p.EmployerLogin)
                    .HasPrincipalKey(p => p.Email)
                    .HasForeignKey(d => d.StudentEmail)
                    .OnDelete(DeleteBehavior.Cascade)

                    .HasConstraintName("FK__EMPLOYER___stude__1BE81D6E");

            });

            modelBuilder.Entity<FacultyInformation>(entity =>
            {
                entity.HasKey(e => e.CourseName)

                    .HasName("PK__FACULTY___B5B2A66B8381A0F4");

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

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.StudentEmail)
                    .IsRequired()
                    .HasColumnName("student_email")
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.HasOne(d => d.Employer)
                    .WithMany(p => p.StudentAppNum)
                    .HasForeignKey(d => d.EmployerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)

                    .HasConstraintName("FK__STUDENT_A__emplo__1FB8AE52");


                entity.HasOne(d => d.StudentEmailNavigation)
                    .WithMany(p => p.StudentAppNum)
                    .HasPrincipalKey(p => p.Email)
                    .HasForeignKey(d => d.StudentEmail)
                    .OnDelete(DeleteBehavior.ClientSetNull)

                    .HasConstraintName("FK__STUDENT_A__stude__1EC48A19");


            });

            modelBuilder.Entity<StudentInformation>(entity =>
            {
                entity.ToTable("STUDENT_INFORMATION");

                entity.HasIndex(e => e.Email)

                    .HasName("UQ__STUDENT___AB6E6164073E76EB")

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
