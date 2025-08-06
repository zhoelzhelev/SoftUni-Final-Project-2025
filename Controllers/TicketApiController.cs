using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedulefy.Data.Models;
using Schedulefy.Services.Core.Contracts;

namespace Schedulefy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketApiController : BaseController
    {
        private readonly ITicketService _ticketService;

        public TicketApiController(ITicketService ticketService)
        {
            this._ticketService = ticketService;
        }

        [HttpPost("buy")]
        public async Task<IActionResult> Buy([FromForm] BuyTicketRequest request)
        {

            try
            {
                if (User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                string userId = this.GetUserId()!;

                Ticket? ticket = await this._ticketService
                    .BuyTicketAsync(request, userId);

                if (ticket is null)
                {
                    return BadRequest("Invalid operation");
                }

                return this.RedirectToAction("Index", "Ticket");
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
                return BadRequest("Invalid operation");
            }
            
        }
    }
}
