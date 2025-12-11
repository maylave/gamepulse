using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Reviews",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Reviews",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<int>(
                name: "GameId1",
                table: "Reviews",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Reviews",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_GameId1",
                table: "Reviews",
                column: "GameId1");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId1",
                table: "Reviews",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId1",
                table: "Reviews",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Games_GameId1",
                table: "Reviews",
                column: "GameId1",
                principalTable: "Games",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId1",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Games_GameId1",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_GameId1",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_UserId1",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "GameId1",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Reviews",
                newName: "Date");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Reviews",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);
        }
    }
}
