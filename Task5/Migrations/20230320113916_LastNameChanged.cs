using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task5.Migrations
{
    public partial class LastNameChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Lastname",
                table: "LastNames",
                newName: "SecondName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SecondName",
                table: "LastNames",
                newName: "Lastname");
        }
    }
}
