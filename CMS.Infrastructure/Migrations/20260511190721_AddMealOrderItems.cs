using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMealOrderItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "PackageId",
                table: "OrderItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ItemType",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MealId",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_MealId",
                table: "OrderItems",
                column: "MealId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Meals_MealId",
                table: "OrderItems",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Meals_MealId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_MealId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "MealId",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "PackageId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "OrderItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "OrderItems",
                type: "datetime2",
                nullable: true);
        }
    }
}
