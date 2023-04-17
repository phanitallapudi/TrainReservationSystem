using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
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

                    if (userProf.Password != userProf.ConfirmPassword)
                    {
                        TempData["ConfirmPassword"] = "Password is not matching";
                        return RedirectToAction("SignUp");
                    }


                    userProf.ConfirmPassword = HashPassword(userProf.Password);


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

            return RedirectToAction();
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(UserProfileDetails userProf)
        {
            return View();
        }

    }
}
