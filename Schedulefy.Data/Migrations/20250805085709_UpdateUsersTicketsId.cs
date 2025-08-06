using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schedulefy.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUsersTicketsId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersTickets",
                table: "UsersTickets");

            migrationBuilder.AddColumn<int>(
                name: "UserTicketId",
                table: "UsersTickets",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersTickets",
                table: "UsersTickets",
                column: "UserTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersTickets_UserId",
                table: "UsersTickets",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersTickets",
                table: "UsersTickets");

            migrationBuilder.DropIndex(
                name: "IX_UsersTickets_UserId",
                table: "UsersTickets");

            migrationBuilder.DropColumn(
                name: "UserTicketId",
                table: "UsersTickets");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersTickets",
                table: "UsersTickets",
                columns: new[] { "UserId", "TicketId" });
        }
    }
}
