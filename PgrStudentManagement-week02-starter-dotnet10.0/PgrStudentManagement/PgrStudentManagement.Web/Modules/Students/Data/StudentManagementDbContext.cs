using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Web.Models;

namespace PgrStudentManagement.Web.Data;
public class StudentManagementDbContext : DbContext
{
    public StudentManagementDbContext(DbContextOptions<StudentManagementDbContext> options): base(options)
    {
        
    }
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Enrolment>()
            .HasOne(enrolment => enrolment.Student) // Enrolment has one Student
            .WithMany(student => student.Enrolments) // Student has many Enrolments
            .HasForeignKey(enrolment => enrolment.StudentNumber) // Specify StudentNumber as the foreign key
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete to avoid deleting students when enrolments are deleted

        modelBuilder.Entity<Enrolment>().Property(enrolment => enrolment.Status).HasConversion<string>().HasMaxLength(30); 
        // Store EnrolmentStatus (property) as readable text in the database with a maximum length of 30 characters


    }
}