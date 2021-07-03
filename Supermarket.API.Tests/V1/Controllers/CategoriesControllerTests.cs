using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Supermarket.API.Helpers.Pagination;
using Supermarket.API.Repository;
using Supermarket.API.V1.Controllers;
using Supermarket.API.V1.Dtos;
using Supermarket.API.V1.Mappings;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Supermarket.API.Tests.V1.Controllers
{
    [TestClass]
    public class CategoriesControllersTests
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
        public async Task GetCategories_Returns_Categories()
        {
            // Arrange
            var fakeLogger = new Mock<ILogger<CategoriesController>>();
            var controller = new CategoriesController(_uow, fakeLogger.Object, _mapper);
            var pagination = new PaginationParameters() { PageNumber = 1, PageSize = 3 };

            // Act
            var response = await controller.Get(pagination);
            List<CategoryDto> result = (List<CategoryDto>)response.Value;

            // Assert
            Assert.AreEqual(result.Count, 3);
            Assert.AreEqual("Beverages", result[0].Name);
            Assert.AreEqual("images/beverages.jpg", result[0].ImageUrl);
        }

        [TestMethod]
        public async Task GetCategory_Returns_Category()
        {
            // Arrange
            var fakeLogger = new Mock<ILogger<CategoriesController>>();
            var controller = new CategoriesController(_uow, fakeLogger.Object, _mapper);

            // Act
            var response = await controller.GetById(3);
            var result = response.Value;

            // Assert
            Assert.AreEqual("Groceries", result.Name);
            Assert.AreEqual("images/groceries.jpg", result.ImageUrl);
        }

        [TestMethod]
        public async Task GetCategory_Returns_NotFound()
        {
            // Arrange
            var fakeLogger = new Mock<ILogger<CategoriesController>>();
            var controller = new CategoriesController(_uow, fakeLogger.Object, _mapper);

            // Act
            var response = await controller.GetById(9972);

            // Assert
            Assert.IsTrue("Microsoft.AspNetCore.Mvc.NotFoundObjectResult" == response.Result.ToString());
        }

        [TestMethod]
        public async Task PostCategory_Returns_UpdatedCategory()
        {
            // Arrange
            var fakeLogger = new Mock<ILogger<CategoriesController>>();
            var controller = new CategoriesController(_uow, fakeLogger.Object, _mapper);
            var category = new CategoryDto() { Name = "Computers", ImageUrl = "images/computers.jpg" };

            // Act
            dynamic response = await controller.Post(category);
            CategoryDto result = response.Result.Value;

            // Assert;
            Assert.AreEqual("Computers", result.Name);
            Assert.AreEqual("images/computers.jpg", result.ImageUrl);
        }

        [TestMethod]
        public async Task PutCategory_Returns_BadRequestErrorMessage()
        {
            // Arrange
            var fakeLogger = new Mock<ILogger<CategoriesController>>();
            var controller = new CategoriesController(_uow, fakeLogger.Object, _mapper);
            var category = new CategoryDto() { Name = "Electronics", ImageUrl = "images/electronics.jpg" };

            // Act
            var response = (BadRequestObjectResult) await controller.Put(9490, category);
            var responseMessage = response.Value.ToString().ToLower();

            // Assert
            Assert.IsTrue(responseMessage.Contains("doesn't match"));
        }

        [TestMethod]
        public async Task PutCategory_Returns_OkResultMessage()
        {
            // Arrange
            var fakeLogger = new Mock<ILogger<CategoriesController>>();
            var controller = new CategoriesController(_uow, fakeLogger.Object, _mapper);
            var category = new CategoryDto() { CategoryId = 3, Name = "Electronics", ImageUrl = "images/electronics.jpg" };

            // Act
            dynamic response = await controller.Put(3, category);

            // Assert
            Assert.AreEqual($"Category {category.Name} was updated.", response.Value.ToString());
        }
    }
}
