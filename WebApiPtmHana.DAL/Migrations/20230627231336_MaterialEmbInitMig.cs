using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPtmHana.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MaterialEmbInitMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumTubos",
                table: "SkuInfos");

            migrationBuilder.AddColumn<float>(
                name: "AnilloCant",
                table: "SkuInfos",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "ArpillaCant",
                table: "SkuInfos",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "CostalesCant",
                table: "SkuInfos",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "FlejeCant",
                table: "SkuInfos",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "MaderaCant",
                table: "SkuInfos",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "TarimaCant",
                table: "SkuInfos",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnilloCant",
                table: "SkuInfos");

            migrationBuilder.DropColumn(
                name: "ArpillaCant",
                table: "SkuInfos");

            migrationBuilder.DropColumn(
                name: "CostalesCant",
                table: "SkuInfos");

            migrationBuilder.DropColumn(
                name: "FlejeCant",
                table: "SkuInfos");

            migrationBuilder.DropColumn(
                name: "MaderaCant",
                table: "SkuInfos");

            migrationBuilder.DropColumn(
                name: "TarimaCant",
                table: "SkuInfos");

            migrationBuilder.AddColumn<int>(
                name: "NumTubos",
                table: "SkuInfos",
                type: "int",
                nullable: true);
        }
    }
}
