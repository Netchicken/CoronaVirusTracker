using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirusTracker.Migrations
{
    public partial class BusinessNameToTracker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ASPNetUsersIdfk",
                table: "Tracker",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "BusinessName",
                table: "Tracker",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessName",
                table: "Tracker");

            migrationBuilder.AlterColumn<Guid>(
                name: "ASPNetUsersIdfk",
                table: "Tracker",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
