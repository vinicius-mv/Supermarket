using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supermarket.API.Context;
using Supermarket.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Supermarket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetAll()
        {
            return _context.Categories.AsNoTracking().ToList();
        }

        [HttpGet("products")]
        public ActionResult<IEnumerable<object>> GetAllWithProducts()
        {
            var categories = _context.Categories.AsNoTracking().ToList();
            var products = _context.Products.AsNoTracking().ToList();

            //var categoriesProducts = from c in categories
            //                         join p in products on c.CategoryId equals p.CategoryId into g
            //                         select new { c.CategoryId, c.Name, c.ImageUrl, Products = g.ToList() };

            var categoriesProducts = categories.GroupJoin(products, c => c.CategoryId, p => p.CategoryId, (c, ps) => 
            new { c.CategoryId, c.Name, c.ImageUrl, Products = ps.ToList() });

            return categoriesProducts.ToList();
        }

        [HttpGet("{id}", Name = "GetCategory")]
        public ActionResult<Category> GetById(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }
            return category;
        }

        [HttpPost]
        public ActionResult<Category> Post([FromBody] Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();

            return new CreatedAtRouteResult("GetCategory", new { id = category.CategoryId }, category);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }
            _context.Entry(category).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<Category> Delete(int id)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            _context.Attach(category);
            _context.Remove(category);
            _context.SaveChanges();
            return category;
        }

    }
}
