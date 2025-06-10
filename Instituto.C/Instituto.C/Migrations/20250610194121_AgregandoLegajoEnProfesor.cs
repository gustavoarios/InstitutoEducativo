using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instituto.C.Migrations
{
    /// <inheritdoc />
    public partial class AgregandoLegajoEnProfesor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Legajo",
                table: "Personas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Legajo",
                table: "Personas");
        }
    }
}
