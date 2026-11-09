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
    public DbSet<AcademicSupervisorRecord> AcademicSupervisors => Set<AcademicSupervisorRecord>();
    public DbSet<AcademicSupervisorEligibleRoleRecord> AcademicSupervisorEligibleRoles => Set<AcademicSupervisorEligibleRoleRecord>();
    public DbSet<SupervisoryAssignmentRecord> SupervisoryAssignments => Set<SupervisoryAssignmentRecord>();

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

        modelBuilder.Entity<AcademicSupervisorRecord>(entity =>
        {
            entity.ToTable("AcademicSupervisors");
            entity.HasKey(supervisor => supervisor.StaffNumber);
            entity.Property(supervisor => supervisor.StaffNumber).HasMaxLength(20);
            entity.Property(supervisor => supervisor.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(supervisor => supervisor.LastName).IsRequired().HasMaxLength(100);
            entity.Property(supervisor => supervisor.EmailAddress).IsRequired().HasMaxLength(255);
            entity.Property(supervisor => supervisor.EmploymentType).HasConversion<string>().IsRequired();
            entity.HasIndex(supervisor => supervisor.EmailAddress).IsUnique();
        });

        modelBuilder.Entity<AcademicSupervisorEligibleRoleRecord>(entity =>
        {
            entity.ToTable("AcademicSupervisorEligibleRoles");
            entity.Property(role => role.Role)
                .HasConversion<string>()
                .IsRequired();
            entity.HasKey(role => new { role.SupervisorStaffNumber, role.Role });
            entity.HasOne(role => role.Supervisor)
                .WithMany(supervisor => supervisor.EligibleRoles)
                .HasForeignKey(role => role.SupervisorStaffNumber)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SupervisoryAssignmentRecord>(entity =>
        {
            entity.ToTable("SupervisoryAssignments");
            entity.HasKey(assignment => new
            {
                assignment.StudentNumber,
                assignment.SupervisorStaffNumber,
                assignment.Role,
                assignment.StartDate
            });
            entity.Property(assignment => assignment.StudentNumber).HasMaxLength(20);
            entity.Property(assignment => assignment.SupervisorStaffNumber).HasMaxLength(20);
            entity.Property(assignment => assignment.Role)
                .HasConversion<string>()
                .IsRequired();
            entity.HasOne(assignment => assignment.Student)
                .WithMany(student => student.SupervisoryAssignments)
                .HasForeignKey(assignment => assignment.StudentNumber)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(assignment => assignment.Supervisor)
                .WithMany(supervisor => supervisor.Assignments)
                .HasForeignKey(assignment => assignment.SupervisorStaffNumber)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(assignment => assignment.StudentNumber);
        });
    }
}
