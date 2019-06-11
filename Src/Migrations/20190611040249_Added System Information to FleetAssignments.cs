using Microsoft.EntityFrameworkCore.Migrations;

namespace Imperium_Incursions_Waitlist.Migrations
{
    public partial class AddedSystemInformationtoFleetAssignments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SystemId",
                table: "FleetAssignments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FleetAssignments_SystemId",
                table: "FleetAssignments",
                column: "SystemId");

            migrationBuilder.AddForeignKey(
                name: "FK_FleetAssignments_Systems_SystemId",
                table: "FleetAssignments",
                column: "SystemId",
                principalTable: "Systems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FleetAssignments_Systems_SystemId",
                table: "FleetAssignments");

            migrationBuilder.DropIndex(
                name: "IX_FleetAssignments_SystemId",
                table: "FleetAssignments");

            migrationBuilder.DropColumn(
                name: "SystemId",
                table: "FleetAssignments");
        }
    }
}
