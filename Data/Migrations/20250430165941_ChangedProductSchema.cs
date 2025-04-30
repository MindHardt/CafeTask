using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedProductSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_order_products_products_product_name",
                table: "order_products");

            migrationBuilder.DropPrimaryKey(
                name: "pk_products",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "pk_order_products",
                table: "order_products");

            migrationBuilder.DropIndex(
                name: "ix_order_products_product_name",
                table: "order_products");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "products",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "product_id",
                table: "order_products",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            
            migrationBuilder.Sql(
                // lang=postgresql
                """
                UPDATE order_products SET product_id = subquery.id FROM (
                    SELECT id, name FROM products
                ) AS subquery
                WHERE order_products.product_name = subquery.name
                """);
            
            migrationBuilder.DropColumn(
                name: "product_name",
                table: "order_products");

            migrationBuilder.AddPrimaryKey(
                name: "pk_products",
                table: "products",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_order_products",
                table: "order_products",
                columns: new[] { "order_id", "product_id" });

            migrationBuilder.CreateIndex(
                name: "ix_products_name",
                table: "products",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_order_products_product_id",
                table: "order_products",
                column: "product_id");

            migrationBuilder.AddForeignKey(
                name: "fk_order_products_products_product_id",
                table: "order_products",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_order_products_products_product_id",
                table: "order_products");

            migrationBuilder.DropPrimaryKey(
                name: "pk_products",
                table: "products");

            migrationBuilder.DropIndex(
                name: "ix_products_name",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "pk_order_products",
                table: "order_products");

            migrationBuilder.DropIndex(
                name: "ix_order_products_product_id",
                table: "order_products");

            migrationBuilder.DropColumn(
                name: "id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "product_id",
                table: "order_products");

            migrationBuilder.AddColumn<string>(
                name: "product_name",
                table: "order_products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "pk_products",
                table: "products",
                column: "name");

            migrationBuilder.AddPrimaryKey(
                name: "pk_order_products",
                table: "order_products",
                columns: new[] { "order_id", "product_name" });

            migrationBuilder.CreateIndex(
                name: "ix_order_products_product_name",
                table: "order_products",
                column: "product_name");

            migrationBuilder.AddForeignKey(
                name: "fk_order_products_products_product_name",
                table: "order_products",
                column: "product_name",
                principalTable: "products",
                principalColumn: "name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
