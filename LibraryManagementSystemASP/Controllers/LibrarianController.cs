using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemASP.Controllers
{
    public class LibrarianController : Controller
    {
        public IActionResult LibrarianDashboard()
        {
            return View();
        }

        public IActionResult LibrarianBookListings()
        {
            return View();
        }

        public IActionResult LibrarianOperationsManagement()
        {
            return View();
        }

        public IActionResult LibrarianRecords()
        {
            return View();
        }
    }
}
