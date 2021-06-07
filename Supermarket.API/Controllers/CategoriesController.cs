using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supermarket.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Supermarket.API.ResourceModels;
using Microsoft.Extensions.Logging;
using Supermarket.API.Filters;
using Supermarket.API.Repository;

namespace Supermarket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(IUnitOfWork unitOfWork, ILogger<CategoriesController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<Category>>> Get()
        {
            try
            {
                return await _unitOfWork.CategoryRepository.Get().ToListAsync();
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpGet("Products")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<CategoryProduct>>> GetWithProducts()
        {
            try
            {
                var categoriesWithProducts = await _unitOfWork.CategoryRepository.GetCategoriesWithProducts();

                return Ok(categoriesWithProducts);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpGet("{id}", Name = "CategoriesGetById")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByFilter(c => c.CategoryId == id);
                if (category == null)
                {
                    _logger.LogInformation($"{DateTime.Now}: NotFound '{id}'");
                    return NotFound($"CategoryId '{id}' not found");
                }
                return category;
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<Category>> Post([FromBody] Category category)
        {
            try
            {
                _unitOfWork.CategoryRepository.Add(category);
                await _unitOfWork.CommitAsync();

                return new CreatedAtRouteResult("CategoriesGetById", new { id = category.CategoryId }, category);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult> Put(int id, [FromBody] Category category)
        {
            try
            {
                if (id != category.CategoryId)
                {
                    _logger.LogInformation($"{DateTime.Now}: BadRequest '{id}', '{JsonSerializer.Serialize(category)}'");
                    return BadRequest($"Body id: '{category.CategoryId}' and request url id '{id}' doesn't match");
                }
                _unitOfWork.CategoryRepository.Add(category);
                await _unitOfWork.CommitAsync();
                return Ok($"Category {category.Name} was updated.");
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<Category>> Delete(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByFilter(c => c.CategoryId == id);
                if (category == null)
                {
                    _logger.LogInformation($"{DateTime.Now}: NotFound '{id}'");
                    return NotFound();
                }
                _unitOfWork.CategoryRepository.Delete(category);
                await _unitOfWork.CommitAsync();
                return Ok(category);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        private ActionResult DefaultErrorMessage(Exception ex, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            _logger.LogError($"{ex.StackTrace}");
            _logger.LogError($"{DateTime.Now}: {ex.GetType()} - {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.GetType()}: error on {this.GetType().Name} - {memberName}");
        }
    }
}
