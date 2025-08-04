using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using static Schedulefy.OCommon.ValidationConstants.Event;

namespace Schedulefy.Data.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(EventNameMaxLength, MinimumLength = EventNameMinLength)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(EventDescriptionMaxLength, MinimumLength = EventDescriptionMinLength)]
        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public DateTime PublishedOn { get; set; }

        public bool IsPremium { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        [ForeignKey(nameof(Publisher))]
        public string PublisherId { get; set; } = null!;

        public virtual IdentityUser Publisher { get; set; } = null!;

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; } = null!;

        [ForeignKey(nameof(Ticket))]
        public int? TicketId { get; set; }

        public virtual Ticket? Ticket { get; set; } 

        public virtual IEnumerable<UserEvent> UsersEvents { get; set; } =
            new List<UserEvent>();
    }
}
