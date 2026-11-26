using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PgrStudentManagement.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPhdApplicationsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhdApplications",
                columns: table => new
                {
                    ApplicationReference = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EmailAddress = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ProgrammeCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ModeOfStudy = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ProposedStartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProposedThesisTitle = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    ResearchSummary = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    FundingSource = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    ProposedSupervisorStaffNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    SubmittedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DecisionDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EnrolledStudentNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhdApplications", x => x.ApplicationReference);
                    table.ForeignKey(
                        name: "FK_PhdApplications_AcademicSupervisors_ProposedSupervisorStaffNumber",
                        column: x => x.ProposedSupervisorStaffNumber,
                        principalTable: "AcademicSupervisors",
                        principalColumn: "StaffNumber",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhdApplications_Students_EnrolledStudentNumber",
                        column: x => x.EnrolledStudentNumber,
                        principalTable: "Students",
                        principalColumn: "StudentNumber",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhdApplications_EmailAddress",
                table: "PhdApplications",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_PhdApplications_EnrolledStudentNumber",
                table: "PhdApplications",
                column: "EnrolledStudentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_PhdApplications_ProposedSupervisorStaffNumber",
                table: "PhdApplications",
                column: "ProposedSupervisorStaffNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhdApplications");
        }
    }
}

