using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LWMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialDemoData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "hubs",
                columns: new[] { "Id", "Address", "Capacity", "CreatedAt", "HubCode", "HubLevel", "HubType", "IsActive", "IsDeleted", "Latitude", "Longitude", "ManagerId", "Name", "OperatingHours", "ProvinceCode", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("001e9c52-7360-4927-aa72-5b91cf8e9661"), "Số 1 Cầu Giấy, Hà Nội", 10000, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "HN-01", 1, "SORTING_CENTER", true, false, null, null, null, "Hub Hà Nội - Trung tâm", null, "HN", null },
                    { new Guid("001e9c52-7360-4927-aa72-5b91cf8e9662"), "Số 100 Quận 1, TP.HCM", 12000, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "HCM-01", 1, "SORTING_CENTER", true, false, null, null, null, "Hub TP.HCM - Miền Nam", null, "HCM", null }
                });

            migrationBuilder.InsertData(
                table: "merchants",
                columns: new[] { "Id", "ApiKey", "CreatedAt", "DefaultHubId", "Email", "IsActive", "IsDeleted", "MerchantCode", "Name", "Phone", "RowVersion", "UpdatedAt" },
                values: new object[] { new Guid("221e9c52-7360-4927-aa72-5b91cf8e9661"), "lwms_test_api_key_2026", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "contact@demo-merc.com", true, false, "DEMO-MERC", "Giao Hàng Demo Store", "0888888888", null, null });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "CreatedAt", "EmployeeCode", "FullName", "HubId", "IsActive", "IsDeleted", "LastLogin", "PasswordHash", "Phone", "Role", "ShipperCapacity", "UpdatedAt", "VehicleType" },
                values: new object[] { new Guid("111e9c52-7360-4927-aa72-5b91cf8e9661"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ADM-01", "Hệ thống Admin", null, true, false, null, "$2a$11$mC7p989S.n6Y7R3YjX2YPO.Y8Y7R3YjX2YPO.Y8Y7R3YjX2YPO.", "0999999999", "Admin", null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "hubs",
                keyColumn: "Id",
                keyValue: new Guid("001e9c52-7360-4927-aa72-5b91cf8e9661"));

            migrationBuilder.DeleteData(
                table: "hubs",
                keyColumn: "Id",
                keyValue: new Guid("001e9c52-7360-4927-aa72-5b91cf8e9662"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "Id",
                keyValue: new Guid("221e9c52-7360-4927-aa72-5b91cf8e9661"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("111e9c52-7360-4927-aa72-5b91cf8e9661"));
        }
    }
}
