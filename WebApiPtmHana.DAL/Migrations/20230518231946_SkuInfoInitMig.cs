using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPtmHana.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SkuInfoInitMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SkuInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MultiploCant = table.Column<int>(type: "int", nullable: false),
                    MultiploCategoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Variable = table.Column<int>(type: "int", nullable: false),
                    FijoCategoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Proceso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FijoCantidad = table.Column<float>(type: "real", nullable: false),
                    NumTubos = table.Column<int>(type: "int", nullable: true),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkuInfos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SkuInfos");
        }
    }
}
