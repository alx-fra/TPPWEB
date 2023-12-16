using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HabitAqui.Data;

namespace HabitAqui.Controllers
{
    public class LocadoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocadoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Locadores
        public async Task<IActionResult> Index()
        {
              return _context.locadores != null ? 
                          View(await _context.locadores.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.locadores'  is null.");
        }

        // GET: Locadores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.locadores == null)
            {
                return NotFound();
            }

            var locador = await _context.locadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (locador == null)
            {
                return NotFound();
            }

            return View(locador);
        }

        // GET: Locadores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locadores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,EstadoSubscricao")] Locador locador)
        {
            if (ModelState.IsValid)
            {
                _context.Add(locador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(locador);
        }

        // GET: Locadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.locadores == null)
            {
                return NotFound();
            }

            var locador = await _context.locadores.FindAsync(id);
            if (locador == null)
            {
                return NotFound();
            }
            return View(locador);
        }

        // POST: Locadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,EstadoSubscricao")] Locador locador)
        {
            if (id != locador.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(locador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocadorExists(locador.Id))
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
            return View(locador);
        }

        // GET: Locadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.locadores == null)
            {
                return NotFound();
            }

            var locador = await _context.locadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (locador == null)
            {
                return NotFound();
            }

            return View(locador);
        }

        // POST: Locadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.locadores == null)
            {
                return Problem("Entity set 'ApplicationDbContext.locadores'  is null.");
            }
            var locador = await _context.locadores.FindAsync(id);
            if (locador != null)
            {
                _context.locadores.Remove(locador);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocadorExists(int id)
        {
          return (_context.locadores?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
