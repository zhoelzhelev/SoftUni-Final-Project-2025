using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedulefy.Data.Models
{
    public class BuyTicketRequest
    {
        public int EventId { get; set; }
        public int Quantity { get; set; }
        public decimal  Price { get; set; }
    }
}
