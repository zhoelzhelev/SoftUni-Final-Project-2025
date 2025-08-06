using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schedulefy.ViewModels.Events;

namespace Schedulefy.Services.Core.Contracts
{
    public interface ICategoryService
    {
        Task<IEnumerable<AddCategoriesDropDownMenu>> GetAllCategoriesAsync();
    }
}
