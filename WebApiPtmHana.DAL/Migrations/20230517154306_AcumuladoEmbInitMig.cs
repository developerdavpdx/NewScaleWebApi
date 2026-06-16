using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPtmHana.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AcumuladoEmbInitMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcumuladoEmbarques",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Proceso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Familia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Acumulado = table.Column<float>(type: "real", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcumuladoEmbarques", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "BitHistorialPesadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdHistorialPesada = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoNombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineaPtH = table.Column<int>(type: "int", nullable: true),
                    PesoTotalPtH = table.Column<float>(type: "real", nullable: true),
                    NumTubosPtH = table.Column<int>(type: "int", nullable: true),
                    AtadoPTH = table.Column<float>(type: "real", nullable: true),
                    TaraPTH = table.Column<float>(type: "real", nullable: true),
                    AnilloPTH = table.Column<float>(type: "real", nullable: true),
                    FlejePTH = table.Column<float>(type: "real", nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BitHistorialPesadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BitHistorialPesadas_HistorialPesadas_IdHistorialPesada",
                        column: x => x.IdHistorialPesada,
                        principalTable: "HistorialPesadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BitHistorialPesadas_IdHistorialPesada",
                table: "BitHistorialPesadas",
                column: "IdHistorialPesada");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcumuladoEmbarques");

            migrationBuilder.DropTable(
                name: "BitHistorialPesadas");
        }
    }
}
