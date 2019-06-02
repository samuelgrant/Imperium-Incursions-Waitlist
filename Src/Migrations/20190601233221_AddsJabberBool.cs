using Microsoft.EntityFrameworkCore.Migrations;

namespace Imperium_Incursions_Waitlist.Migrations
{
    public partial class AddsJabberBool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "JabberNotifications",
                table: "Accounts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JabberNotifications",
                table: "Accounts");
        }
    }
}
