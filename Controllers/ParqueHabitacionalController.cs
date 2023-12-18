using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HabitAqui.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;

namespace HabitAqui.Controllers
{
    [Authorize(Roles = "funcionario")]
    public class ParqueHabitacionalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ParqueHabitacionalController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> SortBy(string sortOrder)
        {
            var userId = _userManager.GetUserId(User);

            var locadorId = _context.recursoshumanos
             .Where(rh => rh.IdRecHum == userId)
             .Select(rh => rh.LocadorId)
             .FirstOrDefault();

            var habitacoes = await _context.habitacoes.Where(a => a.LocadorId == locadorId).ToListAsync();

            switch (sortOrder)
            {
                case "localizacao_asc":
                    habitacoes = habitacoes.OrderBy(habitacao => habitacao.Localizacao).ToList();
                    break;
                case "localizacao_desc":
                    habitacoes = habitacoes.OrderByDescending(habitacao => habitacao.Localizacao).ToList();
                    break;
                case "tipo_asc":
                    habitacoes = habitacoes.OrderBy(habitacao => habitacao.TipoHabitacao).ToList();
                    break;
                case "tipo_desc":
                    habitacoes = habitacoes.OrderByDescending(habitacao => habitacao.TipoHabitacao).ToList();
                    break;
                // Add more cases for other properties if needed
                default:
                    habitacoes = habitacoes.OrderBy(habitacao => habitacao.TipoHabitacao).ToList();
                    break;
            }

            return View(habitacoes);
        }


        [HttpPost]
        public async Task<IActionResult> Index(string? filter)
        {
            var userId = _userManager.GetUserId(User);

            var locadorId = _context.recursoshumanos
             .Where(rh => rh.IdRecHum == userId)
             .Select(rh => rh.LocadorId)
             .FirstOrDefault();


            if (string.IsNullOrEmpty(filter))
            {
                return RedirectToAction(nameof(Index));
            }

            var filteredResult = await _context.habitacoes.Where(habitacao => (habitacao.TipoHabitacao.Contains(filter) || habitacao.Estado.Equals(filter)) && habitacao.LocadorId == locadorId).ToListAsync();

            return View(filteredResult);
        }

        // GET: ParqueHabitacional
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var locadorId = _context.recursoshumanos
             .Where(rh => rh.IdRecHum == userId)
             .Select(rh => rh.LocadorId)
             .FirstOrDefault();

            var habitacoesLocador = await _context.habitacoes.Where(a => a.LocadorId == locadorId).ToListAsync();

            return _context.habitacoes != null ? 
                          View(habitacoesLocador) :
                          Problem("Entity set 'ApplicationDbContext.habitacoes'  is null.");
        }

        // GET: ParqueHabitacional/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.habitacoes == null)
            {
                return NotFound();
            }

            var habitacao = await _context.habitacoes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (habitacao == null)
            {
                return NotFound();
            }

            return View(habitacao);
        }

        // GET: Habitacoes/Create
        public IActionResult Create()
        {

            var categorias = _context.categoriaHabitacoes.ToList();

            var locadores = _context.locadores.ToList();

            // Disponibilizar a lista de categorias para a View usando a ViewBag
            ViewBag.CategoriaHabitacoes = new SelectList(categorias, "Id", "NomeCategoria");
            ViewBag.Locadores = new SelectList(locadores, "Id", "Nome");

            return View();
        }

        // POST: Habitacoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Habitacao habitacao)
        {
            if (ModelState.IsValid)
            {
                if (habitacao.ImagemFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "imagens");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + habitacao.ImagemFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        habitacao.ImagemFile.CopyTo(fileStream);
                        habitacao.ImagemUrl = uniqueFileName;
                    }
                }
                else
                {
                    habitacao.ImagemUrl = "NoImage.png";
                }

                _context.Add(habitacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var categorias = _context.categoriaHabitacoes.ToList();

            var locadores = _context.locadores.ToList();

            ViewBag.CategoriaHabitacoes = new SelectList(categorias, "Id", "NomeCategoria");
            ViewBag.Locadores = new SelectList(locadores, "Id", "Nome");
            return View(habitacao);
        }

        // GET: Habitacoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.habitacoes == null)
            {
                return NotFound();
            }

            var habitacao = await _context.habitacoes.FindAsync(id);
            if (habitacao == null)
            {
                return NotFound();
            }
            return View(habitacao);
        }



        // POST: Habitacoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Localizacao,TipoHabitacao,CustoArrendamento,AvaliacaoLocador,CategoriaHabitacaoId,LocadorId,Estado,ImagemUrl,ImagemFile")] Habitacao habitacao)
        {
            if (id != habitacao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (habitacao.ImagemFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "imagens");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + habitacao.ImagemFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        habitacao.ImagemFile.CopyTo(fileStream);
                        habitacao.ImagemUrl = uniqueFileName;
                    }
                }
                else
                {
                    var existingHabitacao = await _context.habitacoes.AsNoTracking().FirstOrDefaultAsync(h => h.Id == habitacao.Id);
                    habitacao.ImagemUrl = existingHabitacao.ImagemUrl;
                    habitacao.ImagemFile = null; // Optional: Set ImagemFile to null to avoid any unintended behavior
                }
                try
                {
                    _context.Update(habitacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HabitacaoExists(habitacao.Id))
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
            return View(habitacao);
        }


        // GET: ParqueHabitacional/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.habitacoes == null)
            {
                return NotFound();
            }

            if (_context.arrendamentos.Any(m => m.HabitacaoId == id))
            {
                return RedirectToAction(nameof(Index));
            };

            var habitacao = await _context.habitacoes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (habitacao == null)
            {
                return NotFound();
            }

            return View(habitacao);
        }

        // POST: ParqueHabitacional/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.habitacoes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.habitacoes'  is null.");
            }

            var habitacao = await _context.habitacoes.FindAsync(id);

            if (habitacao != null)
            {
                _context.habitacoes.Remove(habitacao);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HabitacaoExists(int id)
        {
          return (_context.habitacoes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
