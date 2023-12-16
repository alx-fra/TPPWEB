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
    public class EquipamentosOpcionaisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EquipamentosOpcionaisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EquipamentosOpcionais
        public async Task<IActionResult> Index()
        {
              return _context.equipamentosOpcional != null ? 
                          View(await _context.equipamentosOpcional.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.equipamentosOpcional'  is null.");
        }

        // GET: EquipamentosOpcionais/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.equipamentosOpcional == null)
            {
                return NotFound();
            }

            var equipamentoOpcional = await _context.equipamentosOpcional
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipamentoOpcional == null)
            {
                return NotFound();
            }

            return View(equipamentoOpcional);
        }

        // GET: EquipamentosOpcionais/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EquipamentosOpcionais/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomeEquipamento")] EquipamentoOpcional equipamentoOpcional)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equipamentoOpcional);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(equipamentoOpcional);
        }

        // GET: EquipamentosOpcionais/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.equipamentosOpcional == null)
            {
                return NotFound();
            }

            var equipamentoOpcional = await _context.equipamentosOpcional.FindAsync(id);
            if (equipamentoOpcional == null)
            {
                return NotFound();
            }
            return View(equipamentoOpcional);
        }

        // POST: EquipamentosOpcionais/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomeEquipamento")] EquipamentoOpcional equipamentoOpcional)
        {
            if (id != equipamentoOpcional.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipamentoOpcional);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipamentoOpcionalExists(equipamentoOpcional.Id))
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
            return View(equipamentoOpcional);
        }

        // GET: EquipamentosOpcionais/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.equipamentosOpcional == null)
            {
                return NotFound();
            }

            var equipamentoOpcional = await _context.equipamentosOpcional
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipamentoOpcional == null)
            {
                return NotFound();
            }

            return View(equipamentoOpcional);
        }

        // POST: EquipamentosOpcionais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.equipamentosOpcional == null)
            {
                return Problem("Entity set 'ApplicationDbContext.equipamentosOpcional'  is null.");
            }
            var equipamentoOpcional = await _context.equipamentosOpcional.FindAsync(id);
            if (equipamentoOpcional != null)
            {
                _context.equipamentosOpcional.Remove(equipamentoOpcional);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EquipamentoOpcionalExists(int id)
        {
          return (_context.equipamentosOpcional?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
