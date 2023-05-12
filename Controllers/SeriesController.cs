using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyect_Rotten_Tomatoes.Data;
using Proyect_Rotten_Tomatoes.Models;

namespace Proyect_Rotten_Tomatoes.Controllers
{
    public class SeriesController : Controller
    {
        private readonly Proyect_Rotten_TomatoesContext _context;

        public SeriesController(Proyect_Rotten_TomatoesContext context)
        {
            _context = context;
        }

        // GET: Series
        public IActionResult Index(string orderBy)
        {
            IEnumerable<Serie> series = _context.Serie;

            switch (orderBy)
            {
                case "CriticRating":
                    series = series.OrderByDescending(m => m.Critic_Rating);
                    break;
                case "AudienceRating":
                    series = series.OrderByDescending(m => m.Audience_Rating);
                    break;
                case "Genre":
                    series = series.OrderBy(m => m.Genre);
                    break;
                default:
                    break;
            }

            return View(series);
        }

        // GET: Series/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Serie == null)
            {
                return NotFound();
            }

            var serie = await _context.Serie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serie == null)
            {
                return NotFound();
            }

            return View(serie);
        }

        // GET: Series/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Series/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                ModelState.AddModelError("url", "Link is required.");
                return View();
            }
            try
            {
                Serie serie = WebScraper.Get_Serie_data(url);
                if (await _context.Serie.SingleOrDefaultAsync(s => s.Title == serie.Title) != null)
                {
                    throw new Exception("Serie already exists");
                }
                _context.Add(serie);
            }
            catch (Exception ex)
            {
                if (ex.Message.Length < 22)
                {
                    ModelState.AddModelError("url", ex.Message);
                }
                else
                {
                    ModelState.AddModelError("url", "Try another link.");
                }
                return View();
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Series");
        }

            // GET: Series/Edit/5
            public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Serie == null)
            {
                return NotFound();
            }

            var serie = await _context.Serie.FindAsync(id);
            if (serie == null)
            {
                return NotFound();
            }
            return View(serie);
        }

        // POST: Series/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,url,Title,Image,Critic_Rating,Audience_Rating,Available_Platforms,Synopsis,Rating,Genre,Serie_Team,Duration,Principal_Actors")] Serie serie)
        {
            if (id != serie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SerieExists(serie.Id))
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
            return View(serie);
        }

        // GET: Series/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Serie == null)
            {
                return NotFound();
            }

            var serie = await _context.Serie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serie == null)
            {
                return NotFound();
            }

            return View(serie);
        }

        // POST: Series/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Serie == null)
            {
                return Problem("Entity set 'Proyect_Rotten_TomatoesContext.Serie'  is null.");
            }
            var serie = await _context.Serie.FindAsync(id);
            if (serie != null)
            {
                _context.Serie.Remove(serie);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SerieExists(int id)
        {
          return (_context.Serie?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> TopSeries()
        {
            return _context.Serie != null ?
                        View(await _context.Serie.ToListAsync()) :
                        Problem("Entity set 'Proyect_Rotten_TomatoesContext.Serie'  is null.");
        }

        public IActionResult UpdateSection()
        {
            // Code to update the section goes here

            var series = _context.Serie.ToList();
            return RedirectToAction("TopSeries", "Series");
        }

        [HttpPost]
        public async Task<IActionResult> Update()
        {
            var series = _context.Serie.ToList();
            var topSeries = series.OrderByDescending(s => s.Critic_Rating).Take(6);
            foreach (var serie in topSeries)
            {
                Task<Serie> updatedSerieTask = WebScraper.Get_Serie_dataAsync(serie.url, serie.Id, _context);
                Serie updatedSerie = updatedSerieTask.Result;
                _context.Serie.Update(updatedSerie);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("TopSeries", "Series");
        }
    }
}
