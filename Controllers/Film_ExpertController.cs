using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyect_Rotten_Tomatoes.Data;
using Proyect_Rotten_Tomatoes.Models;

namespace Proyect_Rotten_Tomatoes.Controllers
{
    public class Film_ExpertController : Controller
    {
        private readonly Proyect_Rotten_TomatoesContext _context;

        public Film_ExpertController(Proyect_Rotten_TomatoesContext context)
        {
            _context = context;
        }

        // GET: Film_Expert
        public async Task<IActionResult> Index()
        {
            return _context.Film_Expert != null ?
                        View(await _context.Film_Expert.ToListAsync()) :
                        Problem("Entity set 'Proyect_Rotten_TomatoesContext.Film_Expert'  is null.");
        }

        // GET: Film_Expert/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Film_Expert/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password")] Film_Expert film_Expert)
        {
            if (ModelState.IsValid)
            {
                _context.Add(film_Expert);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(film_Expert);
        }

        // GET: Film_Expert/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Film_Expert == null)
            {
                return NotFound();
            }

            var film_Expert = await _context.Film_Expert.FindAsync(id);
            if (film_Expert == null)
            {
                return NotFound();
            }
            return View(film_Expert);
        }

        // POST: Film_Expert/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password")] Film_Expert film_Expert)
        {
            if (id != film_Expert.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(film_Expert);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Film_ExpertExists(film_Expert.Id))
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
            return View(film_Expert);
        }

        // GET: Film_Expert/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Film_Expert == null)
            {
                return NotFound();
            }

            var film_Expert = await _context.Film_Expert
                .FirstOrDefaultAsync(m => m.Id == id);
            if (film_Expert == null)
            {
                return NotFound();
            }

            return View(film_Expert);
        }

        // POST: Film_Expert/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Film_Expert == null)
            {
                return Problem("Entity set 'Proyect_Rotten_TomatoesContext.Film_Expert'  is null.");
            }
            var film_Expert = await _context.Film_Expert.FindAsync(id);
            if (film_Expert != null)
            {
                _context.Film_Expert.Remove(film_Expert);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Film_ExpertExists(int id)
        {
          return (_context.Film_Expert?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
