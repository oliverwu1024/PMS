using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaintStore.API.Migrations
{
    /// <inheritdoc />
    public partial class OrderProductManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the join table first, copy the existing links into it,
            // and only then drop the old PaintProducts.OrderId column (EF's default order would lose them).
            migrationBuilder.CreateTable(
                name: "OrderPaintProduct",
                columns: table => new
                {
                    OrdersOrderId = table.Column<int>(type: "int", nullable: false),
                    ProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPaintProduct", x => new { x.OrdersOrderId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_OrderPaintProduct_Orders_OrdersOrderId",
                        column: x => x.OrdersOrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderPaintProduct_PaintProducts_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "PaintProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderPaintProduct_ProductsId",
                table: "OrderPaintProduct",
                column: "ProductsId");

            migrationBuilder.Sql(
                "INSERT INTO OrderPaintProduct (OrdersOrderId, ProductsId) " +
                "SELECT OrderId, Id FROM PaintProducts WHERE OrderId IS NOT NULL");

            migrationBuilder.DropForeignKey(
                name: "FK_PaintProducts_Orders_OrderId",
                table: "PaintProducts");

            migrationBuilder.DropIndex(
                name: "IX_PaintProducts_OrderId",
                table: "PaintProducts");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "PaintProducts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderPaintProduct");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "PaintProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaintProducts_OrderId",
                table: "PaintProducts",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaintProducts_Orders_OrderId",
                table: "PaintProducts",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId");
        }
    }
}
