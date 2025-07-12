using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tourly.Models;

namespace tourly.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject ApplicationDbContext into the controller
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Login Page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {
           

            
                // Step 1: Check if the user exists in the database
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email);

                if (existingUser == null)
                {
                    // User not found
                    ModelState.AddModelError("Email", "No account found with that email.");
                Console.WriteLine("No user found");
                return View(user);  // Return with error
                }

                // Step 2: Verify the provided password against the hashed password
                var passwordHasher = new PasswordHasher<User>();
                var passwordVerificationResult = passwordHasher.VerifyHashedPassword(existingUser, existingUser.Password, user.Password);

                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    // Invalid password
                    ModelState.AddModelError("Password", "Incorrect password.");
                Console.WriteLine("Invalid password");

                return View(user);  // Return with error
                }

                // Step 3: If login is successful, save session or cookies
                HttpContext.Session.SetString("UserId", existingUser.Id.ToString());
                HttpContext.Session.SetString("UserName", existingUser.Name);
                HttpContext.Session.SetString("UserEmail", existingUser.Email);
            HttpContext.Session.SetString("UserType", existingUser.Type);

            // Redirect to a secure page after login (e.g., the homepage or dashboard)
            return RedirectToAction("Index", "Home");
                

            // If model validation fails, return to the login form with validation errors
        }

        // GET: Signup Page
        [HttpGet]
        public IActionResult Signup()
        {
            var user = new User();
            return View(user);
        }

        // POST: Signup Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(User user)
        {
            Console.WriteLine("Signup Requested");

            if (ModelState.IsValid)
            {
                // Step 1: Check if email already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email);

                if (existingUser != null)
                {
                    // Email already in use
                    ModelState.AddModelError("Email", "Email is already in use.");
                    return View(user); // Return with error
                }

                // Step 2: Hash the password before saving it to the database
                user.Password = new PasswordHasher<User>().HashPassword(user, user.Password);

                // Step 3: Save the user to the database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Step 4: Optionally, log the user in immediately after signup
                // You can skip this if you want to redirect to the login page instead
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserType", user.Type);

            
                // Show success message after registration
                TempData["SuccessMessage"] = "Account created successfully!";
                return RedirectToAction("Login");
            }

            // If model validation fails, return to the signup form with error messages
            return View(user);
        }
        [HttpGet]
        public IActionResult Signout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
