using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoRum.Data.Migrations
{
    public partial class RefactorTutorRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TutorLearnerSubjectId",
                table: "TutorRequest",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TutorRequest_TutorLearnerSubjectId",
                table: "TutorRequest",
                column: "TutorLearnerSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorRequest_TutorLearnerSubject_TutorLearnerSubjectId",
                table: "TutorRequest",
                column: "TutorLearnerSubjectId",
                principalTable: "TutorLearnerSubject",
                principalColumn: "TutorLearnerSubjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorRequest_TutorLearnerSubject_TutorLearnerSubjectId",
                table: "TutorRequest");

            migrationBuilder.DropIndex(
                name: "IX_TutorRequest_TutorLearnerSubjectId",
                table: "TutorRequest");

            migrationBuilder.DropColumn(
                name: "TutorLearnerSubjectId",
                table: "TutorRequest");
        }
    }
}
