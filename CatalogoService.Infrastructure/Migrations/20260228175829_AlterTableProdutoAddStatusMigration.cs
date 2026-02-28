using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogoService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableProdutoAddStatusMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "produto",
                newName: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "produto",
                newName: "Status");
        }
    }
}
