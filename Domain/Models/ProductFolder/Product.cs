using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Models;

namespace Domain.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
        [Required]
        public string Detail { get; set; }
        [Required]
        public int Count { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public DateTime InsertTime{ get; set; }
        [Required]
        public int CategoryId { get; set; }

        // NavigationBar       
        public Category Category { get; set; }
        public ICollection<ProductImages> ProductImages { get; set; }
        public ICollection<ProductProperty> Properties { get; set; }
        public Adjective Adjective { get; set; }
    }
}