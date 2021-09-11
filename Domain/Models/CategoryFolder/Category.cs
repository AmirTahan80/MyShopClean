using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Category
    {
        public Category()
        {
            Children = new Collection<Category>();
        }
           
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Detail { get; set; }
        public int? ParentId { get; set; }

        // NavigationBars
        public virtual Category Parent  { get; set; }
        public virtual ICollection<Category> Children { get; set; }
        public ICollection<CategoryToProduct> Products { get; set; }
    }
}
