using Microsoft.AspNetCore.Mvc;
using Schedulefy.Services.Core;
using Schedulefy.Services.Core.Contracts;
using Schedulefy.ViewModels.Comments;

namespace Schedulefy.Controllers
{
    public class CommentController : BaseController
    {
        private readonly ICommentService _commentService;


        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            try
            {
                IEnumerable<ComentIndexViewModel> model = await this._commentService
                    .LoadAllCommentsForCurrentEvent(id);

                ViewData["EventId"] = id;
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return this.RedirectToAction("Details", "Event", id);
            }
        }

        [HttpGet]
        public IActionResult Add(int id)
        {
            try
            {
                AddCommentInputModel model = new AddCommentInputModel() 
                {
                    EventId = id
                };

                return this.View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return this.RedirectToAction("Index", "Event");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCommentInputModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.View(model);
                }

                string? userId = this.GetUserId();

                if (userId is null) 
                {
                    return this.View(model);
                }

                bool result = await this._commentService.AddCommentAsync(model, userId);

                if (!result) 
                {
                    return this.View(model);
                }

                return this.RedirectToAction("Details","Event", model.EventId);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
                return this.RedirectToAction("Index", "Event");
            }
        }
    }
}
