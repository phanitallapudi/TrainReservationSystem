using Microsoft.AspNetCore.Mvc;
using TrainReservationSystem.Models;

namespace TrainReservationSystem.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(UserProfileDetails userProf)
        {
            return View();
        }
    }
}
