using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Supermarket.API.Models
{
    [Table("Categories")]
    public class Category : IComparable<Category>, IEquatable<Category>
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(80)]
        public string Name { get; set; }
        [Required]
        [MaxLength(300)]
        public string ImageUrl { get; set; }

        public int CompareTo(Category other)
        {
            return CategoryId.CompareTo(other.CategoryId);
        }

        public bool Equals(Category other)
        {
            return CategoryId.Equals(other.CategoryId);
        }
    }
}
