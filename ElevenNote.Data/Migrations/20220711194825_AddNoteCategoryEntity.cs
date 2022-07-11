using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElevenNote.Data.Migrations
{
    public partial class AddNoteCategoryEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Categories_NoteId",
                table: "Categories",
                column: "NoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Notes_NoteId",
                table: "Categories",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Notes_NoteId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_NoteId",
                table: "Categories");
        }
    }
}
