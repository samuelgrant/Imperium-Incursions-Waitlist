using Microsoft.EntityFrameworkCore.Migrations;

namespace Imperium_Incursions_Waitlist.Migrations
{
    public partial class TEST : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pilots_Accounts_AccountId",
                table: "Pilots");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Pilots",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Pilots_Accounts_AccountId",
                table: "Pilots",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pilots_Accounts_AccountId",
                table: "Pilots");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Pilots",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pilots_Accounts_AccountId",
                table: "Pilots",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
