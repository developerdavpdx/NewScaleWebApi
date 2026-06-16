using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPtmHana.DAL.Migrations
{
    /// <inheritdoc />
    public partial class EnvioSapInitMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EnvioSAP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentEnvioSAP = table.Column<int>(type: "int", nullable: false),
                    IdHistPesada = table.Column<int>(type: "int", nullable: false),
                    IsPrelim = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvioSAP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnvioSAP_HistorialPesadas_IdHistPesada",
                        column: x => x.IdHistPesada,
                        principalTable: "HistorialPesadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnvioSAP_IdHistPesada",
                table: "EnvioSAP",
                column: "IdHistPesada");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnvioSAP");
        }
    }
}
