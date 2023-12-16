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
    public class DanosHabitacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DanosHabitacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DanosHabitacoes
        public async Task<IActionResult> Index()
        {
              return _context.danosHabitacaes != null ? 
                          View(await _context.danosHabitacaes.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.danosHabitacaes'  is null.");
        }

        // GET: DanosHabitacoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.danosHabitacaes == null)
            {
                return NotFound();
            }

            var danosHabitacao = await _context.danosHabitacaes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (danosHabitacao == null)
            {
                return NotFound();
            }

            return View(danosHabitacao);
        }

        // GET: DanosHabitacoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DanosHabitacoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DescricaoDanos,imagem")] DanosHabitacao danosHabitacao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(danosHabitacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(danosHabitacao);
        }

        // GET: DanosHabitacoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.danosHabitacaes == null)
            {
                return NotFound();
            }

            var danosHabitacao = await _context.danosHabitacaes.FindAsync(id);
            if (danosHabitacao == null)
            {
                return NotFound();
            }
            return View(danosHabitacao);
        }

        // POST: DanosHabitacoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DescricaoDanos,imagem")] DanosHabitacao danosHabitacao)
        {
            if (id != danosHabitacao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(danosHabitacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanosHabitacaoExists(danosHabitacao.Id))
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
            return View(danosHabitacao);
        }

        // GET: DanosHabitacoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.danosHabitacaes == null)
            {
                return NotFound();
            }

            var danosHabitacao = await _context.danosHabitacaes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (danosHabitacao == null)
            {
                return NotFound();
            }

            return View(danosHabitacao);
        }

        // POST: DanosHabitacoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.danosHabitacaes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.danosHabitacaes'  is null.");
            }
            var danosHabitacao = await _context.danosHabitacaes.FindAsync(id);
            if (danosHabitacao != null)
            {
                _context.danosHabitacaes.Remove(danosHabitacao);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DanosHabitacaoExists(int id)
        {
          return (_context.danosHabitacaes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
