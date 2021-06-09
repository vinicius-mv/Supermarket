﻿using Microsoft.AspNetCore.Mvc;
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
using Supermarket.API.Dtos;
using AutoMapper;

namespace Supermarket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> Get()
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.Get().ToListAsync();

                return _mapper.Map<List<CategoryDto>>(category);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpGet("Products")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<CategoryProductsDto>>> GetWithProducts()
        {
            try
            {
                var categoriesProducts = await _unitOfWork.CategoryRepository.GetCategoriesWithProducts();
                var categoriesProductsDto = _mapper.Map<List<CategoryProductsDto>>(categoriesProducts);

                return Ok(categoriesProductsDto);
            }
            catch (Exception ex)
            {
                return DefaultErrorMessage(ex);
            }
        }

        [HttpGet("{id}", Name = "CategoriesGetById")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByFilter(c => c.CategoryId == id);
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

                return new CreatedAtRouteResult("CategoriesGetById", new { id = categoryDto.CategoryId }, categoryDto);
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
                _unitOfWork.CategoryRepository.Add(category);
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
        public async Task<ActionResult<CategoryDto>> Delete(int id)
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
