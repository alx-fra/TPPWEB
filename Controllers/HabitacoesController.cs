using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HabitAqui.Data;
    public class HabitacoesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        
        public HabitacoesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Habitacoes
        public async Task<IActionResult> Index()
        {
            return _context.habitacoes != null ?
                        View(await _context.habitacoes.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.habitacoes'  is null.");
        }

        // GET: Habitacoes/Details/5
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

            // Disponibilizar a lista de categorias para a View usando a ViewBag
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

        // GET: Habitacoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Habitacoes/Delete/5
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
