using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemASP.Controllers
{
    public class LibrarianController : Controller
    {
        public IActionResult LibrarianDashboard()
        {
            return View();
        }
    }
}
