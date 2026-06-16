using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPtmHana.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FamiliEMMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Familia",
                table: "EnvioScraps",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Familia",
                table: "EnvioScraps");
        }
    }
}
