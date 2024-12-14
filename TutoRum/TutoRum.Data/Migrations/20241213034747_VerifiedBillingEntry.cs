using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoRum.Data.Migrations
{
    public partial class VerifiedBillingEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "BillingEntry",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "BillingEntry");
        }
    }
}
