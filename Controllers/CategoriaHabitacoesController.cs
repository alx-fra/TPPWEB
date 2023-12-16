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
    public class CategoriaHabitacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriaHabitacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CategoriaHabitacoes
        public async Task<IActionResult> Index()
        {
              return _context.categoriaHabitacoes != null ? 
                          View(await _context.categoriaHabitacoes.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.categoriaHabitacoes'  is null.");
        }

        // GET: CategoriaHabitacoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.categoriaHabitacoes == null)
            {
                return NotFound();
            }

            var categoriaHabitacao = await _context.categoriaHabitacoes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoriaHabitacao == null)
            {
                return NotFound();
            }

            return View(categoriaHabitacao);
        }

        // GET: CategoriaHabitacoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriaHabitacoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomeCategoria,ativo")] CategoriaHabitacao categoriaHabitacao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoriaHabitacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaHabitacao);
        }

        // GET: CategoriaHabitacoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.categoriaHabitacoes == null)
            {
                return NotFound();
            }

            var categoriaHabitacao = await _context.categoriaHabitacoes.FindAsync(id);
            if (categoriaHabitacao == null)
            {
                return NotFound();
            }
            return View(categoriaHabitacao);
        }

        // POST: CategoriaHabitacoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomeCategoria,ativo")] CategoriaHabitacao categoriaHabitacao)
        {
            if (id != categoriaHabitacao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriaHabitacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaHabitacaoExists(categoriaHabitacao.Id))
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
            return View(categoriaHabitacao);
        }

        // GET: CategoriaHabitacoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.categoriaHabitacoes == null)
            {
                return NotFound();
            }

            var categoriaHabitacao = await _context.categoriaHabitacoes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoriaHabitacao == null)
            {
                return NotFound();
            }

            return View(categoriaHabitacao);
        }

        // POST: CategoriaHabitacoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.categoriaHabitacoes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.categoriaHabitacoes'  is null.");
            }
            var categoriaHabitacao = await _context.categoriaHabitacoes.FindAsync(id);
            if (categoriaHabitacao != null)
            {
                _context.categoriaHabitacoes.Remove(categoriaHabitacao);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaHabitacaoExists(int id)
        {
          return (_context.categoriaHabitacoes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
