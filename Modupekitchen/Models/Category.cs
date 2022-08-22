using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Modupekitchen.Models
{
    public class Category
    {
        [Key]
        [Display(Name = "Category Id")]
        public int Id { get; set; }

        [Display(Name ="Category Name")]
        [Required]
        public string Name { get; set; }
    }
}
