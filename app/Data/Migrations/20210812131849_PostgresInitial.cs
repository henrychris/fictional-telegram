using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace app.Data.Migrations
{
    public partial class PostgresInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EpumpData",
                columns: table => new
                {
                    ID = table.Column<string>(type: "text", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<string>(type: "text", nullable: true),
                    AuthKey = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpumpData", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    EpumpDataId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Company_EpumpData_EpumpDataId",
                        column: x => x.EpumpDataId,
                        principalTable: "EpumpData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull,
                        onUpdate: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ChatId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: true),
                    AuthDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    CurrentBranch = table.Column<string>(type: "text", nullable: true),
                    EpumpDataId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ChatId);
                    table.ForeignKey(
                        name: "FK_Users_EpumpData_EpumpDataId",
                        column: x => x.EpumpDataId,
                        principalTable: "EpumpData",
                        principalColumn: "ID",
                        onUpdate: ReferentialAction.Cascade,
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Company_EpumpDataId",
                table: "Company",
                column: "EpumpDataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_EpumpDataId",
                table: "Users",
                column: "EpumpDataId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "EpumpData");
        }
    }
}
