using System.ComponentModel.DataAnnotations;
using static Schedulefy.OCommon.ValidationConstants.Category;

namespace Schedulefy.ViewModels.Events
{
    public class AddCategoriesDropDownMenu
    {
        public int Id { get; set; }

        [Required]
        [StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength)]
        public string Name { get; set; } = null!;
    }
}