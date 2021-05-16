using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supermarket.API.Context;
using Supermarket.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Supermarket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            try
            {
                return _context.Products.AsNoTracking().ToList();
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpGet("{id}", Name = "ProductsGetById")]
        public ActionResult<Product> GetById(int id)
        {
            try
            {
                var product = _context.Products.AsNoTracking().FirstOrDefault(p => p.ProductId == id);
                if (product == null)
                {
                    return NotFound($"ProductId '{id}' not found");
                }
                return product;
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] Product product)
        {
            try
            {
                // since AspNetCore 2.1 not necessary, It's always checeked in methods in classes marked with [ApiController]
                //if (!ModelState.IsValid) { return BadRequest(ModelState); }
                _context.Products.Add(product);
                _context.SaveChanges();
                return new CreatedAtRouteResult("ProductsGetById", new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Product product)
        {
            try
            {
                if (id != product.ProductId)
                {
                    return BadRequest($"Body id: '{product.ProductId}' and request url id '{id}' doesn't match");
                }
                _context.Entry(product).State = EntityState.Modified;
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<Product> Delete(int id)
        {
            try
            {
                //var product = _context.Products.FirstOrDefault(p => p.ProductId == id); // First: always request the database
                var product = _context.Products.Find(id);   // Find: search for the entity in the context at first, then request database

                if (product == null)
                {
                    return NotFound();
                }
                _context.Products.Attach(product);
                _context.Products.Remove(product);
                _context.SaveChanges();
                return product;
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }
        private ActionResult DefaultErrorMessage(Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.GetType()}: error on {this.GetType().Name} - {MethodBase.GetCurrentMethod().Name}");
        }
    }
}
