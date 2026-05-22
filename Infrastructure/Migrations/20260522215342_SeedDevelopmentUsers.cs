using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDevelopmentUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "LastName", "PasswordHash", "RoleId", "TenantId" },
                values: new object[,]
                {
                    { new Guid("c1000001-0001-4001-8001-000000000001"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "employee@test.com", "Demo", "Employee", "$2a$11$8WY7IQx/VYPXFeXk8VMC0ey5q3J5oAY.1yKet7h9iS3gNRWrv2pDy", new Guid("a1000001-0001-4001-8001-000000000001"), new Guid("b1000001-0001-4001-8001-000000000001") },
                    { new Guid("c1000001-0001-4001-8001-000000000002"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager@test.com", "Demo", "Manager", "$2a$11$8WY7IQx/VYPXFeXk8VMC0ey5q3J5oAY.1yKet7h9iS3gNRWrv2pDy", new Guid("a1000001-0001-4001-8001-000000000002"), new Guid("b1000001-0001-4001-8001-000000000001") },
                    { new Guid("c1000001-0001-4001-8001-000000000003"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "hr@test.com", "Demo", "Hr", "$2a$11$8WY7IQx/VYPXFeXk8VMC0ey5q3J5oAY.1yKet7h9iS3gNRWrv2pDy", new Guid("a1000001-0001-4001-8001-000000000003"), new Guid("b1000001-0001-4001-8001-000000000001") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-4001-8001-000000000001"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-4001-8001-000000000002"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-4001-8001-000000000003"));
        }
    }
}
