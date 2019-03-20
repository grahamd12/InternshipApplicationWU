using Microsoft.EntityFrameworkCore.Migrations;

namespace Interactive_Internship_Application.Migrations
{
    public partial class IdentitySchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "AspNetUsers");
        }
    }
}
