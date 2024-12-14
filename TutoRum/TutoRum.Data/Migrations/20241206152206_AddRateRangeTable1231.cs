using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoRum.Data.Migrations
{
    public partial class AddRateRangeTable1231 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCloseClass",
                table: "TutorLearnerSubject",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCloseClass",
                table: "TutorLearnerSubject");
        }
    }
}
