using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Schedulefy.Data;
using Schedulefy.Services.Core.Contracts;
using Schedulefy.ViewModels.Events;

namespace Schedulefy.Services.Core
{
    public class CategoryService : ICategoryService
    {
        private readonly SchedulefyDbContext _context;

        public CategoryService(SchedulefyDbContext context)
        {
            this._context = context;
        }

        

        public async Task<IEnumerable<AddCategoriesDropDownMenu>> GetAllCategoriesAsync()
        {
            IEnumerable<AddCategoriesDropDownMenu> dropDownMenu = await this._context
                .Categories
                .Select(c => new AddCategoriesDropDownMenu
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToArrayAsync();

            return dropDownMenu;
        }
    }
}
