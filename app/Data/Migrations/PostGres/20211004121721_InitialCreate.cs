using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace telegrambot.Data.Migrations.PostGres
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EpumpData",
                columns: table => new
                {
                    ID = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<string>(type: "text", nullable: true),
                    AuthKey = table.Column<string>(type: "text", nullable: true),
                    AuthDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpumpData", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: true),
                    AuthDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    CurrentBranch = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    EpumpDataId = table.Column<string>(type: "character varying(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ChatId);
                    table.ForeignKey(
                        name: "FK_Users_EpumpData_EpumpDataId",
                        column: x => x.EpumpDataId,
                        principalTable: "EpumpData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_EpumpDataId",
                table: "Users",
                column: "EpumpDataId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "EpumpData");
        }
    }
}
