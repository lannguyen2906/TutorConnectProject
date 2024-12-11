using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoRum.Data.Migrations
{
    public partial class AddRateRangeTable1231222 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "UserTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "TutorSubject",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "TutorRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "TutorLearnerSubject",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Tutor",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "TeachingLocation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Subject",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Schedule",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Post",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "PaymentRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Payment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Feedback",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Faq",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Certificate",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "BillingEntry",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Bill",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Admin",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "TutorSubject");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "TutorRequest");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "TutorLearnerSubject");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Tutor");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "TeachingLocation");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "PaymentRequests");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Feedback");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Faq");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Certificate");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "BillingEntry");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Admin");
        }
    }
}
