using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LWMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantFilterAndSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EstimatedDays",
                table: "service_types",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "service_types",
                columns: new[] { "Id", "BaseFee", "Code", "CreatedAt", "EstimatedDays", "IsActive", "IsDeleted", "MaxDays", "Name", "RowVersion", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("aa1e9c52-7360-4927-aa72-5b91cf8e9661"), 15000m, "STANDARD", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "3-5 ngày", true, false, 0, "Giao hàng Tiêu chuẩn", null, null },
                    { new Guid("bb1e9c52-7360-4927-aa72-5b91cf8e9662"), 25000m, "FAST", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "1-2 ngày", true, false, 0, "Giao hàng Nhanh", null, null },
                    { new Guid("cc1e9c52-7360-4927-aa72-5b91cf8e9663"), 45000m, "EXPRESS", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trong ngày", true, false, 0, "Giao hàng Hỏa tốc", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "service_types",
                keyColumn: "Id",
                keyValue: new Guid("aa1e9c52-7360-4927-aa72-5b91cf8e9661"));

            migrationBuilder.DeleteData(
                table: "service_types",
                keyColumn: "Id",
                keyValue: new Guid("bb1e9c52-7360-4927-aa72-5b91cf8e9662"));

            migrationBuilder.DeleteData(
                table: "service_types",
                keyColumn: "Id",
                keyValue: new Guid("cc1e9c52-7360-4927-aa72-5b91cf8e9663"));

            migrationBuilder.DropColumn(
                name: "EstimatedDays",
                table: "service_types");
        }
    }
}
