using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Supermarket.API.Helpers.Pagination;
using Supermarket.API.Models;
using Supermarket.API.Repository;
using Supermarket.API.V1.Controllers;
using Supermarket.API.V1.Dtos;
using Supermarket.API.V1.Mappings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Supermarket.API.Tests.V1.Controllers
{
    [TestClass]
    public class ProductsControllerTests
    {
        private IUnitOfWork _uow;
        private IMapper _mapper;

        [TestInitialize]
        public void SetUp()
        {
            _uow = new ContextInitializer().UoW;
            _mapper = CreateMapper();
        }

        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));

            return config.CreateMapper();
        }

        [TestMethod]
        public async Task GetProducts_Returns_Products()
        {
            // Arrange
            var fakeLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(_uow, fakeLogger.Object, _mapper);
            var pagination = new PaginationParameters() { PageNumber = 1, PageSize = 3 };

            // Act
            var response = await controller.Get(pagination);
            List<ProductDto> result = (List<ProductDto>)response.Value;

            // Assert
            Assert.AreEqual(result.Count, 3);
            Assert.AreEqual("Diet-Coke", result[0].Name);
            Assert.AreEqual("Diet-Coke 350 ml", result[0].Description);
            Assert.AreEqual(1.3m, result[0].Price);
            Assert.AreEqual(1, result[0].CategoryId);
        }

        [TestMethod]
        public async Task GetProduct_Returns_Product()
        {
            // Arrange
            var fakeLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(_uow, fakeLogger.Object, _mapper);

            // Act
            var response = await controller.GetById(2);
            var result = response.Value;

            // Assert
            Assert.AreEqual("Sandwich", result.Name);
            Assert.AreEqual("Sandwich 300 g", result.Description);
            Assert.AreEqual(4.5m, result.Price);
            Assert.AreEqual(2, result.CategoryId);
        }

        [TestMethod]
        public async Task GetProduct_Returns_NotFound()
        {
            // Arrange
            var fakeLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(_uow, fakeLogger.Object, _mapper);

            // Act
            var response = await controller.GetById(9972);

            // Assert
            Assert.IsTrue("Microsoft.AspNetCore.Mvc.NotFoundObjectResult" == response.Result.ToString());
        }

        [TestMethod]
        public async Task PostProduct_Returns_UpdatedProduct()
        {
            // Arrange
            var fakeLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(_uow, fakeLogger.Object, _mapper);
            var Product = new ProductDto() { Name = "Cheeseburguer", Description = "Cheeseburguer 450 g", Price = 6.0m, CategoryId = 2 };

            // Act
            dynamic response = await controller.Post(Product);
            ProductDto result = response.Value;

            // Assert;
            Assert.IsTrue("Cheeseburguer" == result.Name);
            Assert.IsTrue("Cheeseburguer 450 g" == result.Description);
            Assert.AreEqual(6m, result.Price);
        }

        [TestMethod]
        public async Task PutProduct_Returns_BadRequestErrorMessage()
        {
            // Arrange
            var fakeLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(_uow, fakeLogger.Object, _mapper);
            var Product = new ProductDto() { ProductId = 9490, Name = "Cheese Bacon", Description = "Chesse Bacon 450 g", Price = 6.0m, CategoryId = 2 };

            // Act
            var response = (BadRequestObjectResult)await controller.Put(5, Product);
            var responseMessage = response.Value.ToString().ToLower();

            // Assert
            Assert.IsTrue(responseMessage.Contains("doesn't match"));
        }

        [TestMethod]
        public async Task PutProduct_Returns_OkResult()
        {
            // Arrange
            var fakeLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(_uow, fakeLogger.Object, _mapper);
            var product = new ProductDto() { ProductId = 4, Name = "Apple Juice", Description = "Apple Juice 350 ml", Price = 1.5m, CategoryId = 1 };

            // Act
            var response = await controller.Put(4, product);

            // Assert
            System.Console.WriteLine();
        }
    }
}
