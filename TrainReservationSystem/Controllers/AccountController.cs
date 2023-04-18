﻿using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TrainReservationSystem.Models;

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

        public static int GeneratePNR()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999);
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
                return RedirectToAction("Welcome");
            }
            else
            {
                TempData["Message"] = "Please enter valid account information.";
                ModelState.Clear();
                return RedirectToAction("Login");
            }

            return RedirectToAction("Login");
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
        [HttpPost]
        public IActionResult BookTicket(int trainId, int ticketCount, List<PassengerDetails> passengerDetails)
        {
            var train = context.TrainDetails.SingleOrDefault(t => t.Id == trainId);
            if (train == null)
            {
                TempData["Message"] = "Invalid train selected.";
                return RedirectToAction("Welcome");
            }

            if (ticketCount > 6)
            {
                TempData["Message"] = "You cannot book more than 6 tickets at a time.";
                return RedirectToAction("Welcome");
            }

            var user = context.UserProfileDetails.SingleOrDefault(u => u.Email == User.Identity.Name);
            if (user == null)
            {
                TempData["Message"] = "User not found.";
                return RedirectToAction("Login");
            }

            if (train.SeatCapacity < ticketCount)
            {
                TempData["Message"] = "Not enough seats available for the selected train.";
                return RedirectToAction("Welcome");
            }

            var booking = new BookingHistory
            {

                PNR = GeneratePNR(),
                BookingDate = DateTime.UtcNow,
                ticketCount = ticketCount,
                TrainId = train.Id,
                UserId = user.Id
            };

            var passengers = new List<PassengerDetails>();
            for (int i = 0; i < ticketCount; i++)
            {
                if (i >= passengerDetails.Count)
                {
                    passengers.Add(new PassengerDetails { Name = "Passenger " + (i + 1), Age = 0 });
                }
                else
                {
                    passengers.Add(passengerDetails[i]);
                }
            }

            context.Bookings.Add(booking);
            context.SaveChanges();

            foreach (var passenger in passengers)
            {
                passenger.BookingId = booking.Id;
                context.PassengerDetails.Add(passenger);
            }

            train.SeatCapacity -= ticketCount;
            context.TrainDetails.Update(train);
            context.SaveChanges();

            TempData["Message"] = "Tickets booked successfully!";
            return RedirectToAction("Welcome");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
