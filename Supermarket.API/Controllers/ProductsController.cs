using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Supermarket.API.Context;
using Supermarket.API.Filters;
using Supermarket.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.Json;

namespace Supermarket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(AppDbContext context, ILogger<ProductsController> logger)
        {
           _context = context;
           _logger = logger;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            try
            {

                return await _context.Products.AsNoTracking().ToListAsync();
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
                var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
                if (product == null)
                {
                    _logger.LogInformation($"{DateTime.Now}: NotFound '{id}'");
                    return NotFound($"ProductId '{id}' not found");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult> Post([FromBody] Product product)
        {
            try
            {
                // since AspNetCore 2.1 not necessary, It's always checeked in methods in classes marked with [ApiController]
                //if (!ModelState.IsValid) { return BadRequest(ModelState); }
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
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
                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
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
                //var product = _context.Products.FirstOrDefault(p => p.ProductId == id); // First: always request the database
                var product = _context.Products.Find(id);   // Find: search for the entity in the context at first, then request database

                if (product == null)
                {
                    _logger.LogInformation($"{DateTime.Now}: NotFound '{id}'");
                    return NotFound();
                }
                _context.Products.Attach(product);
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
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
