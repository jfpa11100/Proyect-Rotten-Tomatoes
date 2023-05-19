using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyect_Rotten_Tomatoes.Data;
using Proyect_Rotten_Tomatoes.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace Proyect_Rotten_Tomatoes.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Proyect_Rotten_TomatoesContext _context;


        public HomeController(ILogger<HomeController> logger, Proyect_Rotten_TomatoesContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Set the appropriate layout based on the user's role
            if (role == "Cinephile")
            {
                ViewData["Layout"] = "~/Views/Shared/_Layout_Cinephile.cshtml";
            }
            else if (role == "FilmExpert")
            {
                ViewData["Layout"] = "~/Views/Shared/_Layout_Film_Expert.cshtml";
            }
            else
            {
                ViewData["Layout"] = "~/Views/Shared/_Layout.cshtml";
            }

            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }


        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(string email, string password)
        {
            var cinephile = await _context.Cinephile.SingleOrDefaultAsync(m => m.Email == email);
            var filmExpert = await _context.Film_Expert.SingleOrDefaultAsync(m => m.Email == email);

            if (cinephile == null && filmExpert == null)
            {
                ModelState.AddModelError(string.Empty, "Email not found.");
                return View();
            }

            if (cinephile != null && cinephile.Password == password)
            {
                // cinephile logged in
                CinephilesController.Cinephile = cinephile;
                return RedirectToAction("CinephileMovies","Cinephiles");
            }
            else if (filmExpert != null && filmExpert.Password == password)
            {
                // film expert logged in
                return RedirectToAction("Film_ExpertMovies", "Film_Expert");
            }
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}