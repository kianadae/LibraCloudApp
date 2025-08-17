using LibraCloud.Data;
using LibraCloud.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraCloud.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservations (Admin sees all, Users see theirs)
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                // Admin: See all reservations
                var allReservations = await _context.Reservations
                    .Include(r => r.Book)
                    .Include(r => r.User)
                    .ToListAsync();
                return View(allReservations);
            }
            else
            {
                // Normal user: See own reservations
                var user = await _userManager.GetUserAsync(User);
                var reservations = await _context.Reservations
                    .Include(r => r.Book)
                    .Where(r => r.UserId == user.Id)
                    .ToListAsync();
                return View(reservations);
            }
        }

        // GET: Confirm Reservation
        public async Task<IActionResult> Create(int? bookId)
        {
            if (bookId == null)
            {
                return BadRequest("Missing book ID.");
            }

            var book = await _context.Books.FindAsync(bookId);
            if (book == null || book.QuantityAvailable <= 0)
            {
                return NotFound("Book not available.");
            }

            // Pre-fill BookId for reservation form (hidden field)
            var reservation = new Reservation
            {
                BookId = bookId.Value
            };

            return View(reservation);
        }

        // POST: Reserve Book - REPLACE THIS METHOD WITH THE FIXED VERSION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            var user = await _userManager.GetUserAsync(User);

            // Remove validation errors for navigation properties
            ModelState.Remove("User");
            ModelState.Remove("Book");
            ModelState.Remove("UserId");

            if (ModelState.IsValid && reservation.BookId > 0)
            {
                var book = await _context.Books.FindAsync(reservation.BookId);
                if (book == null || book.QuantityAvailable <= 0)
                {
                    ModelState.AddModelError("", "Book is not available.");
                    return View(reservation);
                }

                // Set the required properties
                reservation.UserId = user.Id;
                reservation.ReservationDate = DateTime.Now;
                reservation.Status = "Reserved";

                _context.Reservations.Add(reservation);
                book.QuantityAvailable -= 1; // Decrease quantity

                await _context.SaveChangesAsync();

                TempData["Success"] = "Book reserved successfully!";
                return RedirectToAction(nameof(Index));
            }

            // If we get here, something went wrong
            return View(reservation);
        }
    }
}