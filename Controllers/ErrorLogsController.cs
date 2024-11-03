using HelpDeskSystem.ClaimsManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskSystem.Controllers
{

    [Authorize]
    [Permission("ERROLOGS:VIEW")]
    public class ErrorLogsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
