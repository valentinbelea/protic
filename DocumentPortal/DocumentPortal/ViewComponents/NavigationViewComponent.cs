using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using DocumentPortal.Data;

namespace DocumentPortal.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public NavigationViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menus = await _context.Menus.OrderBy(m => m.Order).ThenBy(m => m.Id).ToListAsync();
            return View(menus);
        }
    }
}
