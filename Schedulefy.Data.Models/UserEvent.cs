using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Schedulefy.Data.Models
{
    public class UserEvent
    {
        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        public virtual IdentityUser User { get; set; } = null!;

        [ForeignKey(nameof(Event))]
        public int EventId { get; set; }

        public virtual Event Event { get; set; } = null!;
    }
}