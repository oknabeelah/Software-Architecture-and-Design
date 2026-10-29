using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Adapters.Persistence.Relational.Models;

namespace PgrStudentManagement.Adapters.Persistence.Relational;
public class StudentManagementDbContext : DbContext
{
    public StudentManagementDbContext(DbContextOptions<StudentManagementDbContext> options): base(options)
    {
        
    }
    public DbSet<StudentRecord> Students => Set<StudentRecord>();
    public DbSet<EnrolmentRecord> Enrolments => Set<EnrolmentRecord>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<StudentRecord>(student =>
        {
            student.ToTable("Students");
            student.HasKey(record => record.StudentNumber);
            student.Property(record => record.StudentNumber).HasMaxLength(20).IsRequired();
            student.Property(record => record.FirstName).HasMaxLength(100).IsRequired();
            student.Property(record => record.LastName).HasMaxLength(100).IsRequired();
            student.Property(record => record.EmailAddress).HasMaxLength(255).IsRequired();
            student.HasIndex(record => record.EmailAddress).IsUnique();
        });

        modelBuilder.Entity<EnrolmentRecord>(enrolment =>
        {
            enrolment.ToTable("Enrolments");
            enrolment.HasKey(record => record.EnrolmentId);
            enrolment.Property(record => record.StudentNumber).HasMaxLength(20).IsRequired();
            enrolment.Property(record => record.ProgrammeCode).HasMaxLength(20).IsRequired();
            enrolment.Property(record => record.ModeOfStudy).HasMaxLength(20).IsRequired();
            enrolment.Property(record => record.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        });

        modelBuilder.Entity<EnrolmentRecord>()
            .HasOne(enrolment => enrolment.Student) // Enrolment has one Student
            .WithMany(student => student.Enrolments) // Student has many Enrolments
            .HasForeignKey(enrolment => enrolment.StudentNumber) // Specify StudentNumber as the foreign key
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete to avoid deleting students when enrolments are deleted

    }
}