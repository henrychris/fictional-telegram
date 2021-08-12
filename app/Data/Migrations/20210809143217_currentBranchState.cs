using Microsoft.EntityFrameworkCore.Migrations;

namespace app.Data.Migrations
{
    public partial class currentBranchState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentBranch",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentBranch",
                table: "Users");
        }
    }
}
