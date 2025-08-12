using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schedulefy.Data.Models;
using Schedulefy.ViewModels.Tickets;

namespace Schedulefy.Services.Core.Contracts
{
    public interface ITicketService
    {
        Task<Ticket?> BuyTicketAsync(BuyTicketRequest request, string userId);
        Task<IEnumerable<IndexTicketViewModel>> LoadAllUserTickets(string userId);
        Task<BuyTicketViewModel?> LoadTicketInfoAsync(int eventId, string userId);
    }
}
