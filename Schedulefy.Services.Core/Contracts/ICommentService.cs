using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Schedulefy.ViewModels.Comments;

namespace Schedulefy.Services.Core.Contracts
{
    public interface ICommentService
    {
        Task<IEnumerable<ComentIndexViewModel>> LoadAllCommentsForCurrentEvent(int eventId);
        Task<bool> AddCommentAsync(AddCommentInputModel comment, string userId);
    }
}
