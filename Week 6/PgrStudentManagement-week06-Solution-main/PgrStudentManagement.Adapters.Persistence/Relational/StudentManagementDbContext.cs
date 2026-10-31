using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Adapters.Persistence.Relational.Models;

namespace PgrStudentManagement.Adapters.Persistence.Relational;

public class StudentManagementDbContext : DbContext
{
    public StudentManagementDbContext(
        DbContextOptions<StudentManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<StudentRecord> Students => Set<StudentRecord>();
    public DbSet<EnrolmentRecord> Enrolments => Set<EnrolmentRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StudentRecord>(entity =>
        {
            entity.ToTable("Students");
            entity.HasKey(s => s.StudentNumber);
            entity.Property(s => s.StudentNumber).HasMaxLength(20);
            entity.Property(s => s.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(s => s.LastName).IsRequired().HasMaxLength(100);
            entity.Property(s => s.EmailAddress).IsRequired().HasMaxLength(255);
            entity.HasIndex(s => s.EmailAddress).IsUnique();
        });

        modelBuilder.Entity<EnrolmentRecord>(entity =>
        {
            entity.ToTable("Enrolments");
            entity.HasKey(e => e.EnrolmentId);
            entity.Property(e => e.StudentNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ProgrammeCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ModeOfStudy).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(30);

            entity.HasOne(e => e.Student)
                .WithMany(s => s.Enrolments)
                .HasForeignKey(e => e.StudentNumber)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.StudentNumber);
        });
    }
}
