using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Proyect_Rotten_Tomatoes.Data;
using Proyect_Rotten_Tomatoes.Models;

namespace Proyect_Rotten_Tomatoes.Controllers
{
    public class CinephilesController : Controller
    {
        private readonly Proyect_Rotten_TomatoesContext _context;
        public static Cinephile Cinephile { get; set; }

        public CinephilesController(Proyect_Rotten_TomatoesContext context)
        {
            _context = context;
        }

        // GET: Cinephiles
        public async Task<IActionResult> Index()
        {
            return _context.Cinephile != null ?
                        View(await _context.Cinephile.ToListAsync()) :
                        Problem("Entity set 'Proyect_Rotten_TomatoesContext.Cinephile'  is null.");
        }

        // GET: Cinephiles/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult CinephileMovies(string orderBy)
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

        public IActionResult CinephileSeries(string orderBy)
        {
            IEnumerable<Serie> series = _context.Serie;

            switch (orderBy)
            {
                case "CriticRating":
                    series = series.OrderByDescending(s => s.Critic_Rating);
                    break;
                case "AudienceRating":
                    series = series.OrderByDescending(s => s.Audience_Rating);
                    break;
                case "Genre":
                    series = series.OrderBy(s => s.Genre);
                    break;
                default:
                    break;
            }

            return View(series);
        }

        public async Task<IActionResult> CinephileTopMovies(string orderBy)
        {
            return _context.Movie != null ?
                        View(await _context.Movie.ToListAsync()) :
                        Problem("Entity set 'Proyect_Rotten_TomatoesContext.Movie'  is null.");
        }

        public async Task<IActionResult> CinephileTopSeries(string orderBy)
        {
            return _context.Serie != null ?
                        View(await _context.Serie.ToListAsync()) :
                        Problem("Entity set 'Proyect_Rotten_TomatoesContext.Movie'  is null.");
        }

        // POST: Cinephiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password")] Cinephile cinephile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cinephile);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(cinephile);
        }

        // GET: Cinephiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cinephile == null)
            {
                return NotFound();
            }

            var cinephile = await _context.Cinephile.FindAsync(id);
            if (cinephile == null)
            {
                return NotFound();
            }
            return View(cinephile);
        }

        // POST: Cinephiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password")] Cinephile cinephile)
        {
            if (id != cinephile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cinephile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CinephileExists(cinephile.Id))
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
            return View(cinephile);
        }

        // GET: Cinephiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cinephile == null)
            {
                return NotFound();
            }

            var cinephile = await _context.Cinephile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cinephile == null)
            {
                return NotFound();
            }

            return View(cinephile);
        }

        // POST: Cinephiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cinephile == null)
            {
                return Problem("Entity set 'Proyect_Rotten_TomatoesContext.Cinephile'  is null.");
            }
            var cinephile = await _context.Cinephile.FindAsync(id);
            if (cinephile != null)
            {
                _context.Cinephile.Remove(cinephile);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CinephileExists(int id)
        {
          return (_context.Cinephile?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        public IActionResult Favourites(string orderBy)
        {
            List<Movie> favoriteMovies = _context.FavouriteMovies
                .Where(f => f.CinephileId == Cinephile.Id)
                .Include(f => f.Movie)
                .Select(f => f.Movie)
                .ToList();

            List<Serie> favoriteSeries = _context.FavouriteSeries
                .Where(f => f.CinephileId == Cinephile.Id)
                .Include(f => f.Serie)
                .Select(f => f.Serie)
                .ToList();

            switch (orderBy)
            {
                case "CriticRating":
                    favoriteMovies = favoriteMovies.OrderByDescending(m => m.Critic_Rating).ToList();
                    favoriteSeries = favoriteSeries.OrderByDescending(s => s.Critic_Rating).ToList();
                    break;
                case "AudienceRating":
                    favoriteMovies = favoriteMovies.OrderByDescending(m => m.Audience_Rating).ToList();
                    favoriteSeries = favoriteSeries.OrderByDescending(s => s.Audience_Rating).ToList();
                    break;
                case "Genre":
                    favoriteMovies = favoriteMovies.OrderByDescending(m => m.Genre).ToList();
                    favoriteSeries = favoriteSeries.OrderByDescending(s => s.Genre).ToList();
                    break;
                default:
                    break;
            }

            Favorites favorites = new()
            {
                Movies = favoriteMovies,
                Series = favoriteSeries
            };

            return View(favorites);
        }


        [HttpPost]
        public async Task<IActionResult> AddFavoriteMovie(string movie)
        {
            var movieA = JsonConvert.DeserializeObject<Movie>(movie);
            bool favoriteExists = await _context.FavouriteMovies
                .AnyAsync(f => f.CinephileId == Cinephile.Id && f.MovieId == movieA.Id);

            if (!favoriteExists)
            {
                FavouriteMovies favouriteMovie = new()
                {
                CinephileId = Cinephile.Id,
                MovieId = movieA.Id
                };
                _context.FavouriteMovies.Add(favouriteMovie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("CinephileMovies", "Cinephiles");
        
        }

        [HttpPost]
        public async Task<IActionResult> AddFavoriteSerie(string serie)
        {
            var serieA = JsonConvert.DeserializeObject<Serie>(serie);
            bool favoriteExists = await _context.FavouriteSeries
                .AnyAsync(f => f.CinephileId == Cinephile.Id && f.SerieId == serieA.Id);

            if (!favoriteExists)
            {
                FavouriteSeries favouriteSerie = new()
                {
                    CinephileId = Cinephile.Id,
                    SerieId = serieA.Id
                };
                _context.FavouriteSeries.Add(favouriteSerie);
                await _context.SaveChangesAsync();

            }
            return RedirectToAction("CinephileSeries", "Cinephiles");

        }
    }
}
