using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LWMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_cod_records_ParcelId",
                table: "cod_records");

            migrationBuilder.DropIndex(
                name: "IX_bag_items_ParcelId",
                table: "bag_items");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "cod_records",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Các cột này đã được thêm thủ công hoặc qua script trước đó
            /*
            migrationBuilder.AddColumn<DateTime>(
                name: "SettledAt",
                table: "cod_records",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "cod_records",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SealNumber",
                table: "bags",
                type: "nvarchar(max)",
                nullable: true);
            */

            migrationBuilder.CreateIndex(
                name: "IX_cod_records_ParcelId_Status",
                table: "cod_records",
                columns: new[] { "ParcelId", "Status" },
                unique: true,
                filter: "[Status] = 'COLLECTED'");

            migrationBuilder.CreateIndex(
                name: "IX_bag_items_ParcelId",
                table: "bag_items",
                column: "ParcelId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_cod_records_ParcelId_Status",
                table: "cod_records");

            migrationBuilder.DropIndex(
                name: "IX_bag_items_ParcelId",
                table: "bag_items");

            migrationBuilder.DropColumn(
                name: "SettledAt",
                table: "cod_records");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "cod_records");

            migrationBuilder.DropColumn(
                name: "SealNumber",
                table: "bags");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "cod_records",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_cod_records_ParcelId",
                table: "cod_records",
                column: "ParcelId");

            migrationBuilder.CreateIndex(
                name: "IX_bag_items_ParcelId",
                table: "bag_items",
                column: "ParcelId");
        }
    }
}
