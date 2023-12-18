using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HabitAqui.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace HabitAqui.Controllers
{
    [Authorize(Roles = "funcionario")]
    public class ArrendamentosLocadorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public ArrendamentosLocadorController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ArrendamentosLocador
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var locadorId = _context.recursoshumanos
             .Where(rh => rh.IdRecHum == userId)
             .Select(rh => rh.LocadorId)
             .FirstOrDefault();

            var arrendamentos = await _context.arrendamentos.Where(a => a.Habitacao!.LocadorId == locadorId).ToListAsync();

            return View(arrendamentos);
        }

        // GET: ArrendamentosLocador/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.arrendamentos == null)
            {
                return NotFound();
            }

            var arrendamento = await _context.arrendamentos
                .Include(a => a.AspNetUser)
                .Include(a => a.Habitacao)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arrendamento == null)
            {
                return NotFound();
            }

            return View(arrendamento);
        }

        // GET: ArrendamentosLocador/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["HabitacaoId"] = new SelectList(_context.habitacoes, "Id", "Id");
            return View();
        }

        // POST: ArrendamentosLocador/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DataInicioContrato,DataFimContrato,CustoArrendamento,estado,Avaliacao,HabitacaoId,PeriodoMinimo,UserId")] Arrendamento arrendamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(arrendamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", arrendamento.UserId);
            ViewData["HabitacaoId"] = new SelectList(_context.habitacoes, "Id", "Id", arrendamento.HabitacaoId);
            return View(arrendamento);
        }

        // GET: ArrendamentosLocador/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.arrendamentos == null)
            {
                return NotFound();
            }

            var arrendamento = await _context.arrendamentos.FindAsync(id);
            if (arrendamento == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", arrendamento.UserId);
            ViewData["HabitacaoId"] = new SelectList(_context.habitacoes, "Id", "Id", arrendamento.HabitacaoId);
            return View(arrendamento);
        }

        // POST: ArrendamentosLocador/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataInicioContrato,DataFimContrato,CustoArrendamento,estado,Avaliacao,HabitacaoId,PeriodoMinimo,UserId")] Arrendamento arrendamento)
        {
            if (id != arrendamento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(arrendamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArrendamentoExists(arrendamento.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", arrendamento.UserId);
            ViewData["HabitacaoId"] = new SelectList(_context.habitacoes, "Id", "Id", arrendamento.HabitacaoId);
            return View(arrendamento);
        }

        // GET: ArrendamentosLocador/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.arrendamentos == null)
            {
                return NotFound();
            }

            var arrendamento = await _context.arrendamentos
                .Include(a => a.AspNetUser)
                .Include(a => a.Habitacao)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arrendamento == null)
            {
                return NotFound();
            }

            return View(arrendamento);
        }

        // POST: ArrendamentosLocador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.arrendamentos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.arrendamentos'  is null.");
            }
            var arrendamento = await _context.arrendamentos.FindAsync(id);
            if (arrendamento != null)
            {
                _context.arrendamentos.Remove(arrendamento);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArrendamentoExists(int id)
        {
          return (_context.arrendamentos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
