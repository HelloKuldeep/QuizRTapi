using Microsoft.EntityFrameworkCore.Migrations;

namespace QuizRTapi.Migrations
{
    public partial class UpdateTemplateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SparQL",
                table: "QuizRTTemplateT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "QuizRTTemplateT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Topic",
                table: "QuizRTTemplateT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SparQL",
                table: "QuizRTTemplateT");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "QuizRTTemplateT");

            migrationBuilder.DropColumn(
                name: "Topic",
                table: "QuizRTTemplateT");
        }
    }
}
