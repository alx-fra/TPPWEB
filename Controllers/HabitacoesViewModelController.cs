using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HabitAqui.Data;
using HabitAqui.ViewModels;

namespace HabitAqui.Controllers
{
    public class HabitacoesViewModelController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HabitacoesViewModelController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HabitacoesViewModel
        public async Task<IActionResult> Index()
        {
              return _context.HabitacaoViewModel != null ? 
                          View(await _context.HabitacaoViewModel.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.HabitacaoViewModel'  is null.");
        }

        // GET: HabitacoesViewModel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.HabitacaoViewModel == null)
            {
                return NotFound();
            }

            var habitacaoViewModel = await _context.HabitacaoViewModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (habitacaoViewModel == null)
            {
                return NotFound();
            }

            return View(habitacaoViewModel);
        }

        // GET: HabitacoesViewModel/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HabitacoesViewModel/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Localizacao,TipoHabitacao,CustoArrendamento,AvaliacaoLocador,Estado,CategoriaHabitacaoId,NomeLocador,ImagemUrl")] HabitacaoViewModel habitacaoViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(habitacaoViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(habitacaoViewModel);
        }

        // GET: HabitacoesViewModel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.HabitacaoViewModel == null)
            {
                return NotFound();
            }

            var habitacaoViewModel = await _context.HabitacaoViewModel.FindAsync(id);
            if (habitacaoViewModel == null)
            {
                return NotFound();
            }
            return View(habitacaoViewModel);
        }

        // POST: HabitacoesViewModel/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Localizacao,TipoHabitacao,CustoArrendamento,AvaliacaoLocador,Estado,CategoriaHabitacaoId,NomeLocador,ImagemUrl")] HabitacaoViewModel habitacaoViewModel)
        {
            if (id != habitacaoViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(habitacaoViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HabitacaoViewModelExists(habitacaoViewModel.Id))
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
            return View(habitacaoViewModel);
        }

        // GET: HabitacoesViewModel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.HabitacaoViewModel == null)
            {
                return NotFound();
            }

            var habitacaoViewModel = await _context.HabitacaoViewModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (habitacaoViewModel == null)
            {
                return NotFound();
            }

            return View(habitacaoViewModel);
        }

        // POST: HabitacoesViewModel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.HabitacaoViewModel == null)
            {
                return Problem("Entity set 'ApplicationDbContext.HabitacaoViewModel'  is null.");
            }
            var habitacaoViewModel = await _context.HabitacaoViewModel.FindAsync(id);
            if (habitacaoViewModel != null)
            {
                _context.HabitacaoViewModel.Remove(habitacaoViewModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HabitacaoViewModelExists(int id)
        {
          return (_context.HabitacaoViewModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
