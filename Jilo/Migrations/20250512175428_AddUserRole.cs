using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Jilo.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "character varying", nullable: false),
                    discrip = table.Column<string>(type: "character varying", nullable: true),
                    avatar = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("games_pk", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    username = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    email = table.Column<string>(type: "character varying", nullable: false),
                    passwordhash = table.Column<string>(type: "character varying", nullable: false),
                    socialcredits = table.Column<double>(type: "double precision", nullable: true),
                    data_registration = table.Column<DateOnly>(type: "date", nullable: false),
                    avatar = table.Column<string>(type: "character varying", nullable: true),
                    last_online = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    comm = table.Column<string>(type: "character varying", nullable: true),
                    grade = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_pk", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "comm",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    id_user = table.Column<int>(type: "integer", nullable: true),
                    comm = table.Column<string>(type: "character varying", nullable: true),
                    grade = table.Column<double>(type: "double precision", nullable: true),
                    targetuser = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("comm_pk", x => x.id);
                    table.ForeignKey(
                        name: "comm_user_fk",
                        column: x => x.id_user,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "games_user",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "integer", nullable: false),
                    id_game = table.Column<int>(type: "integer", nullable: false),
                    rank = table.Column<string>(type: "character varying", nullable: true),
                    time_in_game = table.Column<string>(type: "character varying", nullable: true),
                    role = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("games_user_pk", x => new { x.id_user, x.id_game });
                    table.ForeignKey(
                        name: "games_user_games_fk",
                        column: x => x.id_game,
                        principalTable: "games",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "games_user_user_fk",
                        column: x => x.id_user,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_comm_id_user",
                table: "comm",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_games_user_id_game",
                table: "games_user",
                column: "id_game");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comm");

            migrationBuilder.DropTable(
                name: "games_user");

            migrationBuilder.DropTable(
                name: "games");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
