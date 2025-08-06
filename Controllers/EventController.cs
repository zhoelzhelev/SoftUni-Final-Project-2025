using Microsoft.AspNetCore.Mvc;
using Schedulefy.Controllers;
using Schedulefy.ViewModels;
using Schedulefy.ViewModels.Events;
using Schedulefy.Services.Core;
using Schedulefy.Services.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using static Schedulefy.OCommon.ValidationConstants.Event;


namespace Schedulefy.Controllers
{
    public class EventController : BaseController
    {
        private readonly IEventService _eventService;
        private readonly ICategoryService _categoryService;

        public EventController(IEventService eventService, ICategoryService categoryService)
        {
            this._eventService = eventService;
            this._categoryService = categoryService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {

            try
            {
                string? userId = this.GetUserId();

                IEnumerable<IndexEventViewModel> viewModel = await this._eventService
                    .GetAllEventsAsync(userId);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return this.RedirectToAction(nameof(Index));
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                string? userId = this.GetUserId();

                DetailsEventViewModel? viewModel = await this._eventService
                    .GetEventDetailsAsync(id, userId);

                if (viewModel is null)
                {
                    return this.RedirectToAction(nameof(Index));
                }

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return this.RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            AddEventInputModel model = new AddEventInputModel()
            {
                PublishedOn = DateTime.UtcNow.ToString(PublishedOnCorrectFormat),
                Categories = await this._categoryService
                .GetAllCategoriesAsync()
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddEventInputModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Categories = await this._categoryService
                        .GetAllCategoriesAsync();

                    return this.View(model);
                }

                string userId = this.GetUserId()!;

                bool result = await this._eventService
                    .AddEventAsync(model, userId);

                if (!result)
                {
                    model.Categories = await this._categoryService
                       .GetAllCategoriesAsync();

                    return this.View(model);
                }

                return this.RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return this.View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                string userId = this.GetUserId()!;

                EditEventInputModel? model = await this._eventService
                    .GetEventForEditingAsync(id, userId);

                if (model is null)
                {
                    return this.RedirectToAction(nameof(Index));
                }
                model.Categories = await this._categoryService
                    .GetAllCategoriesAsync();

                return this.View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return this.RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditEventInputModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Categories = await this._categoryService
                    .GetAllCategoriesAsync();
                    return this.View(model);
                }

                string userId = this.GetUserId()!;

                bool result = await this._eventService
                    .PersistUpdatedInformationAsync(model, userId);

                if (!result)
                {
                    model.Categories = await this._categoryService
                    .GetAllCategoriesAsync();
                    return this.View(model);
                }

                model.Categories = await this._categoryService
                    .GetAllCategoriesAsync();

                return this.RedirectToAction(nameof(Details), model.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return this.View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                string userId = this.GetUserId()!;

                DeleteEventInputModel? model = await this._eventService
                    .GetEventForDeletingAsync(id, userId);

                if (model is null)
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

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteEventInputModel? model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.View(model);
                }

                string userId = this.GetUserId()!;

                bool resutl = await this._eventService
                    .DeleteEntityAsync(model, userId);

                if (!resutl)
                {
                    return this.View(model);
                }

                return this.RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return this.View(model);
            }
        }

        
    }
}
