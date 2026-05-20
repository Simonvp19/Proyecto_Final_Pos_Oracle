using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_Final.Migrations
{
    /// <inheritdoc />
    public partial class AgregarReportes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reportes",
                columns: table => new
                {
                    IdReporte = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalVentaEfectivo = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalVentaTarjeta = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalEstimadoVenta = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalRealVenta = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Descuadre = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reportes", x => x.IdReporte);
                });

            migrationBuilder.CreateTable(
                name: "Reportes_Venta",
                columns: table => new
                {
                    IdReporteVenta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVenta = table.Column<int>(type: "int", nullable: false),
                    IdReporte = table.Column<int>(type: "int", nullable: false),
                    VentaIdVenta = table.Column<int>(type: "int", nullable: false),
                    ReporteIdReporte = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reportes_Venta", x => x.IdReporteVenta);
                    table.ForeignKey(
                        name: "FK_Reportes_Venta_Reportes_ReporteIdReporte",
                        column: x => x.ReporteIdReporte,
                        principalTable: "Reportes",
                        principalColumn: "IdReporte",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reportes_Venta_Ventas_VentaIdVenta",
                        column: x => x.VentaIdVenta,
                        principalTable: "Ventas",
                        principalColumn: "IdVenta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_Venta_ReporteIdReporte",
                table: "Reportes_Venta",
                column: "ReporteIdReporte");

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_Venta_VentaIdVenta",
                table: "Reportes_Venta",
                column: "VentaIdVenta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reportes_Venta");

            migrationBuilder.DropTable(
                name: "Reportes");
        }
    }
}
