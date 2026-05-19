using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PackageSelectionRuleToPackageSizeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageSelectionRules_Packages_PackageId",
                table: "PackageSelectionRules");

            migrationBuilder.RenameColumn(
                name: "PackageId",
                table: "PackageSelectionRules",
                newName: "PackageSizeId");

            migrationBuilder.RenameIndex(
                name: "IX_PackageSelectionRules_PackageId",
                table: "PackageSelectionRules",
                newName: "IX_PackageSelectionRules_PackageSizeId");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "PackageSizes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PackageSelectionRules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PackageSelectionOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageSelectionRules_PackageSizes_PackageSizeId",
                table: "PackageSelectionRules",
                column: "PackageSizeId",
                principalTable: "PackageSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageSelectionRules_PackageSizes_PackageSizeId",
                table: "PackageSelectionRules");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "PackageSizes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PackageSelectionRules");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PackageSelectionOptions");

            migrationBuilder.RenameColumn(
                name: "PackageSizeId",
                table: "PackageSelectionRules",
                newName: "PackageId");

            migrationBuilder.RenameIndex(
                name: "IX_PackageSelectionRules_PackageSizeId",
                table: "PackageSelectionRules",
                newName: "IX_PackageSelectionRules_PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageSelectionRules_Packages_PackageId",
                table: "PackageSelectionRules",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
