using Microsoft.EntityFrameworkCore.Migrations;

namespace QuizRTapi.Migrations
{
    public partial class UpdateTemplateTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategName",
                table: "QuizRTTemplateT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TopicName",
                table: "QuizRTTemplateT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategName",
                table: "QuizRTTemplateT");

            migrationBuilder.DropColumn(
                name: "TopicName",
                table: "QuizRTTemplateT");
        }
    }
}
