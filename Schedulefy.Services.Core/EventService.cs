using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Schedulefy.Data;
using Schedulefy.Data.Models;
using Schedulefy.Services.Core.Contracts;
using Schedulefy.ViewModels.Events;
using static Schedulefy.OCommon.ValidationConstants.Event;

namespace Schedulefy.Services.Core
{
    public class EventService : IEventService
    {
        private readonly SchedulefyDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EventService(SchedulefyDbContext context, UserManager<IdentityUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        public async Task<bool> AddEventAsync(AddEventInputModel model, string userId)
        {
            bool result = false;

            IdentityUser? user = await this._userManager.FindByIdAsync(userId);
            Category? category = await this._context
                .Categories
                .FindAsync(model.CategoryId);
            

            if ((user is not null) && (category is not null)) 
            {
                Ticket ticket = new Ticket
                {
                    PricePerTicket = model.TicketPrice
                };

                await this._context.Tickets.AddAsync(ticket);
                await this._context.SaveChangesAsync();

                Event entity = new Event
                {
                    Name = model.Name,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    PublishedOn = DateTime.Parse(model.PublishedOn),
                    Publisher = user,
                    PublisherId = user.Id,
                    Category = category,
                    CategoryId = category.Id,
                    Ticket = ticket,
                    TicketId = ticket.Id
                };

                result = true;

                await this._context.Events.AddAsync(entity);
                await this._context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<bool> AddEventToGoingAsync(int eventId, string userId)
        {
            bool result = false;

            IdentityUser? user = await this._userManager.FindByIdAsync(userId);
            Event? entity = await this._context
                .Events
                .SingleOrDefaultAsync(e => e.Id == eventId);

            if ((user is not null) && (entity is not null) &&
                entity.PublisherId.ToLower() != userId.ToLower()) 
            {
                UserEvent? userEvent = await this._context
                    .UsersEvents
                    .SingleOrDefaultAsync(ue => ue.UserId.ToLower() == userId.ToLower() &&
                    ue.EventId == eventId);

                if(userEvent is null)
                {
                    userEvent = new UserEvent()
                    {
                        UserId = userId,
                        User = user,
                        Event = entity,
                        EventId = eventId
                    };

                    await this._context.UsersEvents.AddAsync(userEvent);
                    await this._context.SaveChangesAsync();

                    result = true;
                }
            }

            return result;
        }

        public async Task<bool> DeleteEntityAsync(DeleteEventInputModel? model, string userId)
        {
            bool result = false;

            IdentityUser? user = await this._userManager.FindByIdAsync(userId);
            Event? entity = await this._context
                .Events
                .FindAsync(model?.Id);

            if((user is not null) && (entity is not null) &&
                ((entity.PublisherId.ToLower() == userId.ToLower()) || await (IsUserAdminAsync(userId))))
            {
                entity.IsDeleted = true;
                result = true;
                await this._context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<IEnumerable<IndexEventViewModel>> GetAllEventsAsync(string? userId)
        {
            IEnumerable<IndexEventViewModel> viewModels = await this._context
                .Events
                .Where(e => !e.IsDeleted)
                .Include(e => e.Publisher)
                .Include(e => e.Category)
                .Select(e => new IndexEventViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    ImageUrl = e.ImageUrl,
                    PublishedOn = e.PublishedOn.ToString(PublishedOnCorrectFormat),
                    PublisherName = e.Publisher.NormalizedUserName!,
                    CategoryName = e.Category.Name,
                    GoingCount = e.UsersEvents.Count(),
                    IsPublisher = userId != null ? 
                        e.PublisherId.ToLower() == userId.ToLower() : false,
                    IsInGoing = userId != null ?
                        e.UsersEvents.Any(ue => ue.UserId.ToLower() == userId.ToLower()) : false,

                })
                .ToArrayAsync();

            return viewModels;
        }

        public async Task<IEnumerable<GoingEventsViewModel>> GetAllGoingEventsAsync(string userId)
        {
            IEnumerable<GoingEventsViewModel> goingEvents = await this._context
                .UsersEvents
                .Include(ue => ue.Event)
                .ThenInclude(e => e.Category)
                .Where(ue => ue.UserId.ToLower() == userId.ToLower())
                .Select(ue => new GoingEventsViewModel
                {
                    Id = ue.EventId,
                    Name = ue.Event.Name,
                    Description = ue.Event.Description,
                    ImageUrl = ue.Event.ImageUrl,
                    Publisher = ue.User.NormalizedUserName,
                    PublishedOn = ue.Event.PublishedOn.ToString(PublishedOnCorrectFormat),
                    Category = ue.Event.Category.Name
                })
                .ToArrayAsync();

            return goingEvents;
        }

        public async Task<DetailsEventViewModel?> GetEventDetailsAsync(int eventId, string? userId)
        {
            DetailsEventViewModel? viewModel = null;

            Event? entity = await this._context
                .Events
                .Include(e => e.Category)
                .Include(e => e.Publisher)
                .Include(e => e.UsersEvents)
                .SingleOrDefaultAsync(e => e.Id == eventId);

            if(entity is not null)
            {
                viewModel = new DetailsEventViewModel()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    ImageUrl = entity.ImageUrl,
                    PublishedOn = entity.PublishedOn.ToString(PublishedOnCorrectFormat),
                    PublisherName = entity.Publisher.NormalizedUserName!,
                    CategoryName = entity.Category.Name,
                    IsPublisher = userId != null ?
                        entity.PublisherId.ToLower() == userId.ToLower() : false,
                    IsInGoing = userId != null ?
                        entity.UsersEvents.Any(ue => ue.UserId.ToLower() == userId.ToLower()) : false,
                };
            }

            return viewModel;
        }

        public async Task<DeleteEventInputModel?> GetEventForDeletingAsync(int? eventId, string userId)
        {
            DeleteEventInputModel? model = null;

            if(eventId is not null ) 
            {
                Event? entity = await this._context
                    .Events
                    .Include(e => e.Publisher)
                    .SingleOrDefaultAsync(e => e.Id == eventId);

                if (entity is not null &&
                    ((entity.PublisherId.ToLower() == userId.ToLower()) || (await this.IsUserAdminAsync(userId)))) 
                {
                    model = new DeleteEventInputModel()
                    {
                        Id = entity.Id,
                        Name = entity.Name,
                        Publisher = entity.Publisher.NormalizedUserName,
                        PublisherId = entity.PublisherId,
                    };
                }

            }

            return model;
        }

        public async Task<EditEventInputModel?> GetEventForEditingAsync(int? eventId, string userId)
        {
            EditEventInputModel? model = null;

            if(eventId is not null)
            {
                Event? entity = await this._context
                    .Events
                    .Include(e => e.Category)
                    .Include(e => e.Publisher)
                    .SingleOrDefaultAsync(e => e.Id == eventId);

                if (entity is not null && 
                    ((entity.PublisherId.ToLower() == userId.ToLower()) || await this.IsUserAdminAsync(userId))) 
                {
                    model = new EditEventInputModel()
                    {
                        Id = entity.Id,
                        Name = entity.Name,
                        Description = entity.Description,
                        ImageUrl = entity.ImageUrl,
                        PublishedOn = entity.PublishedOn.ToString(PublishedOnCorrectFormat),
                        CategoryId = entity.Category.Id,
                        //PublisherId = userId
                    };
                }
            }

            return model;
        }

        public async Task<bool> PersistUpdatedInformationAsync(EditEventInputModel model, string userId)
        {
            bool result = false;

            IdentityUser? user = await this._userManager.FindByIdAsync(userId);
            Category? category = await this._context
                .Categories
                .FindAsync(model.CategoryId);
           

            if ((user is not null) && (category is not null)) 
            {
                Event? entity = await this._context
                    .Events
                    .SingleOrDefaultAsync(e => e.Id == model.Id);

                if (entity is not null) 
                {
                    entity.Name = model.Name;
                    entity.Description = model.Description;
                    entity.ImageUrl = model.ImageUrl;
                    entity.PublishedOn = DateTime.Parse(model.PublishedOn);
                    entity.CategoryId = category.Id;
                    entity.Category = category;

                    await this._context.SaveChangesAsync();

                    result = true;
                }
            }

            return result;
        }

        public async Task<bool> RemoveEventFromGoingAsync(int eventId, string userId)
        {
            bool result = false;

            IdentityUser? user = await this._userManager.FindByIdAsync(userId);
            Event? entity = await this._context
                .Events
                .SingleOrDefaultAsync(e => e.Id == eventId);

            if (user is not null && entity is not null) 
            {
                UserEvent? userEvent = await this._context
                    .UsersEvents
                    .SingleOrDefaultAsync(ue => ue.UserId.ToLower() == userId.ToLower() && ue.EventId == eventId);

                if(userEvent is not null)
                {
                    this._context.UsersEvents.Remove(userEvent);
                    await this._context.SaveChangesAsync();

                    result = true;
                }
            }

            return result;
        }

        private async Task<bool> IsUserAdminAsync(string userId)
        {
            bool result = false;

            var user = await this._userManager.FindByIdAsync(userId);

            if (user != null)
            {
                bool isInRole = await this._userManager.IsInRoleAsync(user, "Admin");

                if (isInRole)
                {
                    result = true;
                }
            }

            return result; 
        }



    }
}
