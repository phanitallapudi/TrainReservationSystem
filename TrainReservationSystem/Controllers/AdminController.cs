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


        //private bool IsTrainDepartureTimePassed(DateTime departureTime)
        //{
        //    return DateTime.Now > departureTime;
        //}

        public IActionResult Index(string searchBy, string search)
        {
            if (searchBy == "TrainId")
            {
                int number = Convert.ToInt32(search);
                return View(context.TrainDetails.Where(x => x.TrainId == number));
            }
            else if (searchBy == "Origin")
            {
                return View(context.TrainDetails.Where(x => x.Origin == search));
            }
            else if (searchBy == "Destination")
            {
                return View(context.TrainDetails.Where(x => x.Destination == search));
            }
            else
            {
                var expiredTrainDetails = context.TrainDetails.Where(td => td.Departure < DateTime.Now).ToList();

                // Remove the expired train details from the context
                //foreach (var trainDetails in expiredTrainDetails)
                //{
                //    OlderTrainDetails olderTrainDetails = new OlderTrainDetails
                //    {
                //        Id = trainDetails.Id,
                //        TrainName = trainDetails.TrainName,
                //        TrainId = trainDetails.TrainId,
                //        Origin = trainDetails.Origin,
                //        Destination = trainDetails.Destination,
                //        Departure = trainDetails.Departure,
                //        Arrival = trainDetails.Arrival,
                //        SeatCapacity = trainDetails.SeatCapacity,
                //        SeatRate = trainDetails.SeatRate
                //    };
                //    context.OlderTrainDetails.Add(olderTrainDetails);
                //}    
                //context.RemoveRange(expiredTrainDetails);
                //context.SaveChanges();

                var olderTrainDetails = expiredTrainDetails
                    .Select(td => new OlderTrainDetails
                    {
                        TrainName = td.TrainName,
                        TrainId = td.TrainId,
                        Origin = td.Origin,
                        Destination = td.Destination,
                        Departure = td.Departure,
                        Arrival = td.Arrival,
                        SeatCapacity = td.SeatCapacity,
                        SeatRate = td.SeatRate
                    })
                    .ToList();

                // Add the new objects to the context and remove the expired train details
                context.OlderTrainDetails.AddRange(olderTrainDetails);
                context.TrainDetails.RemoveRange(expiredTrainDetails);
                context.SaveChanges();


                var obj = context.TrainDetails.ToList();
                return View(obj);
            }
            
            
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(TrainDetails obj)
        {
            if (obj.Departure < DateTime.Now || obj.Arrival < obj.Departure)
            {
                TempData["Message"] = "Please enter valid departure or arrival date.";
                return View("Create");
            }
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
