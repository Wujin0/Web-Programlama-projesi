using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WEB_PROGRAMLAMA_PROJESİ.Migrations
{
    /// <inheritdoc />
    public partial class CalismaSaatleri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CalismaSaatiBaslangic",
                table: "Antrenorler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CalismaSaatiBitis",
                table: "Antrenorler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalismaSaatiBaslangic",
                table: "Antrenorler");

            migrationBuilder.DropColumn(
                name: "CalismaSaatiBitis",
                table: "Antrenorler");
        }
    }
}
