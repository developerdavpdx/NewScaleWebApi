using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPtmHana.DAL.Migrations
{
    /// <inheritdoc />
    public partial class TicketPlantaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlantaTicket",
                table: "TicketsInfos",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlantaTicket",
                table: "TicketsInfos");
        }
    }
}
