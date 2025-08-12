using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Schedulefy.Data.Models;

namespace Schedulefy.Data
{
    public class SchedulefyDbContext : IdentityDbContext<IdentityUser>
    {
        public SchedulefyDbContext(DbContextOptions<SchedulefyDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserEvent>()
                .HasKey(ue => new
                {
                    ue.UserId,
                    ue.EventId
                });

            builder.Entity<UserTicket>()
                .HasKey(ut => ut.UserTicketId);

            builder.Entity<UserTicket>()
                .HasOne(ut => ut.User)
                .WithMany()
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserTicket>()
                .HasOne(ut => ut.Ticket)
                .WithMany()
                .HasForeignKey(ut => ut.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Event>()
                .HasOne(e => e.Publisher)
                .WithMany()
                .HasForeignKey(e => e.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Category>().HasData(
               new Category { Id = 1, Name = "Concert" },
               new Category { Id = 2, Name = "Public Party" },
               new Category { Id = 3, Name = "Birthday Party" },
               new Category { Id = 4, Name = "Anniversary" },
               new Category { Id = 5, Name = "Sport event" },
               new Category { Id = 6, Name = "Fashion Show" },
               new Category { Id = 7, Name = "Charity event"},
               new Category { Id = 8, Name = "Festival"},
               new Category { Id = 9, Name = "Fair"},
               new Category { Id = 10, Name = "Exhibition" }
           );

            var user1Id = "11111111-1111-1111-1111-111111111111";
            var user2Id = "22222222-2222-2222-2222-222222222222";

            builder.Entity<IdentityUser>().HasData(
                new IdentityUser
                {
                    Id = user1Id,
                    UserName = "publisher1@test.com",
                    NormalizedUserName = "PUBLISHER1@TEST.COM",
                    Email = "publisher1@test.com",
                    NormalizedEmail = "PUBLISHER1@TEST.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAEE1FakeHashExample",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                },
                new IdentityUser
                {
                    Id = user2Id,
                    UserName = "user1@test.com",
                    NormalizedUserName = "USER1@TEST.COM",
                    Email = "user1@test.com",
                    NormalizedEmail = "USER1@TEST.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAEE2FakeHashExample",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                }
            );



            builder.Entity<Ticket>().HasData(
                new Ticket { Id = 1, PricePerTicket = 50.00m },
                new Ticket { Id = 2, PricePerTicket = 20.00m }
            );


            builder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Name = "Rock Night Live",
                    Description = "An unforgettable rock concert with top bands.",
                    ImageUrl = "https://example.com/rocknight.jpg",
                    PublishedOn = DateTime.UtcNow.AddDays(-10),
                    IsPremium = true,
                    IsDeleted = false,
                    PublisherId = user1Id,
                    CategoryId = 1,
                    TicketId = 1
                },
                new Event
                {
                    Id = 2,
                    Name = "Summer Charity Gala",
                    Description = "A charity event for local causes with live performances.",
                    ImageUrl = "https://example.com/charitygala.jpg",
                    PublishedOn = DateTime.UtcNow.AddDays(-5),
                    IsPremium = false,
                    IsDeleted = false,
                    PublisherId = user1Id,
                    CategoryId = 7,
                    TicketId = 2
                }
            );


            builder.Entity<UserEvent>().HasData(
                new { UserId = user2Id, EventId = 1 },
                new { UserId = user2Id, EventId = 2 }
            );

 
            builder.Entity<UserTicket>().HasData(
                new UserTicket { UserTicketId = 1, UserId = user2Id, TicketId = 1, TickeetsCount = 2 },
                new UserTicket { UserTicketId = 2, UserId = user2Id, TicketId = 2, TickeetsCount = 4 }
            );

       
            builder.Entity<Comment>().HasData(
                new Comment { Id = 1, Content = "Amazing event!", EventId = 1, UserId = user2Id },
                new Comment { Id = 2, Content = "Can't wait for next year!", EventId = 2, UserId = user2Id }
            );

        }

        public virtual DbSet<Event> Events { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<UserEvent> UsersEvents { get; set; } = null!;
        public virtual DbSet<Ticket> Tickets { get; set; } = null!;
        public virtual DbSet<UserTicket> UsersTickets { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; }
    }
}
