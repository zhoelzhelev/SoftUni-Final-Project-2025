using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedulefy.ViewModels.Events
{
    public class GoingEventsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string Category { get; set; } = null!;
        public string? Publisher { get; set; }
        public string PublishedOn { get; set; } = null!;
    }
}
