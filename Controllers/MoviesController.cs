using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyect_Rotten_Tomatoes.Data;
using Proyect_Rotten_Tomatoes.Models;

namespace Proyect_Rotten_Tomatoes.Controllers
{
    public class MoviesController : Controller
    {
        private readonly Proyect_Rotten_TomatoesContext _context;
        public IEnumerable<Movie> favoriteMovies { get; set; }

        public MoviesController(Proyect_Rotten_TomatoesContext context)
        {
            _context = context;
        }

        // GET: Movies
        public IActionResult Index(string orderBy)
        {
            IEnumerable<Movie> movies = _context.Movie;

            switch (orderBy)
            {
                case "CriticRating":
                    movies = movies.OrderByDescending(m => m.Critic_Rating);
                    break;
                case "AudienceRating":
                    movies = movies.OrderByDescending(m => m.Audience_Rating);
                    break;
                case "Genre":
                    movies = movies.OrderBy(m => m.Genre);
                    break;
                default:
                    break;
            }

            return View(movies);
        }

        public async Task<IActionResult> TopMovies()
        {
            return _context.Movie != null ?
                        View(await _context.Movie.ToListAsync()) :
                        Problem("Entity set 'Proyect_Rotten_TomatoesContext.Movie'  is null.");
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                ModelState.AddModelError("url", "Link is required."); // Add validation error to ModelState
                return View();
            }
            //try
            //{
                Movie movie = WebScraper.Get_Movie_data(url);
                if (await _context.Movie.SingleOrDefaultAsync(m => m.Title == movie.Title) != null)
                {
                    throw new Exception("Movie already exists");
                }
                _context.Add(movie);
            //}
            //catch (Exception ex)
            //{
            //    if (ex != null)
            //    {
            //        ModelState.AddModelError("url", ex.Message);
            //    }
            //    else
            //    {
            //        ModelState.AddModelError("url", "Try another link.");
            //    }
            //    return View();
            //}
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Movies");
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,url,Title,Image,Critic_Rating,Audience_Rating,Critic_Comments,Audience_Comments,Available_Platforms,Synopsis,Rating,Genre,Film_Team,Duration,Principal_Actors")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Movie == null)
            {
                return Problem("Entity set 'Proyect_Rotten_TomatoesContext.Movie'  is null.");
            }
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return (_context.Movie?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult UpdateSection()
        {
            // Code to update the section goes here

            var movies = _context.Movie.ToList();
            return RedirectToAction("TopMovies", "Movies");
        }

        [HttpPost]
        public async Task<IActionResult> Update()
        {
            var movies = _context.Movie.ToList();
            var topMovies = movies.OrderByDescending(m => m.Critic_Rating).Take(6);
            foreach (var movie in topMovies)
            {
                Task<Movie> updatedMovieTask = WebScraper.Get_Movie_dataAsync(movie.url, movie.Id, _context);
                Movie updatedMovie = updatedMovieTask.Result;
                _context.Update(updatedMovie);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("TopMovies", "Movies");
        }
    }
}
