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
    public class ObservacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ObservacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Observacoes
        public async Task<IActionResult> Index()
        {
              return _context.observacoes != null ? 
                          View(await _context.observacoes.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.observacoes'  is null.");
        }

        // GET: Observacoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.observacoes == null)
            {
                return NotFound();
            }

            var observacao = await _context.observacoes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (observacao == null)
            {
                return NotFound();
            }

            return View(observacao);
        }

        // GET: Observacoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Observacoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TextoObservacoes")] Observacao observacao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(observacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(observacao);
        }

        // GET: Observacoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.observacoes == null)
            {
                return NotFound();
            }

            var observacao = await _context.observacoes.FindAsync(id);
            if (observacao == null)
            {
                return NotFound();
            }
            return View(observacao);
        }

        // POST: Observacoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TextoObservacoes")] Observacao observacao)
        {
            if (id != observacao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(observacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObservacaoExists(observacao.Id))
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
            return View(observacao);
        }

        // GET: Observacoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.observacoes == null)
            {
                return NotFound();
            }

            var observacao = await _context.observacoes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (observacao == null)
            {
                return NotFound();
            }

            return View(observacao);
        }

        // POST: Observacoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.observacoes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.observacoes'  is null.");
            }
            var observacao = await _context.observacoes.FindAsync(id);
            if (observacao != null)
            {
                _context.observacoes.Remove(observacao);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ObservacaoExists(int id)
        {
          return (_context.observacoes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
