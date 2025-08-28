using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class Add_Discount_And_OrderFK_To_Details : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "QuotationDetail",
                type: "decimal(3,2)",
                precision: 3,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RelatedOrderID",
                table: "InventoryMovement",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovement_RelatedOrderID",
                table: "InventoryMovement",
                column: "RelatedOrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovement_Order_RelatedOrderID",
                table: "InventoryMovement",
                column: "RelatedOrderID",
                principalTable: "Order",
                principalColumn: "OrderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovement_Order_RelatedOrderID",
                table: "InventoryMovement");

            migrationBuilder.DropIndex(
                name: "IX_InventoryMovement_RelatedOrderID",
                table: "InventoryMovement");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "QuotationDetail");

            migrationBuilder.DropColumn(
                name: "RelatedOrderID",
                table: "InventoryMovement");
        }
    }
}
