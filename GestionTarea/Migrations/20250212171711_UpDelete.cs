using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTarea.Migrations
{
    /// <inheritdoc />
    public partial class UpDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Borrado",
                table: "Tarea",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Borrado",
                table: "Tarea");
        }
    }
}
