using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace app.Data.Migrations
{
    public partial class RemovedStatusTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "loginStatusEpump");

            migrationBuilder.DropTable(
                name: "loginStatusTelegram");

            migrationBuilder.AddColumn<DateTime>(
                name: "AuthDate",
                table: "EpumpData",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthDate",
                table: "EpumpData");

            migrationBuilder.CreateTable(
                name: "loginStatusEpump",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpumpDataId = table.Column<string>(type: "text", nullable: true),
                    IsLoggedIn = table.Column<bool>(type: "boolean", nullable: false),
                    LoginDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loginStatusEpump", x => x.Id);
                    table.ForeignKey(
                        name: "FK_loginStatusEpump_EpumpData_EpumpDataId",
                        column: x => x.EpumpDataId,
                        principalTable: "EpumpData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "loginStatusTelegram",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsLoggedIn = table.Column<bool>(type: "boolean", nullable: false),
                    LoginDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserChatId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loginStatusTelegram", x => x.Id);
                    table.ForeignKey(
                        name: "FK_loginStatusTelegram_Users_UserChatId",
                        column: x => x.UserChatId,
                        principalTable: "Users",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_loginStatusEpump_EpumpDataId",
                table: "loginStatusEpump",
                column: "EpumpDataId");

            migrationBuilder.CreateIndex(
                name: "IX_loginStatusTelegram_UserChatId",
                table: "loginStatusTelegram",
                column: "UserChatId");
        }
    }
}
