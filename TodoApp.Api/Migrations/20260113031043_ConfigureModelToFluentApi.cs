using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureModelToFluentApi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TodoItems",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_IsCompleted",
                table: "TodoItems",
                column: "IsCompleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodoItems_IsCompleted",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "TodoItems");
        }
    }
}
