using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using TrainReservationSystem.Models;
using System.Windows;
using TrainReservationSystem.Services;

namespace TrainReservationSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbServicesContext context;

        public AccountController(DbServicesContext _context)
        {
            context = _context;
        }


        //methods start here

        private bool UserExists(string name)
        {
            return context.UserProfileDetails.Any(u => u.Email == name);
        }

        private bool PNRExists(int PNR)
        {
            return context.Bookings.Any(u => u.PNR == PNR);
        }

        public static int GeneratePNR()
        {
            Random rnd = new Random();
            return rnd.Next(10000000, 99999999);
        }


        public static string HashPassword(string password)
        {
            // generate a 128-bit salt using a secure random number generator
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // derive a 256-bit subkey (use HMACSHA256 with 10000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));

            // combine the salt and subkey into a single string
            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // split the combined string into two parts
            var parts = hashedPassword.Split('.', 2);
            if (parts.Length != 2)
            {
                return false;
            }

            var salt = Convert.FromBase64String(parts[0]);
            var subkey = Convert.FromBase64String(parts[1]);

            // hash the provided password using the same salt and number of iterations
            var hashed = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 10000, 32);

            // compare the computed hash with the stored hash
            return subkey.SequenceEqual(hashed);
        }


        //Methods ends here
        //ViewControllers start from here


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
            if (!UserExists(userProf.Email))
            {
                string emailPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
                bool isValidEmail = Regex.IsMatch(userProf.Email, emailPattern);

                if (isValidEmail)
                {

                    //if (userProf.Password != userProf.ConfirmPassword)
                    //{
                    //    TempData["ConfirmPassword"] = "Password is not matching";
                    //    return RedirectToAction("SignUp");
                    //}
                    if (userProf.Password != userProf.ConfirmPassword)
                    {
                        TempData["Message"] = "Passwords are not matching";
                        //ModelState.Clear();
                        return RedirectToAction("SignUp");
                    }
                    string hPass = HashPassword(userProf.Password);
                    userProf.Password = hPass;
                    //userProf.ConfirmPassword = hPass;


                    context.UserProfileDetails.Add(userProf);
                    int count = context.SaveChanges();

                    if (count > 0)
                    {
                        TempData["MessageACS"] = "Account created successfully, please log in.";
                    }
                    else
                    {
                        TempData["Message"] = "Error occurred.";
                        ModelState.Clear();
                        return RedirectToAction("SignUp");
                    }
                }
                else
                {
                    TempData["Message"] = "Please enter a valid email address.";
                    //ModelState.Clear();
                    return RedirectToAction("SignUp");
                }
            }
            else
            {
                TempData["MessageUAE"] = "User already exists.";
                ModelState.Clear();
                return RedirectToAction("SignUp");
            }

            return RedirectToAction("Login");
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserProfileDetails user)
        {
            if (user.Email == "admin" && user.Password == "admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            var storedUser = context.UserProfileDetails.SingleOrDefault(u => u.Email == user.Email);

            if (storedUser != null && VerifyPassword(user.Password, storedUser.Password))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("your_secret_key_here");

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                new Claim(ClaimTypes.Name, storedUser.Email)
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);
                HttpContext.Session.SetString("JWTtoken", jwtToken);
                


                //return Ok(new { token = tokenHandler.WriteToken(token) });
                HttpContext.Session.SetInt32("UserId", storedUser.Id);
                return RedirectToAction("Welcome");
            }
            else
            {
                TempData["Message"] = "Please enter valid account information.";
                ModelState.Clear();
                return RedirectToAction("Login");
            }
        }


        public IActionResult Welcome(string searchBy, string search, string origin, string destination)
        {
            var jwtToken = HttpContext.Session.GetString("JWTtoken");
            if (jwtToken == null)
            {
                return RedirectToAction("Login");
            }
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

            var trainDetails = context.TrainDetails.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                switch (searchBy)
                {
                    case "TrainId":
                        trainDetails = trainDetails.Where(x => x.TrainId == int.Parse(search));
                        break;
                    case "Origin":
                        trainDetails = trainDetails.Where(x => x.Origin.StartsWith(search));
                        break;
                    case "Destination":
                        trainDetails = trainDetails.Where(x => x.Destination.StartsWith(search));
                        break;
                }
            }
            else if (!string.IsNullOrEmpty(origin) && !string.IsNullOrEmpty(destination))
            {
                trainDetails = trainDetails.Where(x => x.Origin == origin && x.Destination == destination);
            }

            //removes expired train details

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

            return View(trainDetails.ToList());
        }

        public IActionResult BookTicket()
        {
            return View();
        }

        [HttpPost]
        [Route("Account/BookTicket/{TrainId}")]
        public IActionResult BookTicket(int TrainId, BookingHistory bgh)
        {
            if (bgh.ticketCount > 6)
            {
                TempData["TicketCountG6"] = "Cannot book more than 6 tickets ";
                return RedirectToAction("Welcome");
            }
            else if(bgh.ticketCount < 1)
            {
                TempData["TicketCountG6"] = "Please enter valid ticket count ";
                return RedirectToAction("Welcome");
            }

            BookingHistory bookingHistory = new BookingHistory();

            var UserId = HttpContext.Session.GetInt32("UserId");

            var UserDetails = context.UserProfileDetails.SingleOrDefault(x => x.Id == UserId);
            HttpContext.Session.SetString("UserEmail", UserDetails.Email);
            bookingHistory.UserProfileDetails = UserDetails;
            var trainDetails = context.TrainDetails.SingleOrDefault(x => x.Id == TrainId);
            bookingHistory.TrainDetails = trainDetails;
            bookingHistory.BookingDate = DateTime.Now;
            bool status = true;
            int tempPNR = 0;
            while(status)
            {
                tempPNR = GeneratePNR();
                if (!PNRExists(tempPNR))
                {
                    status = false;
                }

            }
            bookingHistory.PNR = tempPNR;
            bookingHistory.ticketCount = bgh.ticketCount;
            HttpContext.Session.SetInt32("ticketCount", bgh.ticketCount);
            ViewBag.PNR = bookingHistory.PNR;

            trainDetails.SeatCapacity -= bgh.ticketCount;

            context.Bookings.Add(bookingHistory);
            context.SaveChanges();

            //var emailAddress = HttpContext.Session.GetString("UserEmail");

            //string emailBody = $"Booking successfull, here is the PNR number for your booking {tempPNR}";

            //EmailService em = new EmailService();
            //em.SendEmail(emailBody, emailAddress);

            return RedirectToAction("PassengerDetails", new { id = bookingHistory.Id });
        }
        [HttpGet]
        public IActionResult PassengerDetails(int id)
        {
            var bookingHistory = context.Bookings.SingleOrDefault(b => b.Id == id);

            if (bookingHistory == null)
            {
                return RedirectToAction("Index");
            }

            var passengerList = new List<PassengerDetails>();

            for (int i = 0; i < bookingHistory.ticketCount; i++)
            {
                passengerList.Add(new PassengerDetails());
            }

            ViewBag.PNR = bookingHistory.PNR;

            return View(passengerList);
        }

        [HttpPost]
        public IActionResult PassengerDetails(List<PassengerDetails> passengerDetails)
        {
            if (passengerDetails == null)
            {
                return View("Welcome");
            }

            int pnr;
            if (int.TryParse(HttpContext.Request.Form["PNR"], out pnr))
            {
                foreach (var member in passengerDetails)
                {
                    member.PNR = pnr;
                }
            }



            context.PassengerDetails.AddRange(passengerDetails);
            context.SaveChanges();



            return RedirectToAction("Welcome");
        }

        public IActionResult BookedTicketHistory()
        {
            var UserId = HttpContext.Session.GetInt32("UserId");
            return View(context.Bookings.Where(x => x.UserId == UserId));
        }

        public IActionResult PNRViewAccount()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PNRViewAccount(int pnr)
        {
            PNR_ClassMembers pNR_Main_ClassMembers = new PNR_ClassMembers();

            List<PNR_PassengerDetails> pNR_Passengers = new List<PNR_PassengerDetails>();

            var bookingHistory = context.Bookings.FirstOrDefault(b => b.PNR == pnr);

            if (bookingHistory == null)
            {
                TempData["Message"] = "Please enter a valid PNR number.";
                return RedirectToAction("BookedTicketHistory");

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

        [HttpPost]
        public IActionResult CancelTicket(int id)
        {
            var bookingHistory = context.Bookings.SingleOrDefault(b => b.Id == id);

            if (bookingHistory == null)
            {
                return RedirectToAction("Index");
            }

            var trainDetails = context.TrainDetails.SingleOrDefault(x => x.Id == bookingHistory.TrainId);

            // Increase the seat capacity of the train
            trainDetails.SeatCapacity += bookingHistory.ticketCount;

            // Remove passenger details associated with the booking
            var passengerDetails = context.PassengerDetails.Where(p => p.PNR == bookingHistory.PNR).ToList();
            context.PassengerDetails.RemoveRange(passengerDetails);

            // Remove the booking
            context.Bookings.Remove(bookingHistory);

            context.SaveChanges();

            return RedirectToAction("BookedTicketHistory");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
