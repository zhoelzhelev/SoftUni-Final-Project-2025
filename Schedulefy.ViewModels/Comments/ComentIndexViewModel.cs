using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedulefy.ViewModels.Comments
{
    public class ComentIndexViewModel
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; } = null!;
        public string PublisherUserName { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
