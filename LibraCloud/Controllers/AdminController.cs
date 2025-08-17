using LibraCloud.Data;
using LibraCloud.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraCloud.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ========================
        // DASHBOARD + RESERVATIONS
        // ========================
        public IActionResult Dashboard()
        {
            var books = _context.Books.ToList();

            var activeReservations = _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => r.Status != "Canceled")
                .ToList();

            ViewBag.TotalBooks = books.Count;
            ViewBag.TotalReservations = activeReservations.Count;
            ViewBag.Reservations = activeReservations;

            // ✅ Add this to show users
            ViewBag.Users = _userManager.Users.ToList();

            return View(books);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelReservation(int id)
        {
            var reservation = _context.Reservations
                .Include(r => r.Book)
                .FirstOrDefault(r => r.Id == id);

            if (reservation == null) return NotFound();

            if (reservation.Status != "Canceled")
            {
                reservation.Status = "Canceled";

                if (reservation.Book != null)
                {
                    reservation.Book.QuantityAvailable += 1;
                }

                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Dashboard));
        }

        // ==========
        // USER CRUD
        // ==========

        // GET: /Admin/Users
        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // GET: /Admin/CreateUser
        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: /Admin/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Optional: await _userManager.AddToRoleAsync(user, "User");
                return RedirectToAction(nameof(Users));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // GET: /Admin/EditUser/{id}
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };

            return View(model);
        }

        // POST: /Admin/EditUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.Email = model.Email;
            user.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Users));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // GET: /Admin/DeleteUser/{id}
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new DeleteUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };

            return View(model);
        }

        // POST: /Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(DeleteUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Users));
            }

            ModelState.AddModelError("", "Error deleting user.");
            return View(model);
        }
    }
}
