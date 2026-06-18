using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdFavSpotFav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Spots_Users_UserId",
            //     table: "Spots");

            // migrationBuilder.RenameColumn(
            //     name: "UserId",
            //     table: "Spots",
            //     newName: "User_Id");

            // migrationBuilder.RenameColumn(
            //     name: "CreatedAt",
            //     table: "Spots",
            //     newName: "created_at");
            //
            // migrationBuilder.RenameColumn(
            //     name: "Id",
            //     table: "Spots",
            //     newName: "SpotsId");
            //
            // migrationBuilder.RenameIndex(
            //     name: "IX_Spots_UserId",
            //     table: "Spots",
            //     newName: "IX_Spots_User_Id");
            //
            // migrationBuilder.AddColumn<string>(
            //     name: "WhatsAppUrl",
            //     table: "Users",
            //     type: "text",
            //     nullable: true);
            //
            // migrationBuilder.AddColumn<string>(
            //     name: "avatar_url",
            //     table: "Users",
            //     type: "text",
            //     nullable: true);

            migrationBuilder.CreateTable(
                name: "Ads",
                columns: table => new
                {
                    AdId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    LocationArea = table.Column<string>(type: "text", nullable: false),
                    ContactLink = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ads", x => x.AdId);
                    table.ForeignKey(
                        name: "FK_Ads_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteSpots",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpotId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteSpots", x => new { x.UserId, x.SpotId });
                    table.ForeignKey(
                        name: "FK_FavoriteSpots_Spots_SpotId",
                        column: x => x.SpotId,
                        principalTable: "Spots",
                        principalColumn: "SpotsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteSpots_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagsId = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagsId);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteAds",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteAds", x => new { x.UserId, x.AdId });
                    table.ForeignKey(
                        name: "FK_FavoriteAds_Ads_AdId",
                        column: x => x.AdId,
                        principalTable: "Ads",
                        principalColumn: "AdId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteAds_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ads_UserId",
                table: "Ads",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteAds_AdId",
                table: "FavoriteAds",
                column: "AdId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteSpots_SpotId",
                table: "FavoriteSpots",
                column: "SpotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Spots_Users_User_Id",
                table: "Spots",
                column: "User_Id",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Spots_Users_User_Id",
            //     table: "Spots");

            migrationBuilder.DropTable(
                name: "FavoriteAds");

            migrationBuilder.DropTable(
                name: "FavoriteSpots");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Ads");

            migrationBuilder.DropColumn(
                name: "WhatsAppUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "avatar_url",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Spots",
                newName: "CreatedAt");

            // migrationBuilder.RenameColumn(
            //     name: "User_Id",
            //     table: "Spots",
            //     newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "SpotsId",
                table: "Spots",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Spots_User_Id",
                table: "Spots",
                newName: "IX_Spots_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Spots_Users_UserId",
                table: "Spots",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
