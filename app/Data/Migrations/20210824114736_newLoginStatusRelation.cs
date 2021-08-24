﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace app.Data.Migrations
{
    public partial class newLoginStatusRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "loginStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    isLoggedInTelegram = table.Column<bool>(type: "boolean", nullable: false),
                    isLoggedInEpump = table.Column<bool>(type: "boolean", nullable: false),
                    loginDateTelegram = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    loginDateEpump = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserChatId = table.Column<long>(type: "bigint", nullable: false),
                    EpumpDataId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loginStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_loginStatus_EpumpData_EpumpDataId",
                        column: x => x.EpumpDataId,
                        principalTable: "EpumpData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_loginStatus_Users_UserChatId",
                        column: x => x.UserChatId,
                        principalTable: "Users",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_loginStatus_EpumpDataId",
                table: "loginStatus",
                column: "EpumpDataId");

            migrationBuilder.CreateIndex(
                name: "IX_loginStatus_UserChatId",
                table: "loginStatus",
                column: "UserChatId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "loginStatus");
        }
    }
}
