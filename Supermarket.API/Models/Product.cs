﻿using Supermarket.API.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Supermarket.API.Models
{
    [Table("Products")]
    public class Product : IComparable<Product>, IEquatable<Product>, IValidatableObject
    {
        [Key]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [FirstLetterUpper]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "Name must be between 5 and 80 characters")]
        public string Name { get; set; }
        [Required]
        [StringLength(300, ErrorMessage = "Description must be at most {1} characters")]
        public string Description { get; set; }
        [Required]
        [Range(1, 10000, ErrorMessage = "Price must be between {1:C} and {2:C}")]
        public decimal Price { get; set; }
        [Required]
        [StringLength(300, MinimumLength = 10)]
        public string ImageUrl { get; set; }
        public double UnitsInStock { get; set; }
        public DateTime RecordDate { get; set; }
        public int CategoryId { get; set; }


        public int CompareTo(Product other)
        {
            return ProductId.CompareTo(other.CategoryId);
        }

        public bool Equals(Product other)
        {
            return ProductId.Equals(other.ProductId);
        }

        // complex validation between different properties
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            const double stockValueLimit = 1_000_000.0;
            if (Convert.ToDouble(this.Price) * this.UnitsInStock > stockValueLimit)
            {
                yield return new ValidationResult($"Invalid stock value (UnitsInStock * Price), It must be less than {stockValueLimit:C}",
                    new string[] { nameof(this.UnitsInStock), nameof(this.Price) });
            }

            const decimal minimumPriceFoodTakeout = 10m;
            if (this.CategoryId == 3 && this.Price < minimumPriceFoodTakeout)
            {
                yield return new ValidationResult($"Invalid Price, minimum price of this category is {minimumPriceFoodTakeout:C} ",
                    new string[] { nameof(this.Price) });
            }
        }
    }
}
