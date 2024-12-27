using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemASP.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult AdminDashboard()
        {
            return View();
        }

        public IActionResult AdminBookManagement()
        {
            return View();
        }

        public IActionResult AdminUserManagement()
        {
            return View();
        }

        public IActionResult AdminRecords()
        {
            return View();
        }
    }
}
