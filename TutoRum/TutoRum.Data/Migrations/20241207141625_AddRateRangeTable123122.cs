using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoRum.Data.Migrations
{
    public partial class AddRateRangeTable123122 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "TutorSubject");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "TutorSubject");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "TutorRequest");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "TutorRequest");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "TutorLearnerSubject");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "TutorLearnerSubject");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Tutor");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "Tutor");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "TeachingLocation");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "TeachingLocation");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "PaymentRequests");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "PaymentRequests");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Feedback");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "Feedback");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Faq");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "Faq");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Certificate");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "Certificate");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "BillingEntry");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "BillingEntry");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Admin");

            migrationBuilder.DropColumn(
                name: "MetaKeyword",
                table: "Admin");

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "UserTokens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "TutorSubject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "TutorRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "TutorLearnerSubject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "Tutor",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "TeachingLocation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "Subject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "Schedule",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "Post",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "PaymentRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "Feedback",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "Faq",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "Certificate",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "BillingEntry",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "Bill",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reasonDesc",
                table: "Admin",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "TutorSubject");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "TutorRequest");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "TutorLearnerSubject");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "Tutor");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "TeachingLocation");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "PaymentRequests");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "Feedback");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "Faq");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "Certificate");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "BillingEntry");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "reasonDesc",
                table: "Admin");

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "UserTokens",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "UserTokens",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "TutorSubject",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "TutorSubject",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "TutorRequest",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "TutorRequest",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "TutorLearnerSubject",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "TutorLearnerSubject",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Tutor",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "Tutor",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "TeachingLocation",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "TeachingLocation",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Subject",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "Subject",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Schedule",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "Schedule",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Post",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "Post",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "PaymentRequests",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "PaymentRequests",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Payment",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "Payment",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Notifications",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "Notifications",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Feedback",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "Feedback",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Faq",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "Faq",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Certificate",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "Certificate",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "BillingEntry",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "BillingEntry",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Bill",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "Bill",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Admin",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeyword",
                table: "Admin",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
