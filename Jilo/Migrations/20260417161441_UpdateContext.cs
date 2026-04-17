using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jilo.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "comm_user_fk",
                table: "comm");

            migrationBuilder.DropForeignKey(
                name: "games_user_games_fk",
                table: "games_user");

            migrationBuilder.DropForeignKey(
                name: "games_user_user_fk",
                table: "games_user");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "User",
                type: "character varying",
                nullable: true,
                defaultValueSql: "'\"USER\"'::character varying");

            migrationBuilder.AddColumn<string>(
                name: "discription",
                table: "User",
                type: "character varying",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "adversmet_user",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "integer", nullable: false),
                    id_game = table.Column<int>(type: "integer", nullable: false),
                    discription = table.Column<string>(type: "character varying", nullable: true),
                    date_create = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    id_second_user = table.Column<int>(type: "integer", nullable: true),
                    name_second_user = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adversmet_user", x => new { x.id_user, x.id_game });
                    table.ForeignKey(
                        name: "adversmet_user_games_fk",
                        column: x => x.id_game,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "adversmet_user_user_fk",
                        column: x => x.id_user,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "adversmet_user_unique",
                table: "adversmet_user",
                columns: new[] { "id_user", "id_game" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_adversmet_user_id_game",
                table: "adversmet_user",
                column: "id_game");

            migrationBuilder.AddForeignKey(
                name: "comm_user_fk",
                table: "comm",
                column: "id_user",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "games_user_games_fk",
                table: "games_user",
                column: "id_game",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "games_user_user_fk",
                table: "games_user",
                column: "id_user",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "comm_user_fk",
                table: "comm");

            migrationBuilder.DropForeignKey(
                name: "games_user_games_fk",
                table: "games_user");

            migrationBuilder.DropForeignKey(
                name: "games_user_user_fk",
                table: "games_user");

            migrationBuilder.DropTable(
                name: "adversmet_user");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "User");

            migrationBuilder.DropColumn(
                name: "discription",
                table: "User");

            migrationBuilder.AddForeignKey(
                name: "comm_user_fk",
                table: "comm",
                column: "id_user",
                principalTable: "User",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "games_user_games_fk",
                table: "games_user",
                column: "id_game",
                principalTable: "games",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "games_user_user_fk",
                table: "games_user",
                column: "id_user",
                principalTable: "User",
                principalColumn: "id");
        }
    }
}
