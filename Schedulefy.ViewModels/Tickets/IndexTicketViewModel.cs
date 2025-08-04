using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedulefy.ViewModels.Tickets
{
    public class IndexTicketViewModel
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public string? EventImageUrl { get; set; }
        public string? EventTitleName { get; set; } 
        public decimal Price { get; set; }
    }
}
