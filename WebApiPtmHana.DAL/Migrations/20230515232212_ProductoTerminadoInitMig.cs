using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPtmHana.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ProductoTerminadoInitMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductoTerminados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoNombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Clasification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id_Linea = table.Column<int>(type: "int", nullable: true),
                    FechaPesaje = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PesoTotal = table.Column<float>(type: "real", nullable: true),
                    NumTubos = table.Column<int>(type: "int", nullable: true),
                    TurnoPT = table.Column<int>(type: "int", nullable: true),
                    AtadoPT = table.Column<float>(type: "real", nullable: true),
                    TaraPT = table.Column<float>(type: "real", nullable: true),
                    AnilloPT = table.Column<float>(type: "real", nullable: true),
                    FlejePT = table.Column<float>(type: "real", nullable: true),
                    FolioPT = table.Column<float>(type: "real", nullable: true),
                    KgxHrbyPt = table.Column<float>(type: "real", nullable: true),
                    Calidad = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoTerminados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoTerminados_Lineas_Id_Linea",
                        column: x => x.Id_Linea,
                        principalTable: "Lineas",
                        principalColumn: "Id_linea");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductoTerminados_Id_Linea",
                table: "ProductoTerminados",
                column: "Id_Linea");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductoTerminados");
        }
    }
}
