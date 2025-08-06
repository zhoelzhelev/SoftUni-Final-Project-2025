using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Schedulefy.Data.Models
{
    public class UserTicket
    {
        [Key]
        public int UserTicketId { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        public virtual IdentityUser User { get; set; } = null!;

        [ForeignKey(nameof(Ticket))]
        public int TicketId { get; set; }

        public virtual Ticket Ticket { get; set; } = null!;

        public int TickeetsCount { get; set; }
    }
}
