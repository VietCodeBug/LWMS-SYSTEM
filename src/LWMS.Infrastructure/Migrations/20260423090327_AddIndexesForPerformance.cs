using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LWMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesForPerformance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_parcels_SlaDate_Status",
                table: "parcels",
                columns: new[] { "SlaDate", "Status" },
                filter: "[SlaDate] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_parcels_SlaDate_Status",
                table: "parcels");
        }
    }
}
