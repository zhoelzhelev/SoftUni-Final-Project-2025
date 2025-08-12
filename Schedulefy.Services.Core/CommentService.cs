using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Schedulefy.Data;
using Schedulefy.Data.Models;
using Schedulefy.Services.Core.Contracts;
using Schedulefy.ViewModels.Comments;

namespace Schedulefy.Services.Core
{
    public class CommentService : ICommentService
    {
        private readonly SchedulefyDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentService(SchedulefyDbContext context, UserManager<IdentityUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        public async Task<bool> AddCommentAsync(AddCommentInputModel model, string userId)
        {
            bool result = false;
            IdentityUser? user = await this._userManager.FindByIdAsync(userId);
            Event? entity = await this._context
                .Events             
                .FirstOrDefaultAsync(e => e.Id == model.EventId);
            if(user is not null && entity is not null)
            {
                Comment comment = new Comment() 
                {
                    UserId = userId,
                    User = user,
                    Content = model.Content,
                    EventId = model.EventId,
                    Event = entity,
                };

                await this._context.Comments.AddAsync(comment);
                await this._context.SaveChangesAsync();

                result = true;
            }

            return result;
        }

        public async Task<IEnumerable<ComentIndexViewModel>> LoadAllCommentsForCurrentEvent(int eventId)
        {
            IEnumerable<ComentIndexViewModel> model = await this._context
                .Comments
                .Where(c => c.EventId == eventId)
                .Include(c => c.Event)
                .Include(c => c.User)
                .Select(c => new ComentIndexViewModel
                {
                    EventId = eventId,
                    Id = c.Id,
                    EventName = c.Event.Name,
                    PublisherUserName = c.User!.NormalizedUserName!.ToLower(),
                    Content = c.Content,
                })
                .ToArrayAsync();

            return model;
        }
    }
}
