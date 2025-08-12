using Microsoft.AspNetCore.Identity;
using Schedulefy.Data;
using Schedulefy.Services.Core;
using Schedulefy.Services.Core.Contracts;

using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Schedulefy.Data;
using Schedulefy.Data.Models;
using Schedulefy.Services.Core;
using Schedulefy.ViewModels.Events;
using System;
using System.Threading.Tasks;


namespace Schedulefy.UnitTests
{
    [TestFixture]
    public class EventServiceTests
    {
        private SchedulefyDbContext _context;
        private Mock<UserManager<IdentityUser>> _userManagerMock;
        private IEventService _eventService;
        private IdentityUser _testUser = null!;
        private Category _testCategory = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SchedulefyDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;

            _context = new SchedulefyDbContext(options);

            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _eventService = new EventService(_context, _userManagerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose(); 
        }

        [Test]
        public void PassAways()
        {
            Assert.Pass();
        }

        [Test]
        public async Task AddEventAsyncWithValidUserAndCategoryShouldReturnTrue()
        {
            _testUser = new IdentityUser { Id = "test-id", UserName = "TestUser" };
            _userManagerMock.Setup(um => um.FindByIdAsync("test-id"))
                .ReturnsAsync(_testUser);

            _testCategory = new Category { Id = 1, Name = "Music" };
            _context.Categories.Add(_testCategory);
            _context.SaveChanges();

            AddEventInputModel model = new AddEventInputModel
            {
                Name = "Test Event",
                Description = "asd asda awsd",
                ImageUrl = "http://image.com/image.png",
                PublishedOn = "05-08-2025", 
                CategoryId = 1,
                TicketPrice = 10.5m
            };

            
            bool result = await _eventService.AddEventAsync(model, "test-id");

            
            Assert.IsTrue(result);
            Assert.AreEqual(1, await _context.Events.CountAsync());

            var evt = await _context.Events.Include(e => e.Ticket).FirstOrDefaultAsync();
            Assert.NotNull(evt);
            Assert.AreEqual("Test Event", evt.Name);
            Assert.AreEqual(10.5m, evt.Ticket.PricePerTicket);
        }

        [Test]
        public async Task AddEventAsyncWithInvalidUserShouldReturnFalse()
        {
            
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityUser?)null);

            var model = new AddEventInputModel
            {
                Name = "Test Event",
                Description = "Test Description",
                ImageUrl = "http://image.com/image.png",
                PublishedOn = "2025-08-05",
                CategoryId = 1,
                TicketPrice = 10.5m
            };

           
            var result = await _eventService.AddEventAsync(model, "invalid-user");

            
            Assert.IsFalse(result);
            Assert.AreEqual(0, await _context.Events.CountAsync());
        }

        [Test]
        public async Task AddEventWithInvalidCategoryShouldReturnFlase()
        {
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityUser?)null);

            var model = new AddEventInputModel
            {
                Name = "Test Event",
                Description = "Test Description",
                ImageUrl = "http://image.com/image.png",
                PublishedOn = "2025-08-05",
                CategoryId = 10000,
                TicketPrice = 10.5m
            };


            var result = await _eventService.AddEventAsync(model, "invalid-user");


            Assert.IsFalse(result);
            Assert.AreEqual(0, await _context.Events.CountAsync());
        }

        [Test]
        public async Task AddEventToGoingAsyncShouldAddUserEventWhenValid()
        {
            
            var userId = "user-1";
            var user = new IdentityUser { Id = userId };
            var @event = new Event
            {
                Id = 1,
                Name = "Test Event",
                Description = "Some description",
                PublisherId = "publisher-123"
            };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            
            var result = await _eventService.AddEventToGoingAsync(1, userId);

            
            Assert.IsTrue(result);
            var userEvent = await _context.UsersEvents.FirstOrDefaultAsync();
            Assert.NotNull(userEvent);
            Assert.AreEqual(userId, userEvent.UserId);
            Assert.AreEqual(1, userEvent.EventId);
        }

        [Test]
        public async Task AddEventToGoingAsyncShouldNotAddWhenUserIsPublisher()
        {
            
            var userId = "publisher-123";
            var user = new IdentityUser { Id = userId };
            var @event = new Event
            {
                Id = 2,
                Name = "Self Event",
                Description = "Some description",
                PublisherId = userId
            };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            
            var result = await _eventService.AddEventToGoingAsync(2, userId);

            
            Assert.IsFalse(result);
            Assert.AreEqual(0, await _context.UsersEvents.CountAsync());
        }

        [Test]
        public async Task AddEventToGoingAsyncShouldNotAddWhenUserAlreadyJoined()
        {

            var userId = "user-123";
            var user = new IdentityUser { Id = userId };
            var @event = new Event
            {
                Id = 3,
                Name = "Test Event",
                Description = "Some description",
                PublisherId = "another-user"
            };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _context.Events.Add(@event);
            _context.UsersEvents.Add(new UserEvent
            {
                UserId = userId,
                EventId = 3
            });
            await _context.SaveChangesAsync();


            var result = await _eventService.AddEventToGoingAsync(3, userId);


            Assert.IsFalse(result);
            Assert.AreEqual(1, await _context.UsersEvents.CountAsync()); 
        }

        [Test]
        public async Task AddEventToGoingAsyncShouldReturnFalseWhenUserNotFound()
        {
            
            var userId = "missing-user";
            var @event = new Event
            {
                Id = 4,
                Name = "Another Event",
                Description = "Some description",
                PublisherId = "someone"
            };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((IdentityUser)null!);

            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            
            var result = await _eventService.AddEventToGoingAsync(4, userId);

            
            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddEventToGoingAsyncShouldReturnFalseWhenEventNotFound()
        {
            
            var userId = "user-x";
            var user = new IdentityUser { Id = userId };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            
            var result = await _eventService.AddEventToGoingAsync(99, userId); 

           
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteEntityAsyncShouldMarkEventAsDeletedWhenUserIsPublisher()
        {
            
            var userId = "user-1";
            var user = new IdentityUser { Id = userId };
            var eventEntity = new Event
            {
                Id = 1,
                Name = "Test Event",
                Description = "desc",
                PublisherId = userId,
                IsDeleted = false,
                PublishedOn = DateTime.Now,
                ImageUrl = "http://test.com/image.png"
            };

            await _context.Events.AddAsync(eventEntity);
            await _context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);

            var model = new DeleteEventInputModel { Id = 1 };

            
            var result = await _eventService.DeleteEntityAsync(model, userId);

            
            Assert.IsTrue(result);
            var dbEvent = await _context.Events.FindAsync(1);
            Assert.IsTrue(dbEvent.IsDeleted);
        }

        [Test]
        public async Task DeleteEntityAsyncShouldReturnFalseWhenUserIsNotPublisher()
        {
            
            var userId = "user-1";
            var otherUserId = "user-2";

            var user = new IdentityUser { Id = userId };

            var eventEntity = new Event
            {
                Id = 2,
                Name = "Another Event",
                Description = "desc",
                PublisherId = otherUserId,
                IsDeleted = false,
                PublishedOn = DateTime.Now,
                ImageUrl = "http://test.com/image2.png"
            };

            await _context.Events.AddAsync(eventEntity);
            await _context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);

            var model = new DeleteEventInputModel { Id = 2 };

            
            var result = await _eventService.DeleteEntityAsync(model, userId);

            
            Assert.IsFalse(result);
            var dbEvent = await _context.Events.FindAsync(2);
            Assert.IsFalse(dbEvent.IsDeleted);
        }

        [Test]
        public async Task DeleteEntityAsyncShouldReturnFalseWhenEventDoesNotExist()
        {
            
            var userId = "user-1";
            var user = new IdentityUser { Id = userId };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);

            var model = new DeleteEventInputModel { Id = 999 };

            
            var result = await _eventService.DeleteEntityAsync(model, userId);

            
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteEntityAsyncShouldReturnFalseWhenUserIsNull()
        {
            
            string userId = "non-existent";
            _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync((IdentityUser)null);

            var model = new DeleteEventInputModel { Id = 1 };

            
            var result = await _eventService.DeleteEntityAsync(model, userId);

            
            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetAllEventsAsyncShouldReturnEventsExcludingDeletedOnNullUser()
        {
            
            var category = new Category { Id = 1, Name = "Music" };
            var publisher = new IdentityUser { Id = "user1", NormalizedUserName = "PUBLISHER" };

            var event1 = new Event
            {
                Id = 1,
                Name = "Event 1",
                ImageUrl = "img1",
                Description = "desc",
                PublishedOn = DateTime.Now,
                Publisher = publisher,
                PublisherId = publisher.Id,
                Category = category,
                CategoryId = category.Id,
                IsDeleted = false
            };

            var event2 = new Event
            {
                Id = 2,
                Name = "Deleted Event",
                ImageUrl = "img2",
                Description = "desc",
                PublishedOn = DateTime.Now,
                Publisher = publisher,
                PublisherId = publisher.Id,
                Category = category,
                CategoryId = category.Id,
                IsDeleted = true
            };

            await _context.Categories.AddAsync(category);
            await _context.Users.AddAsync(publisher);
            await _context.Events.AddRangeAsync(event1, event2);
            await _context.SaveChangesAsync();

            
            var result = (await _eventService.GetAllEventsAsync(null)).ToList();

            
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Event 1", result[0].Name);
            Assert.IsFalse(result[0].IsPublisher);
            Assert.IsFalse(result[0].IsInGoing);
        }

        [Test]
        public async Task GetAllEventsAsyncShouldSetIsPublisherTrueWhenUserIsPublisher()
        {
            
            string userId = "user123";
            var category = new Category { Id = 1, Name = "Music" };
            var publisher = new IdentityUser { Id = userId, NormalizedUserName = "PUBLISHER" };

            var evt = new Event
            {
                Id = 1,
                Name = "Event 1",
                ImageUrl = "img1",
                Description = "desc",
                PublishedOn = DateTime.Now,
                Publisher = publisher,
                PublisherId = userId,
                Category = category,
                CategoryId = category.Id,
                IsDeleted = false
            };

            await _context.Categories.AddAsync(category);
            await _context.Users.AddAsync(publisher);
            await _context.Events.AddAsync(evt);
            await _context.SaveChangesAsync();

            
            var result = (await _eventService.GetAllEventsAsync(userId)).FirstOrDefault();

            
            Assert.NotNull(result);
            Assert.IsTrue(result.IsPublisher);
            Assert.IsFalse(result.IsInGoing);
        }

        public async Task GetAllGoingEventsAsyncReturnsCorrectEvents()
        {
            
            var userId = "user-1";
            var user = new IdentityUser { Id = userId, NormalizedUserName = "TESTUSER" };
            var category = new Category { Id = 1, Name = "Conference" };
            var evnt = new Event
            {
                Id = 1,
                Name = "Event A",
                Description = "Description A",
                ImageUrl = "http://image.com",
                PublishedOn = new DateTime(2025, 8, 5),
                Category = category,
                CategoryId = category.Id
            };
            var userEvent = new UserEvent
            {
                UserId = userId,
                User = user,
                Event = evnt,
                EventId = evnt.Id
            };

            await _context.Categories.AddAsync(category);
            await _context.Users.AddAsync(user);
            await _context.Events.AddAsync(evnt);
            await _context.UsersEvents.AddAsync(userEvent);
            await _context.SaveChangesAsync();

           
            var result = (await _eventService.GetAllGoingEventsAsync(userId)).ToList();

            
            Assert.AreEqual(1, result.Count);
            var goingEvent = result.First();
            Assert.AreEqual(evnt.Id, goingEvent.Id);
            Assert.AreEqual("Event A", goingEvent.Name);
            Assert.AreEqual("Description A", goingEvent.Description);
            Assert.AreEqual("TESTUSER", goingEvent.Publisher);
            Assert.AreEqual("Conference", goingEvent.Category);
            Assert.AreEqual("05/08/2025", goingEvent.PublishedOn);
        }

        [Test]
        public async Task GetEventDetailsAsyncReturnsCorrectViewModelWhenEventExists()
        {
            
            var userId = "user-123";
            var user = new IdentityUser { Id = userId, NormalizedUserName = "TESTUSER" };

            var category = new Category { Id = 1, Name = "Music" };

            var @event = new Event
            {
                Id = 10,
                Name = "Rock Concert",
                ImageUrl = "http://image.com/rock.jpg",
                Description = "Some Description",
                PublishedOn = new DateTime(2025, 8, 5),
                PublisherId = userId,
                Publisher = user,
                CategoryId = category.Id,
                Category = category,
                UsersEvents = new List<UserEvent>()
            };

            await _context.Users.AddAsync(user);
            await _context.Categories.AddAsync(category);
            await _context.Events.AddAsync(@event);
            await _context.SaveChangesAsync();

            
            var result = await _eventService.GetEventDetailsAsync(10, userId);

            
            Assert.IsNotNull(result);
            Assert.AreEqual("Rock Concert", result!.Name);
            Assert.AreEqual("TESTUSER", result.PublisherName);
            Assert.AreEqual("Music", result.CategoryName);
            Assert.AreEqual("05.08.2025", result.PublishedOn);
            Assert.IsTrue(result.IsPublisher);
            Assert.IsFalse(result.IsInGoing);
        }

        [Test]
        public async Task GetEventDetailsAsyncReturnsNullWhenEventDoesNotExist()
        {
            
            var result = await _eventService.GetEventDetailsAsync(999, "some-user");

            
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetEventForDeletingAsyncReturnsModelWhenUserIsPublisher()
        {
            var user = new IdentityUser { Id = "user-1", NormalizedUserName = "TESTUSER" };
            var eventEntity = new Event
            {
                Id = 1,
                Name = "Test Event",
                Description = "Some Description",
                PublisherId = user.Id,
                Publisher = user
            };

            await _context.Users.AddAsync(user);
            await _context.Events.AddAsync(eventEntity);
            await _context.SaveChangesAsync();

            var result = await _eventService.GetEventForDeletingAsync(1, "user-1");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result!.Id);
            Assert.AreEqual("Test Event", result.Name);
            Assert.AreEqual("TESTUSER", result.Publisher);
        }

        
        [Test]
        public async Task GetEventForDeletingAsyncReturnsNullWhenEventNotFound()
        {
            var result = await _eventService.GetEventForDeletingAsync(99, "user-1");
            Assert.IsNull(result);
        }

        
        [Test]
        public async Task GetEventForDeletingAsyncReturnsNullWhenEventIdIsNull()
        {
            var result = await _eventService.GetEventForDeletingAsync(null, "user-1");
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetEventForEditingAsyncReturnsModelWhenUserIsPublisher()
        {
            var user = new IdentityUser { Id = "user-1", NormalizedUserName = "TESTUSER" };
            var category = new Category { Id = 1, Name = "Category1" };
            var @event = new Event
            {
                Id = 1,
                Name = "Event1",
                Description = "Description",
                ImageUrl = "http://image.com/image.png",
                PublishedOn = new DateTime(2025, 8, 5),
                PublisherId = user.Id,
                Publisher = user,
                CategoryId = category.Id,
                Category = category
            };

            await _context.Users.AddAsync(user);
            await _context.Categories.AddAsync(category);
            await _context.Events.AddAsync(@event);
            await _context.SaveChangesAsync();

            var result = await _eventService.GetEventForEditingAsync(1, "user-1");

            Assert.IsNotNull(result);
            Assert.AreEqual(@event.Id, result!.Id);
            Assert.AreEqual(@event.Name, result.Name);
            Assert.AreEqual(@event.Description, result.Description);
            Assert.AreEqual(@event.ImageUrl, result.ImageUrl);
            Assert.AreEqual(@event.PublishedOn.ToString("dd.MM.yyyy"), result.PublishedOn); 
            Assert.AreEqual(category.Id, result.CategoryId);
        }

       

        [Test]
        public async Task GetEventForEditingAsyncReturnsNullWhenEventNotFound()
        {
            var result = await _eventService.GetEventForEditingAsync(99, "user-1");
            Assert.IsNull(result);
        }

       

        [Test]
        public async Task GetEventForEditingAsyncReturnsNullWhenEventIdIsNull()
        {
            var result = await _eventService.GetEventForEditingAsync(null, "user-1");
            Assert.IsNull(result);
        }

        [Test]
        public async Task PersistUpdatedInformationAsyncReturnsTrueWhenUpdateSucceeds()
        {
            
            var userId = "user-1";
            _testUser = new IdentityUser { Id = userId };
            _testCategory = new Category { Id = 1, Name = "Music" };

            var existingEvent = new Event
            {
                Id = 10,
                Name = "Old Name",
                Description = "Old Description",
                ImageUrl = "old.png",
                PublishedOn = DateTime.Parse("2023-01-01"),
                PublisherId = userId,
                CategoryId = _testCategory.Id,
                Category = _testCategory
            };

            await _context.Categories.AddAsync(_testCategory);
            await _context.Events.AddAsync(existingEvent);
            await _context.SaveChangesAsync();

            _userManagerMock.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(_testUser);

            var model = new EditEventInputModel
            {
                Id = 10,
                Name = "New Name",
                Description = "New Description",
                ImageUrl = "new.png",
                PublishedOn = "2025-08-05",
                CategoryId = 1
            };

            
            var result = await _eventService.PersistUpdatedInformationAsync(model, userId);

            
            Assert.IsTrue(result);

            var updatedEvent = await _context.Events.FindAsync(10);
            Assert.AreEqual("New Name", updatedEvent.Name);
            Assert.AreEqual("New Description", updatedEvent.Description);
            Assert.AreEqual("new.png", updatedEvent.ImageUrl);
            Assert.AreEqual(DateTime.Parse("2025-08-05"), updatedEvent.PublishedOn);
            Assert.AreEqual(1, updatedEvent.CategoryId);
        }

        [Test]
        public async Task PersistUpdatedInformationAsyncReturnsFalseWhenUserNotFound()
        {
            
            _userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser?)null);

            var model = new EditEventInputModel
            {
                Id = 10,
                CategoryId = 1,
                PublishedOn = "2025-08-05"
            };

           
            var result = await _eventService.PersistUpdatedInformationAsync(model, "invalid-user");

           
            Assert.IsFalse(result);
        }

        [Test]
        public async Task PersistUpdatedInformationAsyncReturnsFalseWhenCategoryNotFound()
        {
            // Arrange
            var userId = "user-1";
            _testUser = new IdentityUser { Id = userId };
            _userManagerMock.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(_testUser);

            var model = new EditEventInputModel
            {
                Id = 10,
                CategoryId = 999, 
                PublishedOn = "2025-08-05"
            };

            
            var result = await _eventService.PersistUpdatedInformationAsync(model, userId);

            
            Assert.IsFalse(result);
        }

        [Test]
        public async Task PersistUpdatedInformationAsyncReturnsFalseWhenEventNotFound()
        {
            
            var userId = "user-1";
            _testUser = new IdentityUser { Id = userId };
            _testCategory = new Category { Id = 1, Name = "Music" };
            await _context.Categories.AddAsync(_testCategory);
            await _context.SaveChangesAsync();

            _userManagerMock.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(_testUser);

            var model = new EditEventInputModel
            {
                Id = 999, 
                CategoryId = 1,
                PublishedOn = "2025-08-05"
            };

            
            var result = await _eventService.PersistUpdatedInformationAsync(model, userId);

            
            Assert.IsFalse(result);
        }

        
        [Test]
        public async Task RemoveEventFromGoingAsyncReturnsFalseWhenUserEventDoesNotExist()
        {
            
            var testEvent = new Event
            {
                Id = 1,
                Name = "Event 1",
                Description = "Some description",
                CategoryId = _testCategory.Id,
                Category = _testCategory,
                PublisherId = "another-user",
                PublishedOn = DateTime.UtcNow
            };

            _context.Events.Add(testEvent);
            await _context.SaveChangesAsync();

            _userManagerMock.Setup(u => u.FindByIdAsync(_testUser.Id))
                .ReturnsAsync(_testUser);

            
            var result = await _eventService.RemoveEventFromGoingAsync(testEvent.Id, _testUser.Id);

           
            Assert.IsFalse(result);
        }

        [Test]
        public async Task RemoveEventFromGoingAsyncReturnsFalseWhenUserNotFound()
        {
            
            var testEvent = new Event 
            { 
                Id = 1,  
                Description = "Some Description",
                Name = "Event 1",
                PublisherId = "Publisher"
            };
            _context.Events.Add(testEvent);
            await _context.SaveChangesAsync();

            _userManagerMock.Setup(u => u.FindByIdAsync(_testUser.Id))
                .ReturnsAsync((IdentityUser?)null);

            
            var result = await _eventService.RemoveEventFromGoingAsync(testEvent.Id, _testUser.Id);

            
            Assert.IsFalse(result);
        }

        [Test]
        public async Task RemoveEventFromGoingAsyncReturnsFalseWhenEventNotFound()
        {
            
            _userManagerMock.Setup(u => u.FindByIdAsync(_testUser.Id))
                .ReturnsAsync(_testUser);

            
            var result = await _eventService.RemoveEventFromGoingAsync(999, _testUser.Id);

            
            Assert.IsFalse(result);
        }

    }
}

