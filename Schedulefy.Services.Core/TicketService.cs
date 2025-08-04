using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Schedulefy.Data;
using Schedulefy.Data.Models;
using Schedulefy.Services.Core.Contracts;
using Schedulefy.ViewModels.Tickets;

namespace Schedulefy.Services.Core
{
    public class TicketService : ITicketService
    {
        private readonly SchedulefyDbContext _context;

        public TicketService(SchedulefyDbContext context)
        {
            this._context = context;
        }

        public async Task<Ticket?> BuyTicketAsync(BuyTicketRequest request, string userId)
        {

            Ticket? ticket = null;

            Event? entity = await this._context
                .Events
                .Include(e => e.Ticket)
                .FirstOrDefaultAsync(e => e.Id == request.EventId);

            if(entity is not null)
            {
                ticket = entity.Ticket;
                IdentityUser? user = await this._context
               .Users
               .FirstOrDefaultAsync(u => u.Id.ToLower() == userId.ToLower());


                if (ticket is not null && user is not null)
                {
                    UserTicket userTicket = new UserTicket()
                    {
                        User = user,
                        UserId = userId,
                        Ticket = ticket,
                        TicketId = ticket.Id,
                        TickeetsCount = request.Quantity
                    };


                    await this._context.UsersTickets.AddAsync(userTicket);
                    await this._context.SaveChangesAsync();

                }
            }

           

            return ticket;
        }

        public async Task<IEnumerable<IndexTicketViewModel>> LoadAllUserTickets(string userId)
        {
           


            IEnumerable<IndexTicketViewModel> tickets = await this._context
                .UsersTickets
                .Include(ut => ut.Ticket)
                .Where(ut => ut.UserId.ToLower() == userId.ToLower())
                .Select(ut => new IndexTicketViewModel
                {
                    Id = ut.TicketId,
                    Count = ut.TickeetsCount,
                    EventImageUrl = this._context
                    .Events
                    .Where(e => e.TicketId == ut.TicketId)
                    .Select(e => e.ImageUrl)
                    .First()
                    .ToString(),
                    EventTitleName = this._context
                    .Events
                    .Where(e => e.TicketId == ut.TicketId)
                    .Select(e => e.Name)
                    .First()
                    .ToString(),
                    Price = ut.TickeetsCount * this._context
                    .Events
                    .Where(e => e.TicketId == ut.TicketId)
                    .Include(e => e.Ticket)
                    .Select(e => e.Ticket!.PricePerTicket)
                    .First()
                })
                .ToArrayAsync();

            return tickets;
        }

        public async Task<BuyTicketViewModel?> LoadTicketInfoAsync(int eventId, string userId)
        {
            BuyTicketViewModel? viewModel = null;

            Event? entity = await this._context
                .Events
                .Include(e => e.Ticket)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (entity is not null && entity.PublisherId.ToLower() != userId.ToLower())
            {
                viewModel = new BuyTicketViewModel()
                {
                    EventId = eventId,
                    EventImageUrl = entity.ImageUrl,
                    EventTitle = entity.Name,
                    PricePerTicket = entity.Ticket!.PricePerTicket
                };
            }

            return viewModel;
        }
    }
}
