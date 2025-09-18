using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class Add_Product_ModelSpec_ManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModelSpecProduct",
                columns: table => new
                {
                    ProductsProductID = table.Column<int>(type: "int", nullable: false),
                    SpecsModelSpecID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelSpecProduct", x => new { x.ProductsProductID, x.SpecsModelSpecID });
                    table.ForeignKey(
                        name: "FK_ModelSpecProduct_ModelSpec_SpecsModelSpecID",
                        column: x => x.SpecsModelSpecID,
                        principalTable: "ModelSpec",
                        principalColumn: "ModelSpecID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModelSpecProduct_Product_ProductsProductID",
                        column: x => x.ProductsProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModelSpecProduct_SpecsModelSpecID",
                table: "ModelSpecProduct",
                column: "SpecsModelSpecID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelSpecProduct");
        }
    }
}
