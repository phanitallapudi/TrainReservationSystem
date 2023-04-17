using Microsoft.AspNetCore.Mvc;
using TrainReservationSystem.Models;

namespace TrainReservationSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly DbServicesContext context;

        public AdminController(DbServicesContext _context)
        {
            context = _context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(TrainDetails obj)
        {
            context.TrainDetails.Add(obj);
            context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
