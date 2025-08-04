using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedulefy.ViewModels.Events
{
    public abstract class BaseEventViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string PublishedOn { get; set; } = null!;
        public string PublisherName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public bool IsPublisher { get; set; }
        public bool IsInGoing { get; set; }
    }
}
