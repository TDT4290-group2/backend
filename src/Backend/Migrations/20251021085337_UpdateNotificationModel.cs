using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotificationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Notification");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "Notification",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<DateTime>(
                name: "HappenedAt",
                table: "Notification",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dataType",
                table: "Notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "exceedingLevel",
                table: "Notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "userMessage",
                table: "Notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "value",
                table: "Notification",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HappenedAt",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "dataType",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "exceedingLevel",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "userMessage",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "value",
                table: "Notification");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "Notification",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Notification",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Notification",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Notification",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
