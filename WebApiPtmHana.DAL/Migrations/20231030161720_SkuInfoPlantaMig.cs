using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPtmHana.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SkuInfoPlantaMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Planta",
                table: "SkuInfos",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Planta",
                table: "SkuInfos");
        }
    }
}
