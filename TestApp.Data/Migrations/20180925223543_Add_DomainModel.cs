using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestApp.Data.Migrations
{
    public partial class Add_DomainModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID('dbo.CustomerNumber') IS NOT NULL
  DROP FUNCTION CustomerNumber
GO
CREATE FUNCTION [dbo].[CustomerNumber](@i INT) 
RETURNS CHAR(5) WITH SCHEMABINDING
AS 
BEGIN 
    RETURN (CHAR(@i / 26000 % 26 + 65) + 
    CHAR(@i / 1000 % 26 + 65) + 
    CHAR(@i / 100 % 10 + 48) + 
    CHAR(@i / 10 % 10 + 48) + 
    CHAR(@i % 10 + 48)) 
END");
            
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false, computedColumnSql: "CAST(ISNULL([dbo].[CustomerNumber](dbID), 'AA000') AS varchar(5)) PERSISTED"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ZIP = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: true),
                    dbID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => new { x.CustomerId, x.Name });
                    table.UniqueConstraint("IX_CustomerId", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Street = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ZIP = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: true),
                    AddressType = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false),
                    CustomerId = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => new { x.CustomerId, x.AddressType });
                    table.ForeignKey(
                        name: "FK_Addresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Customers");
            
            migrationBuilder.Sql(@"IF OBJECT_ID('dbo.CustomerNumber') IS NOT NULL DROP FUNCTION CustomerNumber");
        }
    }
}
