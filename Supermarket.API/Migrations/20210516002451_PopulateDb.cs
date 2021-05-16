using Microsoft.EntityFrameworkCore.Migrations;

namespace Supermarket.API.Migrations
{
    public partial class PopulateDb : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("Insert Into Categories(Name, ImageUrl) Values('Beverages', 'http://wwww.macoratti.net/Imagens/1.jpg')");
            mb.Sql("Insert Into Categories(Name, ImageUrl) Values('Food takeout', 'http://wwww.macoratti.net/Imagens/2.jpg')");
            mb.Sql("Insert Into Categories(Name, ImageUrl) Values('Groceries', 'http://wwww.macoratti.net/Imagens/3.jpg')");

            mb.Sql("Insert Into Products(Name, Description, Price, ImageUrl, UnitsInStock, RecordDate, CategoryId) " +
                "Values('Diet-Coke', 'Diet Cola can - 350ml', '5.45', 'http://wwww.macoratti.net/Imagens/coca.jpg', 50, now(), (Select CategoryId From Categories where Name = 'Beverages'))");

            mb.Sql("Insert Into Products(Name, Description, Price, ImageUrl, UnitsInStock, RecordDate, CategoryId) " +
                "Values('Tuna Sandwich', 'Delicious tuna sandwich', '8.50', 'http://wwww.macoratti.net/Imagens/atum.jpg', 10, now(), (Select CategoryId From Categories where Name = 'Food takeout'))");

            mb.Sql("Insert Into Products(Name, Description, Price, ImageUrl, UnitsInStock, RecordDate, CategoryId) " +
                "Values('Pudding', 'Creamy chocolate pudding', '6.75', 'http://wwww.macoratti.net/Imagens/coca.jpg', 20, now(), (Select CategoryId From Categories where Name = 'Groceries'))");
        }

        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete From Categories");
            mb.Sql("Delete From Products");
        }
    }
}
