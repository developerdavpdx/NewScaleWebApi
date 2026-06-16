using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPtmHana.DAL.Migrations
{
    /// <inheritdoc />
    public partial class TicketsInitMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketsInfos",
                columns: table => new
                {
                    IdTicket = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Folio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrdenFabricacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Turno = table.Column<int>(type: "int", nullable: false),
                    Id_Linea = table.Column<int>(type: "int", nullable: false),
                    Operacion = table.Column<int>(type: "int", nullable: false),
                    NumTubos = table.Column<int>(type: "int", nullable: true),
                    PesoNeto = table.Column<float>(type: "real", nullable: true),
                    Medida = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Porcentaje = table.Column<float>(type: "real", nullable: true),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaPesaje = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AsignadoTicket = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketsInfos", x => x.IdTicket);
                    table.ForeignKey(
                        name: "FK_TicketsInfos_Lineas_Id_Linea",
                        column: x => x.Id_Linea,
                        principalTable: "Lineas",
                        principalColumn: "Id_linea",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketsInfos_Id_Linea",
                table: "TicketsInfos",
                column: "Id_Linea");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketsInfos");
        }
    }
}
