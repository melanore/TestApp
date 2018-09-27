using Microsoft.EntityFrameworkCore.Migrations;

namespace TestApp.Data.Migrations
{
    public partial class Customer_Change_PK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Customers_CustomerId",
                table: "Addresses");
            
            migrationBuilder.DropUniqueConstraint(
                name: "IX_CustomerId",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Name",
                table: "Customers",
                column: "Name");
            
            migrationBuilder.AddForeignKey(
                "FK_Addresses_Customers_CustomerId",
                "Addresses",
                "CustomerId",
                "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Customers_CustomerId",
                table: "Addresses");
            
            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Name",
                table: "Customers");

            migrationBuilder.AddUniqueConstraint(
                name: "IX_CustomerId",
                table: "Customers",
                column: "CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                columns: new[] { "CustomerId", "Name" });
            
            migrationBuilder.AddForeignKey(
                "FK_Addresses_Customers_CustomerId",
                "Addresses",
                "CustomerId",
                "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
