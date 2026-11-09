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
            entity.Property(supervisor => supervisor.StaffNumber).IsRequired().HasMaxLength(20);
            entity.Property(supervisor => supervisor.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(supervisor => supervisor.LastName).IsRequired().HasMaxLength(100);
            entity.Property(supervisor => supervisor.EmailAddress).IsRequired().HasMaxLength(255);
            entity.Property(supervisor => supervisor.EmploymentType).IsRequired().HasMaxLength(30);

            entity.HasMany(supervisor => supervisor.EligibleRoleEntries)
                .WithOne(eligibleRole => eligibleRole.AcademicSupervisor)
                .HasForeignKey(eligibleRole => eligibleRole.SupervisorStaffNumber)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AcademicSupervisorEligibleRoleRecord>(entity =>
        {
            entity.ToTable("AcademicSupervisorEligibleRoles");
            entity.HasKey(eligibleRole => new
            {
                eligibleRole.SupervisorStaffNumber,
                eligibleRole.Role
            });

            entity.Property(eligibleRole => eligibleRole.SupervisorStaffNumber).IsRequired().HasMaxLength(20);
            entity.Property(eligibleRole => eligibleRole.Role).IsRequired().HasMaxLength(40);
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

            entity.Property(assignment => assignment.StudentNumber).IsRequired().HasMaxLength(20);
            entity.Property(assignment => assignment.SupervisorStaffNumber).IsRequired().HasMaxLength(20);
            entity.Property(assignment => assignment.Role).IsRequired().HasMaxLength(40);

            entity.HasOne<StudentRecord>()
                .WithMany()
                .HasForeignKey(assignment => assignment.StudentNumber)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<AcademicSupervisorRecord>()
                .WithMany()
                .HasForeignKey(assignment => assignment.SupervisorStaffNumber)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
