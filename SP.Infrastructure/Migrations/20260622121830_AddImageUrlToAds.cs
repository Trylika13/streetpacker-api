using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToAds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Tags",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Ads",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Ad_Tags",
                columns: table => new
                {
                    AdId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ad_Tags", x => new { x.AdId, x.TagId });
                    table.ForeignKey(
                        name: "FK_Ad_Tags_Ads_AdId",
                        column: x => x.AdId,
                        principalTable: "Ads",
                        principalColumn: "AdId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ad_Tags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Spot_Tags",
                columns: table => new
                {
                    SpotId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spot_Tags", x => new { x.SpotId, x.TagId });
                    table.ForeignKey(
                        name: "FK_Spot_Tags_Spots_SpotId",
                        column: x => x.SpotId,
                        principalTable: "Spots",
                        principalColumn: "SpotsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Spot_Tags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ad_Tags_TagId",
                table: "Ad_Tags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Spot_Tags_TagId",
                table: "Spot_Tags",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ad_Tags");

            migrationBuilder.DropTable(
                name: "Spot_Tags");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Ads");
        }
    }
}
