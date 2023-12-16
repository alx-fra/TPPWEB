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
    public class RecursosHumanosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecursosHumanosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RecursosHumanos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.recursoshumanos.Include(r => r.Locador);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: RecursosHumanos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.recursoshumanos == null)
            {
                return NotFound();
            }

            var recursoHumano = await _context.recursoshumanos
                .Include(r => r.Locador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recursoHumano == null)
            {
                return NotFound();
            }

            return View(recursoHumano);
        }

        // GET: RecursosHumanos/Create
        public IActionResult Create()
        {
            ViewData["LocadorId"] = new SelectList(_context.locadores, "Id", "Id");
            return View();
        }

        // POST: RecursosHumanos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdRecHum,RoleNoLocador,LocadorId")] RecursoHumano recursoHumano)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recursoHumano);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LocadorId"] = new SelectList(_context.locadores, "Id", "Id", recursoHumano.LocadorId);
            return View(recursoHumano);
        }

        // GET: RecursosHumanos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.recursoshumanos == null)
            {
                return NotFound();
            }

            var recursoHumano = await _context.recursoshumanos.FindAsync(id);
            if (recursoHumano == null)
            {
                return NotFound();
            }
            ViewData["LocadorId"] = new SelectList(_context.locadores, "Id", "Id", recursoHumano.LocadorId);
            return View(recursoHumano);
        }

        // POST: RecursosHumanos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdRecHum,RoleNoLocador,LocadorId")] RecursoHumano recursoHumano)
        {
            if (id != recursoHumano.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recursoHumano);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecursoHumanoExists(recursoHumano.Id))
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
            ViewData["LocadorId"] = new SelectList(_context.locadores, "Id", "Id", recursoHumano.LocadorId);
            return View(recursoHumano);
        }

        // GET: RecursosHumanos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.recursoshumanos == null)
            {
                return NotFound();
            }

            var recursoHumano = await _context.recursoshumanos
                .Include(r => r.Locador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recursoHumano == null)
            {
                return NotFound();
            }

            return View(recursoHumano);
        }

        // POST: RecursosHumanos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.recursoshumanos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.recursoshumanos'  is null.");
            }
            var recursoHumano = await _context.recursoshumanos.FindAsync(id);
            if (recursoHumano != null)
            {
                _context.recursoshumanos.Remove(recursoHumano);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecursoHumanoExists(int id)
        {
          return (_context.recursoshumanos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
