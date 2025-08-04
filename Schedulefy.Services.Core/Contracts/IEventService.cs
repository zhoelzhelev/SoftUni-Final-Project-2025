using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Schedulefy.ViewModels.Events;

namespace Schedulefy.Services.Core.Contracts
{
    public interface IEventService
    {
        Task<IEnumerable<IndexEventViewModel>> GetAllEventsAsync(string? userId); 
        Task<DetailsEventViewModel?> GetEventDetailsAsync(int eventId, string? userId);
        Task<bool> AddEventAsync(AddEventInputModel model, string userId);
        Task<EditEventInputModel?> GetEventForEditingAsync(int? eventId, string userId);
        Task<bool> PersistUpdatedInformationAsync(EditEventInputModel model, string userId);
        Task<DeleteEventInputModel?> GetEventForDeletingAsync(int? eventId, string userId);
        Task<bool> DeleteEntityAsync(DeleteEventInputModel? model, string userId);
        Task<IEnumerable<GoingEventsViewModel>> GetAllGoingEventsAsync(string userId);
        Task<bool> AddEventToGoingAsync(int eventId, string userId);
        Task<bool> RemoveEventFromGoingAsync(int eventId, string userId);
    }
}
