using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemASP.Controllers
{
    public class ReaderController : Controller
    {
        public IActionResult ReaderDashboard()
        {
            return View();
        }

        public IActionResult ReaderBrowseBooks()
        {
            return View();
        }

        public IActionResult ReaderReservations()
        {
            return View();
        }

        public IActionResult ReaderBorrowing()
        {
            return View();
        }

        public IActionResult ReaderProfile()
        {
            return View();
        }

    }
}
