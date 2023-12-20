using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        [DisplayName("Display Order")]
        [Range(1,10, ErrorMessage ="Display order must between 1 & 10")]
        public int DisplayOrder { get; set; }

        public DateTimeOffset CreatedDateTime { get; set; } = DateTimeOffset.UtcNow;

    }
}

