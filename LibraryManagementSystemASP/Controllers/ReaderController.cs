using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemASP.Controllers
{
    public class ReaderController : Controller
    {
        public IActionResult ReaderDashboard()
        {
            return View();
        }
    }
}
