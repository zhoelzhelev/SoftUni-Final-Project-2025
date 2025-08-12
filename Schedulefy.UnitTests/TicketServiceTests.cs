using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Schedulefy.Data;
using Schedulefy.Data.Models;
using Schedulefy.Services.Core;
using Schedulefy.ViewModels.Tickets;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Schedulefy.Tests.Services
{
    public class TicketServiceTests
    {
        private SchedulefyDbContext _context;
        private TicketService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SchedulefyDbContext>()
                .UseInMemoryDatabase(databaseName: "TicketServiceTestDb")
                .Options;

            _context = new SchedulefyDbContext(options);

            // Reset DB
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _service = new TicketService(_context);
        }

        [Test]
        public async Task BuyTicketAsyncShouldAddUserTicketWhenValid()
        {
           
            var user = new IdentityUser 
            { 
                Id = "user1", 
                UserName = "user1@test.com" 
            };
            var ticket = new Ticket 
            { 
                Id = 1, 
                PricePerTicket = 10.0m 
            };
            var evnt = new Event 
            { 
                Id = 1, Name = "Concert", 
                Description = "Some Description",
                PublisherId = "id",
                ImageUrl = "image.jpg", 
                Ticket = ticket, TicketId = ticket.Id 
            };
            await _context.Users.AddAsync(user);
            await _context.Tickets.AddAsync(ticket);
            await _context.Events.AddAsync(evnt);
            await _context.SaveChangesAsync();

            var request = new BuyTicketRequest { EventId = evnt.Id, Quantity = 3 };

           
            var result = await _service.BuyTicketAsync(request, user.Id);

           
            Assert.IsNotNull(result);
            var userTicket = await _context.UsersTickets.FirstOrDefaultAsync();
            Assert.AreEqual(3, userTicket.TickeetsCount);
            Assert.AreEqual(ticket.Id, userTicket.TicketId);
        }

        [Test]
        public async Task BuyTicketAsyncShouldReturnNullWhenEventDoesNotExist()
        {
            var result = await _service.BuyTicketAsync(new BuyTicketRequest { EventId = 999 }, "nonexistent");
            Assert.IsNull(result);
        }

        [Test]
        public async Task LoadAllUserTicketsShouldReturnTicketsForUser()
        {
           
            var user = new IdentityUser 
            { 
                Id = "user2" 
            };
            var ticket = new Ticket 
            {
                Id = 2, 
                PricePerTicket = 20.0m 
            };
            var evnt = new Event 
            { 
                Id = 2, Name = "Festival", 
                Description = "Some Description",
                PublisherId = user.Id,
                ImageUrl = "img.jpg", 
                Ticket = ticket, 
                TicketId = ticket.Id 
            };

            await _context.Users.AddAsync(user);
            await _context.Tickets.AddAsync(ticket);
            await _context.Events.AddAsync(evnt);
            await _context.UsersTickets.AddAsync(new UserTicket
            {
                UserId = user.Id,
                TicketId = ticket.Id,
                TickeetsCount = 2
            });
            await _context.SaveChangesAsync();

           
            var result = await _service.LoadAllUserTickets(user.Id);

            
            Assert.AreEqual(1, result.Count());
            var ticketVm = result.First();
            Assert.AreEqual(2 * 20.0m, ticketVm.Price);
            Assert.AreEqual("Festival", ticketVm.EventTitleName);
        }

        [Test]
        public async Task LoadAllUserTicketsShouldReturnEmptyWhenUserHasNoTickets()
        {
            var result = await _service.LoadAllUserTickets("ghost-user");
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task LoadTicketInfoAsyncShouldReturnTicketWhenUserIsNotPublisher()
        {
            var user = new IdentityUser { Id = "user3" };
            var ticket = new Ticket { Id = 3, PricePerTicket = 30.0m };
            var evnt = new Event
            {
                Id = 3,
                Name = "Theatre",
                Description = "Some Description",
                ImageUrl = "theatre.jpg",
                Ticket = ticket,
                TicketId = ticket.Id,
                PublisherId = "publisher"
            };

            await _context.Tickets.AddAsync(ticket);
            await _context.Events.AddAsync(evnt);
            await _context.SaveChangesAsync();

            var result = await _service.LoadTicketInfoAsync(evnt.Id, user.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual("Theatre", result.EventTitle);
            Assert.AreEqual(30.0m, result.PricePerTicket);
        }

        [Test]
        public async Task LoadTicketInfoAsyncShouldReturnNullWhenUserIsPublisher()
        {
            var ticket = new Ticket { Id = 4, PricePerTicket = 50.0m };
            var evnt = new Event
            {
                Id = 4,
                Name = "Seminar",
                Description = "Some Description",
                ImageUrl = "seminar.jpg",
                Ticket = ticket,
                TicketId = ticket.Id,
                PublisherId = "user4"
            };

            await _context.Tickets.AddAsync(ticket);
            await _context.Events.AddAsync(evnt);
            await _context.SaveChangesAsync();

            var result = await _service.LoadTicketInfoAsync(evnt.Id, "user4");
            Assert.IsNull(result);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
