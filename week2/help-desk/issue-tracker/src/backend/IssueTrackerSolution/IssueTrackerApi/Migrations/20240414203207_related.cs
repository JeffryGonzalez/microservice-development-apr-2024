using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IssueTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class related : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CatalogItemId",
                schema: "issues-api",
                table: "Issues",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Issues_CatalogItemId",
                schema: "issues-api",
                table: "Issues",
                column: "CatalogItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Catalog_CatalogItemId",
                schema: "issues-api",
                table: "Issues",
                column: "CatalogItemId",
                principalSchema: "issues-api",
                principalTable: "Catalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Catalog_CatalogItemId",
                schema: "issues-api",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_CatalogItemId",
                schema: "issues-api",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "CatalogItemId",
                schema: "issues-api",
                table: "Issues");
        }
    }
}
