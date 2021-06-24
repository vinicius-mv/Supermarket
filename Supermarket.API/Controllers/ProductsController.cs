using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Supermarket.API.Filters;
using Supermarket.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using Supermarket.API.Repository;
using AutoMapper;
using Supermarket.API.Dtos;
using Supermarket.API.Pagination;
using Microsoft.AspNetCore.Authorization;

namespace Supermarket.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductsController> _logger;
        private readonly IMapper _mapper;

        public ProductsController(IUnitOfWork unitOfWork, ILogger<ProductsController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Get([FromQuery] PaginationParameters parameters)
        {
            try
            {
                var products = await _unitOfWork.ProductRepository.GetProducts(parameters);

                var paginationHeader = JsonSerializer.Serialize(products.GetMetadata());
                Response.Headers.Add("X-Pagination", paginationHeader);

                return _mapper.Map<List<ProductDto>>(products);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpGet("{id}", Name = "ProductsGetById")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.GetBy(p => p.ProductId == id);
                if (product == null)
                {
                    _logger.LogInformation($"{DateTime.Now}: NotFound '{id}'");
                    return NotFound($"ProductId '{id}' not found");
                }
                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpGet("OrderByPrice")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetOrderedByPrice()
        {
            var products = await _unitOfWork.ProductRepository.GetProductsByPrice();
            return Ok(_mapper.Map<List<ProductDto>>(products));
        }

        [HttpPost]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult> Post([FromBody] ProductDto productDto)
        {
            try
            {
                // since AspNetCore 2.1 not necessary, It's always checeked in methods in classes marked with [ApiController]
                //if (!ModelState.IsValid) { return BadRequest(ModelState); }
                var product = _mapper.Map<Product>(productDto);
                _unitOfWork.ProductRepository.Add(product);
                await _unitOfWork.CommitAsync();
                return new CreatedAtRouteResult("ProductsGetById", new { id = productDto.ProductId }, productDto);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult> Put(int id, [FromBody] ProductDto productDto)
        {
            try
            {
                if (id != productDto.ProductId)
                {
                    _logger.LogInformation($"{DateTime.Now}: BadRequest '{id}', '{JsonSerializer.Serialize(productDto)}'");
                    return BadRequest($"Body id: '{productDto.ProductId}' and request url id '{id}' doesn't match");
                }

                var product = _mapper.Map<Product>(productDto);
                _unitOfWork.ProductRepository.Update(product);
                await _unitOfWork.CommitAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<ProductDto>> Delete(int id)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.GetBy(p => p.ProductId == id);

                if (product == null)
                {
                    _logger.LogInformation($"{DateTime.Now}: NotFound '{id}'");
                    return NotFound();
                }
                _unitOfWork.ProductRepository.Delete(product);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<ProductDto>(product);
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
