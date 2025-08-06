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

        }

        public virtual DbSet<Event> Events { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<UserEvent> UsersEvents { get; set; } = null!;
        public virtual DbSet<Ticket> Tickets { get; set; } = null!;
        public virtual DbSet<UserTicket> UsersTickets { get; set; } = null!;
    }
}
