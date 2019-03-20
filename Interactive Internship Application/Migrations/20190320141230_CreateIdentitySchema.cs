using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Interactive_Internship_Application.Migrations
{
    public partial class CreateIdentitySchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APPLICATION_TEMPLATE",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    field_name = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                    field_description = table.Column<string>(unicode: false, maxLength: 1024, nullable: true),
                    entity = table.Column<string>(unicode: false, maxLength: 128, nullable: true),
                    control_type = table.Column<string>(unicode: false, maxLength: 128, nullable: true),
                    proper_name = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    deleted = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPLICATION_TEMPLATE", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "FACULTY_INFORMATION",
                columns: table => new
                {
                    course_name = table.Column<string>(unicode: false, maxLength: 16, nullable: false),
                    prof_email = table.Column<string>(unicode: false, maxLength: 512, nullable: true),
                    dept_rep_email = table.Column<string>(unicode: false, maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FACULTY___B5B2A66B331366DA", x => x.course_name);
                });

            migrationBuilder.CreateTable(
                name: "STUDENT_INFORMATION",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    email = table.Column<string>(unicode: false, maxLength: 512, nullable: false),
                    last_login = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STUDENT_INFORMATION", x => x.id);
                    table.UniqueConstraint("AK_STUDENT_INFORMATION_email", x => x.email);
                });

            migrationBuilder.CreateTable(
                name: "APPLICATION_DATA",
                columns: table => new
                {
                    record_id = table.Column<int>(nullable: false),
                    data_key_id = table.Column<int>(nullable: false),
                    value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__APPLICAT__71F78C458526B05D", x => new { x.record_id, x.data_key_id });
                    table.ForeignKey(
                        name: "FK__APPLICATI__data___1A9EF37A",
                        column: x => x.data_key_id,
                        principalTable: "APPLICATION_TEMPLATE",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__APPLICATI__recor__19AACF41",
                        column: x => x.record_id,
                        principalTable: "STUDENT_INFORMATION",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYER_LOGIN",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    email = table.Column<string>(unicode: false, maxLength: 512, nullable: false),
                    pin = table.Column<short>(nullable: true),
                    student_email = table.Column<string>(unicode: false, maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYER_LOGIN", x => x.id);
                    table.ForeignKey(
                        name: "FK__EMPLOYER___stude__12FDD1B2",
                        column: x => x.student_email,
                        principalTable: "STUDENT_INFORMATION",
                        principalColumn: "email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_APPLICATION_DATA_data_key_id",
                table: "APPLICATION_DATA",
                column: "data_key_id");

            migrationBuilder.CreateIndex(
                name: "UQ__EMPLOYER__AB6E616458DB5CE9",
                table: "EMPLOYER_LOGIN",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYER_LOGIN_student_email",
                table: "EMPLOYER_LOGIN",
                column: "student_email");

            migrationBuilder.CreateIndex(
                name: "UQ__STUDENT___AB6E6164E42D1D12",
                table: "STUDENT_INFORMATION",
                column: "email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APPLICATION_DATA");

            migrationBuilder.DropTable(
                name: "EMPLOYER_LOGIN");

            migrationBuilder.DropTable(
                name: "FACULTY_INFORMATION");

            migrationBuilder.DropTable(
                name: "APPLICATION_TEMPLATE");

            migrationBuilder.DropTable(
                name: "STUDENT_INFORMATION");
        }
    }
}
