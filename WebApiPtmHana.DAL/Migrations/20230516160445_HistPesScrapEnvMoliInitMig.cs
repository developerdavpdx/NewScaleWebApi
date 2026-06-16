using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPtmHana.DAL.Migrations
{
    /// <inheritdoc />
    public partial class HistPesScrapEnvMoliInitMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Proceso",
                table: "ProductoTerminados",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EnvioScraps",
                columns: table => new
                {
                    IdEnvioScrap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Operador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoItem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Peso = table.Column<double>(type: "float", nullable: false),
                    Familia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubFamilia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Turno = table.Column<int>(type: "int", nullable: false),
                    Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Proceso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comentarios = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Linea = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvioScraps", x => x.IdEnvioScrap);
                });

            migrationBuilder.CreateTable(
                name: "ScrapMolinos",
                columns: table => new
                {
                    IdScrap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEnvioScrap = table.Column<int>(type: "int", nullable: true),
                    Operador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoItem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Peso = table.Column<double>(type: "float", nullable: false),
                    Familia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubFamilia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Turno = table.Column<int>(type: "int", nullable: false),
                    Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Proceso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaScrap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdRemolido = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    AtadoScrpt = table.Column<float>(type: "real", nullable: false),
                    TaraScrpt = table.Column<float>(type: "real", nullable: false),
                    AnilloScrpt = table.Column<float>(type: "real", nullable: false),
                    FlejeScrpt = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapMolinos", x => x.IdScrap);
                    table.ForeignKey(
                        name: "FK_ScrapMolinos_EnvioScraps_IdEnvioScrap",
                        column: x => x.IdEnvioScrap,
                        principalTable: "EnvioScraps",
                        principalColumn: "IdEnvioScrap");
                });

            migrationBuilder.CreateTable(
                name: "HistorialPesadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_Scrap = table.Column<int>(type: "int", nullable: true),
                    Id_ProductoTerminado = table.Column<int>(type: "int", nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdEstatusSAP = table.Column<int>(type: "int", nullable: true),
                    IdEstatusHP = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialPesadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialPesadas_EstadosSap_IdEstatusSAP",
                        column: x => x.IdEstatusSAP,
                        principalTable: "EstadosSap",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistorialPesadas_EstatusHistorialPesadas_IdEstatusHP",
                        column: x => x.IdEstatusHP,
                        principalTable: "EstatusHistorialPesadas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistorialPesadas_ProductoTerminados_Id_ProductoTerminado",
                        column: x => x.Id_ProductoTerminado,
                        principalTable: "ProductoTerminados",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistorialPesadas_ScrapMolinos_Id_Scrap",
                        column: x => x.Id_Scrap,
                        principalTable: "ScrapMolinos",
                        principalColumn: "IdScrap");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialPesadas_Id_ProductoTerminado",
                table: "HistorialPesadas",
                column: "Id_ProductoTerminado");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialPesadas_Id_Scrap",
                table: "HistorialPesadas",
                column: "Id_Scrap");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialPesadas_IdEstatusHP",
                table: "HistorialPesadas",
                column: "IdEstatusHP");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialPesadas_IdEstatusSAP",
                table: "HistorialPesadas",
                column: "IdEstatusSAP");

            migrationBuilder.CreateIndex(
                name: "IX_ScrapMolinos_IdEnvioScrap",
                table: "ScrapMolinos",
                column: "IdEnvioScrap");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialPesadas");

            migrationBuilder.DropTable(
                name: "ScrapMolinos");

            migrationBuilder.DropTable(
                name: "EnvioScraps");

            migrationBuilder.DropColumn(
                name: "Proceso",
                table: "ProductoTerminados");
        }
    }
}
