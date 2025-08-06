using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedulefy.Services.Core.Contracts;
using Schedulefy.ViewModels.Events;
using Schedulefy.ViewModels.Tickets;

namespace Schedulefy.Controllers
{
    public class TicketController : BaseController
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            this._ticketService = ticketService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                if (User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                string userId = this.GetUserId()!;

                IEnumerable<IndexTicketViewModel> viewModel = await this._ticketService
                    .LoadAllUserTickets(userId);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return this.RedirectToAction(nameof(Index));
            }

        }

        [HttpGet]
        public async Task<IActionResult> Buy(int id)
        {
            try
            {
                if (User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                string userId = this.GetUserId()!;

                BuyTicketViewModel? model = await this._ticketService
                    .LoadTicketInfoAsync(id, userId);

                if(model is null)
                {
                    return this.RedirectToAction(nameof(Index));
                }

                return this.View(model);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
                return this.RedirectToAction(nameof(Index));
            }
        }
    }
}
