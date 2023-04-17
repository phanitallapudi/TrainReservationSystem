using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var obj = context.TrainDetails.ToList();
            return View(obj);
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

        public IActionResult Edit(int id)
        {
            var elem = context.TrainDetails.SingleOrDefault(model => model.Id == id);
            if (elem == null)
            {
                return View();
            }
            return View(elem);
        }

        [HttpPost]
        public IActionResult Edit(TrainDetails stud)
        {
            if (!ModelState.IsValid)
            {
                return View(stud);
            }
            context.Entry(stud).State = EntityState.Modified;
            int change = context.SaveChanges();

            if (change > 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.EditMdsg = ("<script>alert('Error occured')</script>");
                return View(stud);
            }
        }
        public ActionResult InfoEdit(int id)
        {
            var existingObject = context.TrainDetails.FirstOrDefault(x => x.Id == id);

            if (existingObject == null)
            {
            }

            return View(existingObject);
        }


        [HttpPost]
        public ActionResult InfoEdit(TrainDetails model)
        {
            var existingObject = context.TrainDetails.FirstOrDefault(x => x.Id == model.Id);

            if (existingObject == null)
            {
                
            }

            existingObject.TrainName = model.TrainName;
            existingObject.TrainId = model.TrainId;
            existingObject.Origin = model.Origin;
            existingObject.Destination = model.Destination;
            existingObject.Departure = model.Departure;
            existingObject.Arrival = model.Arrival;
            existingObject.SeatCapacity = model.SeatCapacity;
            existingObject.SeatRate = model.SeatRate;

            context.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult InfoDelete(int id)
        {
            var changes = context.TrainDetails.Where(model => model.Id == id).FirstOrDefault();
            return View(changes);
        }

        [HttpPost]
        public ActionResult InfoDelete(TrainDetails stud)
        {
            context.Entry(stud).State = EntityState.Deleted;
            var removeData = context.TrainDetails.Where(m => m.Id == stud.Id).FirstOrDefault();
            //context.Std_TableInfo.Remove(removeData);
            int change = context.SaveChanges();

            if (change > 0)
            {
                ModelState.Clear();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.EditMdsg = ("<script>alert('Error occured')</script>");
            }
            return View();
        }

    }
}
