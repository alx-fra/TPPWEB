using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HabitAqui.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HabitAqui.Controllers
{
    public class ArrendamentosController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ArrendamentosController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Arrendamentos/Index
        public async Task<IActionResult> Arrendamentos()
        {
            var userId = _userManager.GetUserId(User);

       
            var arrendamentos = _context.arrendamentos
                .Where(a => a.UserId == userId)
                .ToList();

            return View(arrendamentos);
        }
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.arrendamentos.Include(a => a.AspNetUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Arrendamentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.arrendamentos == null)
            {
                return NotFound();
            }

            var arrendamento = await _context.arrendamentos
                .Include(a => a.AspNetUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arrendamento == null)
            {
                return NotFound();
            }

            return View(arrendamento);
        }

        // GET: Arrendamentos/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Arrendamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DataInicioContrato,DataFimContrato,CustoArrendamento,AvaliacaoLocador,estado,UserId")] Arrendamento arrendamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(arrendamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", arrendamento.UserId);
            return View(arrendamento);
        }

        // GET: Arrendamentos/Edit/5
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
            return View(arrendamento);
        }

        public async Task<IActionResult> EditarAval(int? id)
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
            return View(arrendamento);
        }

        public async Task<IActionResult> Detalhes(int? id)
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

            var detalhes = (from arr in _context.arrendamentos
                            join hab in _context.habitacoes on arr.Habitacao.Id equals hab.Id
                                      join locador in _context.locadores on hab.LocadorId equals locador.Id
                                      join cat in _context.categoriaHabitacoes on hab.CategoriaHabitacaoId equals cat.Id
                                      where arr.Id.Equals(id)
                                      select new
                                      {
                                          arr.DataInicioContrato,
                                          arr.DataFimContrato,
                                          arr.CustoArrendamento,
                                          arr.Observacoes,
                                          arr.Avaliacao,
                                          hab.ImagemUrl,
                                          hab.TipoHabitacao,
                                          hab.Localizacao,
                                          hab.Estado,
                                          locador.Nome,
                                          locador.AvaliacaoLocador,
                                          cat.NomeCategoria
                                      });
            ViewBag.detalhes = await detalhes.ToListAsync();

            return View();
        }

        // POST: Arrendamentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataInicioContrato,DataFimContrato,CustoArrendamento,HabitacaoId,Habitacao,Avaliacao,estado,UserId")] Arrendamento arrendamento)
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
            return View(arrendamento);
        }

        [HttpPost]
        public async Task<IActionResult> Pedido(DateTime inicioContrato, DateTime fimContrato, int modelId)
        {
            var habitacao = await _context.habitacoes.FindAsync(modelId);

            if (habitacao == null)
            {
                return NotFound("Habitacao not found.");
            }

            try
            {
                // Create a new Arrendamento
                var arrendamento = new Arrendamento
                {
                    DataInicioContrato = inicioContrato,
                    DataFimContrato = fimContrato,
                    CustoArrendamento = habitacao.CustoArrendamento, // Set the cost based on the Habitacao
                    estado = "pendente",
                    Habitacao = habitacao,
                    HabitacaoId = habitacao.Id,
                    PeriodoMinimo = 0,
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
                };

                _context.Add(arrendamento);
                await _context.SaveChangesAsync();

                // You can return a success message or redirect to a success page
                return Ok("Arrendamento pedido com sucesso.");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return BadRequest($"Erro ao processar o pedido de arrendamento: {habitacao.CustoArrendamento}");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> EditarAval(int id, [Bind("Id,DataInicioContrato,DataFimContrato,CustoArrendamento,HabitacaoId,Habitacao,Avaliacao,estado,UserId")] Arrendamento arrendamento)
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
                return RedirectToAction(nameof(Arrendamentos));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", arrendamento.UserId);
            return View(arrendamento);
        }


        // GET: Arrendamentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.arrendamentos == null)
            {
                return NotFound();
            }

            var arrendamento = await _context.arrendamentos
                .Include(a => a.AspNetUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arrendamento == null)
            {
                return NotFound();
            }

            return View(arrendamento);
        }

        // POST: Arrendamentos/Delete/5
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
