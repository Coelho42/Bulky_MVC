using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Model
{
    public class Category
    {
        public int Id { get; set; }
        // Data annotations
        [Required]
        [DisplayName("Category Name")]
        [MaxLength(30)]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be between 1-100")]
        public int DisplayOrder { get; set; }
    }
}
