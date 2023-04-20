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
                        ModelState.Clear();
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
                    ModelState.Clear();
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


        public IActionResult Welcome()
        {
            
            var content = context.TrainDetails.ToList();
            return View(content);
        }

        public IActionResult BookTicket()
        {
            return View();
        }

        [HttpPost]
        [Route("Account/BookTicket/{TrainId}")]
        public IActionResult BookTicket(int TrainId, BookingHistory bgh)
        {
            //try
            //{
            BookingHistory bookingHistory = new BookingHistory();

            var UserId = HttpContext.Session.GetInt32("UserId");

            var UserDetails = context.UserProfileDetails.SingleOrDefault(x => x.Id == UserId);
            bookingHistory.UserProfileDetails = UserDetails;
            var trainDetails = context.TrainDetails.SingleOrDefault(x => x.Id == TrainId);
            bookingHistory.TrainDetails = trainDetails;
            //bookingHistory.TrainId = trainDetails.Id;
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
            context.Bookings.Add(bookingHistory);
            context.SaveChanges();
            return RedirectToAction("PassengerDetails", new { id = bookingHistory.Id });
            //}
            //catch (Exception ex)
            //{
            //    return RedirectToAction("Login");
            //}
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


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
