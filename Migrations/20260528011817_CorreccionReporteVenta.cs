using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_Final.Migrations
{
    /// <inheritdoc />
    public partial class CorreccionReporteVenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reportes_Venta_Ventas_IdVenta",
                table: "Reportes_Venta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventarios",
                table: "Inventarios");

            migrationBuilder.DropIndex(
                name: "IX_Inventarios_IdProducto",
                table: "Inventarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DetallesVenta",
                table: "DetallesVenta");

            migrationBuilder.DropIndex(
                name: "IX_DetallesVenta_IdVenta",
                table: "DetallesVenta");

            migrationBuilder.DropColumn(
                name: "IdInventario",
                table: "Inventarios");

            migrationBuilder.DropColumn(
                name: "IdDetalleVenta",
                table: "DetallesVenta");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventarios",
                table: "Inventarios",
                columns: new[] { "IdProducto", "IdSucursal" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DetallesVenta",
                table: "DetallesVenta",
                columns: new[] { "IdVenta", "IdProducto" });

            migrationBuilder.AddForeignKey(
                name: "FK_Reportes_Venta_Ventas_IdVenta",
                table: "Reportes_Venta",
                column: "IdVenta",
                principalTable: "Ventas",
                principalColumn: "IdVenta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reportes_Venta_Ventas_IdVenta",
                table: "Reportes_Venta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventarios",
                table: "Inventarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DetallesVenta",
                table: "DetallesVenta");

            migrationBuilder.AddColumn<int>(
                name: "IdInventario",
                table: "Inventarios",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "IdDetalleVenta",
                table: "DetallesVenta",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventarios",
                table: "Inventarios",
                column: "IdInventario");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DetallesVenta",
                table: "DetallesVenta",
                column: "IdDetalleVenta");

            migrationBuilder.CreateIndex(
                name: "IX_Inventarios_IdProducto",
                table: "Inventarios",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVenta_IdVenta",
                table: "DetallesVenta",
                column: "IdVenta");

            migrationBuilder.AddForeignKey(
                name: "FK_Reportes_Venta_Ventas_IdVenta",
                table: "Reportes_Venta",
                column: "IdVenta",
                principalTable: "Ventas",
                principalColumn: "IdVenta",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
