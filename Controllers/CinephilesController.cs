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
    public class CinephilesController : Controller
    {
        private readonly Proyect_Rotten_TomatoesContext _context;

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
    }
}
