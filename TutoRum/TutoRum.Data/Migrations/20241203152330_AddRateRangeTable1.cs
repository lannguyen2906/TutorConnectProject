using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoRum.Data.Migrations
{
    public partial class AddRateRangeTable1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RateRangeId",
                table: "TutorRequest",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TutorRequest_RateRangeId",
                table: "TutorRequest",
                column: "RateRangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorRequest_RateRange_RateRangeId",
                table: "TutorRequest",
                column: "RateRangeId",
                principalTable: "RateRange",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorRequest_RateRange_RateRangeId",
                table: "TutorRequest");

            migrationBuilder.DropIndex(
                name: "IX_TutorRequest_RateRangeId",
                table: "TutorRequest");

            migrationBuilder.DropColumn(
                name: "RateRangeId",
                table: "TutorRequest");
        }
    }
}
