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
using Microsoft.AspNetCore.Http;
using System.Reflection;

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
        public ActionResult<IEnumerable<Category>> Get()
        {
            try
            {
                return _context.Categories.AsNoTracking().ToList();
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpGet("Products")]
        public ActionResult<IEnumerable<object>> GetWithProducts()
        {
            try
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
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpGet("{id}", Name = "CategoriesGetById")]
        public ActionResult<Category> GetById(int id)
        {
            try
            {
                var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
                if (category == null)
                {
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
        public ActionResult<Category> Post([FromBody] Category category)
        {
            try
            {
                _context.Categories.Add(category);
                _context.SaveChanges();

                return new CreatedAtRouteResult("CategoriesGetById", new { id = category.CategoryId }, category);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage (ex);
            }
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Category category)
        {
            try
            {
                if (id != category.CategoryId)
                {
                    return BadRequest($"Body id: '{category.CategoryId}' and request url id '{id}' doesn't match");
                }
                _context.Entry(category).State = EntityState.Modified;
                _context.SaveChanges();
                return Ok($"Category {category.Name} was updated.");
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }



        [HttpDelete("{id}")]
        public ActionResult<Category> Delete(int id)
        {
            try
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
