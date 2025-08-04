using System.ComponentModel.DataAnnotations;
using static Schedulefy.OCommon.ValidationConstants.Category;

namespace Schedulefy.Data.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength)]
        public string Name { get; set; } = null!;

        public virtual IEnumerable<Event> Events { get; set; } = 
            new List<Event>();
    }
}