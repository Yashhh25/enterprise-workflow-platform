using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("a1000001-0001-4001-8001-000000000001"), "Employee" },
                    { new Guid("a1000001-0001-4001-8001-000000000002"), "Manager" },
                    { new Guid("a1000001-0001-4001-8001-000000000003"), "HR" },
                    { new Guid("a1000001-0001-4001-8001-000000000004"), "Finance" },
                    { new Guid("a1000001-0001-4001-8001-000000000005"), "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0001-4001-8001-000000000001"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0001-4001-8001-000000000002"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0001-4001-8001-000000000003"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0001-4001-8001-000000000004"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("a1000001-0001-4001-8001-000000000005"));
        }
    }
}
