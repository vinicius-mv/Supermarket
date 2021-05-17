﻿using Microsoft.AspNetCore.Mvc;
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
using Supermarket.API.ResourceModel;

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
        public async Task<ActionResult<IEnumerable<Category>>> Get()
        {
            try
            {
                return await _context.Categories.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpGet("Products")]
        public async Task<ActionResult<IEnumerable<CategoryProduct>>> GetWithProducts()
        {
            try
            {
                var categories = await _context.Categories.AsNoTracking().ToListAsync();
                var products = await _context.Products.AsNoTracking().ToListAsync();

                //var categoriesProducts = from c in categories
                //                         join p in products on c.CategoryId equals p.CategoryId into g
                //                         select new CategoryProduct
                //                         { CategoryId = c.CategoryId, Name = c.Name, ImageUrl = c.ImageUrl, Products = g.ToHashSet() };

                var categoriesAndProducts = categories.GroupJoin(products, c => c.CategoryId, p => p.CategoryId, (c, ps) =>
                new CategoryProduct()
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    ImageUrl = c.ImageUrl,
                    Products = ps.ToHashSet()
                }).ToList();

                return categoriesAndProducts;
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpGet("{id}", Name = "CategoriesGetById")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
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
        public async Task<ActionResult<Category>> Post([FromBody] Category category)
        {
            try
            {
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                return new CreatedAtRouteResult("CategoriesGetById", new { id = category.CategoryId }, category);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Category category)
        {
            try
            {
                if (id != category.CategoryId)
                {
                    return BadRequest($"Body id: '{category.CategoryId}' and request url id '{id}' doesn't match");
                }
                _context.Entry(category).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok($"Category {category.Name} was updated.");
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }



        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> Delete(int id)
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
                await _context.SaveChangesAsync();
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
