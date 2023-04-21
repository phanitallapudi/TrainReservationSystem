using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TrainReservationSystem.Models;

namespace TrainReservationSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbServicesContext context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(DbServicesContext _context, ILogger<HomeController> logger)
        {
            context = _context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult CheckPNR()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckPNR(int pnr)
        {
            PNR_ClassMembers pNR_Main_ClassMembers = new PNR_ClassMembers();

            List<PNR_PassengerDetails> pNR_Passengers = new List<PNR_PassengerDetails>();

            var bookingHistory = context.Bookings.FirstOrDefault(b => b.PNR == pnr);

            if (bookingHistory == null) 
            {
				TempData["Message"] = "Please enter a valid PNR number.";
                return RedirectToAction("Index");

			}

			var trainDetails = context.TrainDetails.FirstOrDefault(b => b.Id == bookingHistory.TrainId);
            var bookingDetails = context.Bookings.FirstOrDefault(b => b.PNR == pnr);
            var temp_pNR_Passengers = context.PassengerDetails.Where(b => b.PNR == pnr).ToList();

            foreach (var item in temp_pNR_Passengers)
            {
                PNR_PassengerDetails member = new PNR_PassengerDetails()
                {
                    Name = item.Name,
                    Age = item.Age,
                    Gender = item.Gender
                };
                pNR_Passengers.Add(member);
            }

            pNR_Main_ClassMembers.PNR = pnr;

            pNR_Main_ClassMembers.PassengerDetails = pNR_Passengers;
            pNR_Main_ClassMembers.TrainName = trainDetails.TrainName;
            pNR_Main_ClassMembers.TrainId = trainDetails.TrainId;
            pNR_Main_ClassMembers.Origin = trainDetails.Origin;
            pNR_Main_ClassMembers.Destination = trainDetails.Destination;
            pNR_Main_ClassMembers.Departure = trainDetails.Departure;
            pNR_Main_ClassMembers.Arrival = trainDetails.Arrival;

            pNR_Main_ClassMembers.BookingDate = bookingDetails.BookingDate;
            pNR_Main_ClassMembers.TicketCount = bookingDetails.ticketCount;


            return View(pNR_Main_ClassMembers);
        }


    }
}