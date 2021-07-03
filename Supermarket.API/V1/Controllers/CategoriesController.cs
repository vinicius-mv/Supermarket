using Microsoft.AspNetCore.Mvc;
using Supermarket.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Supermarket.API.Filters;
using Supermarket.API.Repository;
using AutoMapper;
using System.Linq;
using Supermarket.API.Helpers.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Supermarket.API.V1.Dtos;

namespace Supermarket.API.V1.Controllers
{
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoriesController> _logger;
        private readonly IMapper _mapper;

        public CategoriesController(IUnitOfWork unitOfWork, ILogger<CategoriesController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [DisableCors]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        // [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)] replaced by [ApiConventionMethod]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))] repaced by [ApiConvetionType] -> clas attr
        public async Task<ActionResult<IEnumerable<CategoryDto>>> Get([FromQuery] PaginationParameters parameters)
        {
            try
            {
                var categories = await _unitOfWork.CategoryRepository.GetCategories(parameters);

                var paginationHeader = JsonSerializer.Serialize(categories.GetMetadata());
                Response?.Headers.Add("X-Pagination", paginationHeader);

                return _mapper.Map<List<CategoryDto>>(categories);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpGet("CategoryProducts")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<CategoryProductsDto>>> GetWithProducts([FromQuery] PaginationParameters parameters)
        {
            try
            {
                var categoriesProducts = await _unitOfWork.CategoryRepository.GetCategoriesWithProducts(parameters);

                var paginationHeader = JsonSerializer.Serialize(categoriesProducts.GetMetadata());
                Response.Headers.Add("X-Pagination", paginationHeader);

                var categoriesProductsDto = categoriesProducts.Select(cp => cp.ConvertToDto(_mapper));

                return Ok(categoriesProductsDto);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpGet("{id}", Name = "CategoriesGetById")]
        [DisableCors]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetBy(c => c.CategoryId == id);
                if (category == null)
                {
                    _logger.LogInformation($"{DateTime.Now}: NotFound '{id}'");
                    return NotFound($"CategoryId '{id}' not found");
                }
                return _mapper.Map<CategoryDto>(category);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<CategoryDto>> Post([FromBody] CategoryDto categoryDto)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryDto);
                _unitOfWork.CategoryRepository.Add(category);
                await _unitOfWork.CommitAsync();

                return new CreatedAtRouteResult("CategoriesGetById", new { id = category.CategoryId }, categoryDto);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult> Put(int id, [FromBody] CategoryDto categoryDto)
        {
            try
            {
                if (id != categoryDto.CategoryId)
                {
                    _logger.LogInformation($"{DateTime.Now}: BadRequest '{id}', '{JsonSerializer.Serialize(categoryDto)}'");
                    return BadRequest($"Body id: '{categoryDto.CategoryId}' and request url id '{id}' doesn't match");
                }
                var category = _mapper.Map<Category>(categoryDto);
                _unitOfWork.CategoryRepository.Update(category);
                await _unitOfWork.CommitAsync();
                return Ok($"Category {categoryDto.Name} was updated.");
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CategoryDto>> Delete(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetBy(c => c.CategoryId == id);
                if (category == null)
                {
                    _logger.LogInformation($"{DateTime.Now}: NotFound '{id}'");
                    return NotFound();
                }
                _unitOfWork.CategoryRepository.Delete(category);
                await _unitOfWork.CommitAsync();
                return Ok(_mapper.Map<CategoryDto>(category));
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
