using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestApp.Data.Migrations
{
    public partial class Add_Basic_Audit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "CreatedBy",
                "Customers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                "CreatedDateTime",
                "Customers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                "UpdatedBy",
                "Customers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                "UpdatedDateTime",
                "Customers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                "Version",
                "Customers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                "CreatedBy",
                "Addresses",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                "CreatedDateTime",
                "Addresses",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                "UpdatedBy",
                "Addresses",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                "UpdatedDateTime",
                "Addresses",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                "Version",
                "Addresses",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "CreatedBy",
                "Customers");

            migrationBuilder.DropColumn(
                "CreatedDateTime",
                "Customers");

            migrationBuilder.DropColumn(
                "UpdatedBy",
                "Customers");

            migrationBuilder.DropColumn(
                "UpdatedDateTime",
                "Customers");

            migrationBuilder.DropColumn(
                "Version",
                "Customers");

            migrationBuilder.DropColumn(
                "CreatedBy",
                "Addresses");

            migrationBuilder.DropColumn(
                "CreatedDateTime",
                "Addresses");

            migrationBuilder.DropColumn(
                "UpdatedBy",
                "Addresses");

            migrationBuilder.DropColumn(
                "UpdatedDateTime",
                "Addresses");

            migrationBuilder.DropColumn(
                "Version",
                "Addresses");
        }
    }
}