using Microsoft.EntityFrameworkCore.Migrations;

namespace Imperium_Incursions_Waitlist.Migrations
{
    public partial class Test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Wings",
                table: "Fleets",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "JabberNotifications",
                table: "Accounts",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool));

            migrationBuilder.CreateIndex(
                name: "IX_WaitingPilots_SystemId",
                table: "WaitingPilots",
                column: "SystemId");

            migrationBuilder.AddForeignKey(
                name: "FK_WaitingPilots_Systems_SystemId",
                table: "WaitingPilots",
                column: "SystemId",
                principalTable: "Systems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaitingPilots_Systems_SystemId",
                table: "WaitingPilots");

            migrationBuilder.DropIndex(
                name: "IX_WaitingPilots_SystemId",
                table: "WaitingPilots");

            migrationBuilder.DropColumn(
                name: "Wings",
                table: "Fleets");

            migrationBuilder.AlterColumn<bool>(
                name: "JabberNotifications",
                table: "Accounts",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);
        }
    }
}
