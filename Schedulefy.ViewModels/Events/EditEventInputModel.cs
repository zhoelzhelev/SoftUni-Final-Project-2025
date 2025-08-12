using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedulefy.ViewModels.Events
{
    public class EditEventInputModel : AddEventInputModel
    {
        public int Id { get; set; }
        
        //public string? PublisherId { get; set; } 
    }
}
