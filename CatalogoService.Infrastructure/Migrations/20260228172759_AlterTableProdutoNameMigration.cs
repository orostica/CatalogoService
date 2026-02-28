using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogoService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableProdutoNameMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_categoria_categoria_id",
                table: "Produtos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Produtos",
                table: "Produtos");

            migrationBuilder.RenameTable(
                name: "Produtos",
                newName: "produto");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_nome",
                table: "produto",
                newName: "IX_produto_nome");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_categoria_id",
                table: "produto",
                newName: "IX_produto_categoria_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_produto",
                table: "produto",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_produto_categoria_categoria_id",
                table: "produto",
                column: "categoria_id",
                principalTable: "categoria",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_produto_categoria_categoria_id",
                table: "produto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_produto",
                table: "produto");

            migrationBuilder.RenameTable(
                name: "produto",
                newName: "Produtos");

            migrationBuilder.RenameIndex(
                name: "IX_produto_nome",
                table: "Produtos",
                newName: "IX_Produtos_nome");

            migrationBuilder.RenameIndex(
                name: "IX_produto_categoria_id",
                table: "Produtos",
                newName: "IX_Produtos_categoria_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Produtos",
                table: "Produtos",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_categoria_categoria_id",
                table: "Produtos",
                column: "categoria_id",
                principalTable: "categoria",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
