using HelpDeskSystem.ClaimsManagement;
using HelpDeskSystem.Data;
using HelpDeskSystem.Models;
using HelpDeskSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HelpDeskSystem.Controllers
{

    [Authorize]
    [Permission("DASHBOARD:VIEW")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        [Permission("DASHBOARD:VIEW")]
        public async Task<IActionResult> Index(TicketDashboardViewModel vm)
        {
            if (!User.Identity.IsAuthenticated)
            {

                return this.Redirect("~/identity/account/login");
            }
            else
            {

                vm.TicketsSummary = await _context.TicketsSummaryView.FirstOrDefaultAsync();

                vm.TicketsPriority = await _context.TicketsPriorityView.FirstOrDefaultAsync();


                vm.Tickets = await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.TicketComments)
                .OrderBy(x => x.CreatedOn)
                .ToListAsync();

                return View(vm);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
