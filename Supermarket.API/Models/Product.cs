using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        [MaxLength(80)]
        public string Name { get; set; }
        [Required]
        [MaxLength(300)]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; }
        public double UnitsInStock { get; set; }
        public DateTime RecordDate { get; set; }

        // ef nav
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
