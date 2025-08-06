using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedulefy.Services.Core.Contracts;
using Schedulefy.ViewModels.Events;

namespace Schedulefy.Controllers
{
    public class GoingController : BaseController
    {
        private readonly IEventService _eventService;

        public GoingController(IEventService eventSeervice)
        {
            this._eventService = eventSeervice;
        }


        [HttpGet]
        public async Task<IActionResult> Goings()
        {
            try
            {
                if (User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                string userId = this.GetUserId()!;

                IEnumerable<GoingEventsViewModel> goingEvents = await this._eventService
                    .GetAllGoingEventsAsync(userId);

                return this.View(goingEvents);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return this.RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(int id)
        {
            try
            {
                if (User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                string? userId = this.GetUserId();

                if (userId is null)
                {
                    return this.Redirect(nameof(Index));
                }

                bool result = await this._eventService
                    .AddEventToGoingAsync(id, userId);

                if (!result)
                {
                    return this.RedirectToAction(nameof(Index));
                }

                return this.RedirectToAction(nameof(Goings));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return this.RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                if (User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                string? userId = this.GetUserId();

                if (userId is null)
                {
                    return this.RedirectToAction(nameof(Index));
                }

                bool result = await this._eventService
                    .RemoveEventFromGoingAsync(id, userId);

                if (!result)
                {
                    return this.RedirectToAction(nameof(Index));
                }

                return this.RedirectToAction(nameof(Goings));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return this.RedirectToAction(nameof(Index));
            }
        }
    }
}
