using Microsoft.EntityFrameworkCore.Migrations;

namespace app.Data.Migrations
{
    public partial class UpdateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Company");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EpumpDataId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Company_EpumpData_EpumpDataId",
                        column: x => x.EpumpDataId,
                        principalTable: "EpumpData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Company_EpumpDataId",
                table: "Company",
                column: "EpumpDataId",
                unique: true);
        }
    }
}
