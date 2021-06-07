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

namespace Supermarket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IUnitOfWork unitOfWork, ILogger<ProductsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            try
            {
                return await _unitOfWork.ProductRepository.Get().ToListAsync();
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpGet("{id}", Name = "ProductsGetById")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.GetByFilter(p => p.ProductId == id);
                if (product == null)
                {
                    _logger.LogInformation($"{DateTime.Now}: NotFound '{id}'");
                    return NotFound($"ProductId '{id}' not found");
                }
                return product;
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpGet("OrderByPrice")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<Product>>> GetOrderedByPrice()
        {
            var products = await _unitOfWork.ProductRepository.GetProductsOrderedByPrice();
            return Ok(products);
        }

        [HttpPost]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult> Post([FromBody] Product product)
        {
            try
            {
                // since AspNetCore 2.1 not necessary, It's always checeked in methods in classes marked with [ApiController]
                //if (!ModelState.IsValid) { return BadRequest(ModelState); }
                _unitOfWork.ProductRepository.Add(product);
                await _unitOfWork.CommitAsync();
                return new CreatedAtRouteResult("ProductsGetById", new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult> Put(int id, [FromBody] Product product)
        {
            try
            {
                if (id != product.ProductId)
                {
                    _logger.LogInformation($"{DateTime.Now}: BadRequest '{id}', '{JsonSerializer.Serialize(product)}'");
                    return BadRequest($"Body id: '{product.ProductId}' and request url id '{id}' doesn't match");
                }
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
        public async Task<ActionResult<Product>> Delete(int id)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.GetByFilter(p => p.ProductId == id);

                if (product == null)
                {
                    _logger.LogInformation($"{DateTime.Now}: NotFound '{id}'");
                    return NotFound();
                }
                _unitOfWork.ProductRepository.Delete(product);
                await _unitOfWork.CommitAsync();
                return product;
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
