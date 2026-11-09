using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PgrStudentManagement.Adapters.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicSupervisors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicSupervisors",
                columns: table => new
                {
                    StaffNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EmailAddress = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    EmploymentType = table.Column<string>(type: "TEXT", nullable: false),
                    TrainingCompleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicSupervisors", x => x.StaffNumber);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EmailAddress = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentNumber);
                });

            migrationBuilder.CreateTable(
                name: "AcademicSupervisorEligibleRoles",
                columns: table => new
                {
                    SupervisorStaffNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicSupervisorEligibleRoles", x => new { x.SupervisorStaffNumber, x.Role });
                    table.ForeignKey(
                        name: "FK_AcademicSupervisorEligibleRoles_AcademicSupervisors_SupervisorStaffNumber",
                        column: x => x.SupervisorStaffNumber,
                        principalTable: "AcademicSupervisors",
                        principalColumn: "StaffNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enrolments",
                columns: table => new
                {
                    EnrolmentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ProgrammeCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpectedEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModeOfStudy = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrolments", x => x.EnrolmentId);
                    table.ForeignKey(
                        name: "FK_Enrolments_Students_StudentNumber",
                        column: x => x.StudentNumber,
                        principalTable: "Students",
                        principalColumn: "StudentNumber",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupervisoryAssignments",
                columns: table => new
                {
                    StudentNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    SupervisorStaffNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExceptionAuthorised = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupervisoryAssignments", x => new { x.StudentNumber, x.SupervisorStaffNumber, x.Role, x.StartDate });
                    table.ForeignKey(
                        name: "FK_SupervisoryAssignments_AcademicSupervisors_SupervisorStaffNumber",
                        column: x => x.SupervisorStaffNumber,
                        principalTable: "AcademicSupervisors",
                        principalColumn: "StaffNumber",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupervisoryAssignments_Students_StudentNumber",
                        column: x => x.StudentNumber,
                        principalTable: "Students",
                        principalColumn: "StudentNumber",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicSupervisors_EmailAddress",
                table: "AcademicSupervisors",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrolments_StudentNumber",
                table: "Enrolments",
                column: "StudentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Students_EmailAddress",
                table: "Students",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupervisoryAssignments_StudentNumber",
                table: "SupervisoryAssignments",
                column: "StudentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_SupervisoryAssignments_SupervisorStaffNumber",
                table: "SupervisoryAssignments",
                column: "SupervisorStaffNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicSupervisorEligibleRoles");

            migrationBuilder.DropTable(
                name: "Enrolments");

            migrationBuilder.DropTable(
                name: "SupervisoryAssignments");

            migrationBuilder.DropTable(
                name: "AcademicSupervisors");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
