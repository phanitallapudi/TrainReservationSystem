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
            var bookings = context.Bookings.ToList();
            var expiredBk = context.Bookings.ToList();
            List<BookingHistory> bookingHistory = new List<BookingHistory>();
            List<BookingHistory> expiredBookings = new List<BookingHistory>();

            //removes failed bookings
            if (bookings.Count != 0)
            {
                for (int i = bookings.Count - 1; i >= 0; i--)
                {
                    var passengerDetails = context.PassengerDetails.Where(x => x.PNR == bookings[i].PNR).ToList();
                    if (passengerDetails.Count == 0)
                    {
                        var trainDetails_delete = context.TrainDetails.SingleOrDefault(x => x.Id == bookings[i].TrainId);
                        trainDetails_delete.SeatCapacity += bookings[i].ticketCount;

                        bookingHistory.Add(bookings[i]);
                        bookings.RemoveAt(i);
                    }
                    context.Bookings.RemoveRange(bookingHistory);
                    context.SaveChanges();
                }
            }

            //removes expired bookings i.e., train which are already left
            if (expiredBk.Count != 0)
            {
                for (int i = expiredBk.Count - 1; i >= 0; i--)
                {
                    var tempTrainDetails = context.TrainDetails.SingleOrDefault(x => x.Id == expiredBk[i].TrainId);
                    if (tempTrainDetails == null || tempTrainDetails.Departure < DateTime.Now)
                    {
                        expiredBookings.Add(expiredBk[i]);
                        expiredBk.RemoveAt(i);
                    }
                    context.Bookings.RemoveRange(expiredBookings);
                    context.SaveChanges();
                }
            }

            //removes expired train details i.e., train which are already left

            var expiredTrainDetails = context.TrainDetails.Where(td => td.Departure < DateTime.Now).ToList();

            foreach (var expiredTrainDetail in expiredTrainDetails)
            {
                var tempBookings = context.Bookings.Where(b => b.TrainId == expiredTrainDetail.TrainId).ToList();
                context.Bookings.RemoveRange(tempBookings);
            }

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