using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddedFollowUserTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Followers",
                columns: table => new
                {
                    SourceUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Followers", x => new { x.SourceUserId, x.DestinationUserId });
                    table.ForeignKey(
                        name: "FK_Followers_AspNetUsers_DestinationUserId",
                        column: x => x.DestinationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Followers_AspNetUsers_SourceUserId",
                        column: x => x.SourceUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Followers_DestinationUserId",
                table: "Followers",
                column: "DestinationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Followers");
        }
    }
}
