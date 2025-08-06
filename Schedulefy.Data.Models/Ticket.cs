using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Schedulefy.Data.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public decimal PricePerTicket { get; set; }


  


    }
}
