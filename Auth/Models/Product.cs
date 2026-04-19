using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Auth.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        public string? ImagePath {  get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    public class Basket
    {
        public Guid id { get; set; }
        public List<BasketItem> Products { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }

    public class BasketItem
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public Guid BasketID { get; set; }
        public Basket Basket { get; set; }
    }
}
