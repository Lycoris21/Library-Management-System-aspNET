using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemASP.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult AdminDashboard()
        {
            return View();
        }
    }
}
