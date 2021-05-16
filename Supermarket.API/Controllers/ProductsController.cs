using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supermarket.API.Context;
using Supermarket.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult<IEnumerable<Product>> GetAll()
        {
            var products = _context.Products.AsNoTracking().ToList();
            return products;
        }

        [HttpGet("{id}", Name = "GetProduct")]
        public ActionResult<Product> GetById(int id)
        {
            var product = _context.Products.AsNoTracking().FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost]
        public ActionResult Post([FromBody]Product product)
        {
            // since AspNetCore 2.1 not necessary, It's always checeked in methods in classes marked with [ApiController]
            //if (!ModelState.IsValid) { return BadRequest(ModelState); }

            _context.Products.Add(product);
            _context.SaveChanges();
            return new CreatedAtRouteResult("GetProduct", new { id = product.ProductId }, product);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody]Product product)
        {
            if(id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<Product> Delete(int id)
        {
            //var product = _context.Products.FirstOrDefault(p => p.ProductId == id); // First: always request the database
            var product = _context.Products.Find(id);   // Find: search for the entity in the context at first, then request database
            
            if(product == null)
            {
                return NotFound();
            }
            _context.Products.Attach(product);
            _context.Products.Remove(product);
            _context.SaveChanges();
            return product;
        }
    }
}
