using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;
using tourly.Models;

namespace tourly.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewBag.authenticated = true;
                ViewBag.userId = HttpContext.Session.GetString("UserId");
                ViewBag.userName = userName;
                ViewBag.userEmail = HttpContext.Session.GetString("UserEmail");

                if (HttpContext.Session.GetString("UserType") == "Guide")
                {
                    return RedirectToAction("Guide", "Home");
                }
            }
            else
            {
                ViewBag.authenticated = false;
            }
            var packages = await _context.Packages.ToListAsync();
            return View(packages);
        }

        public async Task<IActionResult> Guide()
        {
            var userName = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(userName))
            {
                ViewBag.authenticated = true;
                ViewBag.userId = HttpContext.Session.GetString("UserId");
                ViewBag.userName = userName;
                ViewBag.userEmail = HttpContext.Session.GetString("UserEmail");

                if (HttpContext.Session.GetString("UserType") == "Guide")
                {
                    var packages = await _context.Packages
                        .Where(p => p.Creator == HttpContext.Session.GetString("UserId"))
                        .ToListAsync();
                    return View(packages);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Bookings(string PackageId)
        {
            var userName = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(userName))
            {
                ViewBag.authenticated = true;
                ViewBag.userId = HttpContext.Session.GetString("UserId");
                ViewBag.userName = userName;
                ViewBag.userEmail = HttpContext.Session.GetString("UserEmail");

                if (HttpContext.Session.GetString("UserType") == "Guide")
                {
                    var bookings = await (from b in _context.Bookings
                                          join u in _context.Users on b.UserId equals u.Id
                                          where b.PackageId.ToString() == PackageId
                                          select new
                                          {
                                              b.Id,
                                              b.PackageId,
                                              b.UserId,
                                              u.Name,
                                              u.Email,
                                              b.CreatedAt
                                          }).ToListAsync();

                    var bookingViewModels = bookings.Select(b => new BookingViewModel
                    {
                        Id = b.Id,
                        PackageId = b.PackageId,
                        UserId = b.UserId,
                        UserName = b.Name,
                        UserEmail = b.Email,
                        BookingDate = b.CreatedAt
                    }).ToList();

                    return View(bookingViewModels);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> NewBooking(Booking booking)
        {
            var existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.UserId == booking.UserId && b.PackageId == booking.PackageId);

            if (existingBooking != null)
            {
                return RedirectToAction("Index", "Home");
            }

            booking.CreatedAt = DateTime.UtcNow;
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            Console.WriteLine("New booking added");
            return RedirectToAction("MyBookings", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Package package)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("Valid package");

                _context.Packages.Add(package);
                await _context.SaveChangesAsync();

                return RedirectToAction("Guide", "Home");
            }

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return RedirectToAction("Guide", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string PackageId)
        {
            if (!string.IsNullOrEmpty(PackageId))
            {
                var package = await _context.Packages.FirstOrDefaultAsync(p => p.Id.ToString() == PackageId);

                if (package != null)
                {
                    var bookingsToDelete = await _context.Bookings
                        .Where(b => b.PackageId.ToString() == PackageId)
                        .ToListAsync();

                    if (bookingsToDelete.Any())
                    {
                        _context.Bookings.RemoveRange(bookingsToDelete);
                    }

                    _context.Packages.Remove(package);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Guide", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Mybookings()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (!string.IsNullOrEmpty(userName))
            {
                ViewBag.authenticated = true;
                ViewBag.userId = HttpContext.Session.GetString("UserId");
                ViewBag.userName = userName;
                ViewBag.userEmail = HttpContext.Session.GetString("UserEmail");

                if (HttpContext.Session.GetString("UserType") == "Guide")
                {
                    return RedirectToAction("Guide", "Home");
                }
            }
            else
            {
                ViewBag.authenticated = false;
                return RedirectToAction("Index", "Home");
            }

            var bookings = await _context.Bookings
                .Where(b => b.UserId.ToString() == HttpContext.Session.GetString("UserId"))
                .ToListAsync();

            var packageIds = bookings.Select(b => b.PackageId).ToList();

            var packages = await _context.Packages
                .Where(p => packageIds.Contains(p.Id))
                .ToListAsync();

            var bookingWithPackages = bookings.Select(b => new MybookingModel
            {
                Booking = b,
                Package = packages.FirstOrDefault(p => p.Id == b.PackageId)
            }).ToList();

            return View(bookingWithPackages);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(string BookingId)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id.ToString() == BookingId);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync(); // ? This ensures deletion is persisted
            }

            return RedirectToAction("Mybookings", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
