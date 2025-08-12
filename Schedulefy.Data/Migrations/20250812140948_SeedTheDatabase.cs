using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Schedulefy.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedTheDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "11111111-1111-1111-1111-111111111111", 0, "227d70a5-ae4b-46da-8a80-a5214636c0cf", "publisher1@test.com", true, false, null, "PUBLISHER1@TEST.COM", "PUBLISHER1@TEST.COM", "AQAAAAIAAYagAAAAEE1FakeHashExample", null, false, "68951a92-723c-4ec7-accb-3455218763ad", false, "publisher1@test.com" },
                    { "22222222-2222-2222-2222-222222222222", 0, "6bc62172-1d34-4332-830d-f35bf4b275dc", "user1@test.com", true, false, null, "USER1@TEST.COM", "USER1@TEST.COM", "AQAAAAIAAYagAAAAEE2FakeHashExample", null, false, "fc382b54-ca03-491b-ab54-a3827d706db7", false, "user1@test.com" }
                });

            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "Id", "PricePerTicket" },
                values: new object[,]
                {
                    { 1, 50.00m },
                    { 2, 20.00m }
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "IsDeleted", "IsPremium", "Name", "PublishedOn", "PublisherId", "TicketId" },
                values: new object[,]
                {
                    { 1, 1, "An unforgettable rock concert with top bands.", "https://example.com/rocknight.jpg", false, true, "Rock Night Live", new DateTime(2025, 8, 2, 14, 9, 45, 594, DateTimeKind.Utc).AddTicks(6803), "11111111-1111-1111-1111-111111111111", 1 },
                    { 2, 7, "A charity event for local causes with live performances.", "https://example.com/charitygala.jpg", false, false, "Summer Charity Gala", new DateTime(2025, 8, 7, 14, 9, 45, 594, DateTimeKind.Utc).AddTicks(6813), "11111111-1111-1111-1111-111111111111", 2 }
                });

            migrationBuilder.InsertData(
                table: "UsersTickets",
                columns: new[] { "UserTicketId", "TickeetsCount", "TicketId", "UserId" },
                values: new object[,]
                {
                    { 1, 2, 1, "22222222-2222-2222-2222-222222222222" },
                    { 2, 4, 2, "22222222-2222-2222-2222-222222222222" }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "Content", "EventId", "UserId" },
                values: new object[,]
                {
                    { 1, "Amazing event!", 1, "22222222-2222-2222-2222-222222222222" },
                    { 2, "Can't wait for next year!", 2, "22222222-2222-2222-2222-222222222222" }
                });

            migrationBuilder.InsertData(
                table: "UsersEvents",
                columns: new[] { "EventId", "UserId" },
                values: new object[,]
                {
                    { 1, "22222222-2222-2222-2222-222222222222" },
                    { 2, "22222222-2222-2222-2222-222222222222" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UsersEvents",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { 1, "22222222-2222-2222-2222-222222222222" });

            migrationBuilder.DeleteData(
                table: "UsersEvents",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { 2, "22222222-2222-2222-2222-222222222222" });

            migrationBuilder.DeleteData(
                table: "UsersTickets",
                keyColumn: "UserTicketId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UsersTickets",
                keyColumn: "UserTicketId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "22222222-2222-2222-2222-222222222222");

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "11111111-1111-1111-1111-111111111111");

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
