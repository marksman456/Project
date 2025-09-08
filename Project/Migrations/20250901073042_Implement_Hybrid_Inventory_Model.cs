using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class Implement_Hybrid_Inventory_Model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "ProductDetail",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ProductDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSerialized",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "OrderDetail",
                type: "decimal(5,2)",
                nullable: true,
                defaultValue: 1.00m,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldDefaultValue: 1.00m);

            migrationBuilder.CreateIndex(
                name: "UQ__ProductD__048A0008FA6EEC8F",
                table: "ProductDetail",
                column: "SerialNumber",
                unique: true,
                filter: "[SerialNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ__ProductD__048A0008FA6EEC8F",
                table: "ProductDetail");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProductDetail");

            migrationBuilder.DropColumn(
                name: "IsSerialized",
                table: "Product");

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "ProductDetail",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "OrderDetail",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 1.00m,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true,
                oldDefaultValue: 1.00m);

            migrationBuilder.CreateIndex(
                name: "UQ__ProductD__048A0008FA6EEC8F",
                table: "ProductDetail",
                column: "SerialNumber",
                unique: true);
        }
    }
}
