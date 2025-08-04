using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedulefy.ViewModels.Tickets
{
    public class BuyTicketViewModel
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; } = null!;
        public string? EventImageUrl { get; set; }
        public decimal PricePerTicket { get; set; }
    }
}
