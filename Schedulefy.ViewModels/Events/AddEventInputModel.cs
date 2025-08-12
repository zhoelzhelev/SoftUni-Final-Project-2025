using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Schedulefy.OCommon.ValidationConstants.Event;

namespace Schedulefy.ViewModels.Events
{
    public class AddEventInputModel
    {
        [Required]
        [StringLength(EventNameMaxLength, MinimumLength = EventNameMinLength)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(EventDescriptionMaxLength, MinimumLength = EventDescriptionMinLength)]
        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public decimal TicketPrice { get; set; }

        public string PublishedOn { get; set; } = null!;

        public int CategoryId { get; set; }

        public virtual IEnumerable<AddCategoriesDropDownMenu> Categories { get; set; } = 
            new List<AddCategoriesDropDownMenu>();
    }
}
