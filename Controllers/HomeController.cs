using HabitAqui.Data;
using HabitAqui.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace HabitAqui.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController   (ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;

        }

        public async Task<IActionResult> Index(string sortOrder,string aval, [FromQuery(Name = "Cat")] string categoriaSelecionada, [FromQuery(Name = "Loc")] string Loc, string SearchTerm, string SearchCategory)
{
    if (_context.habitacoes != null)
    {
                var categorias = _context.categoriaHabitacoes.ToList();

                var locadores = _context.locadores.ToList();

                ViewBag.CategoriaHabitacoes = new SelectList(categorias, "Id", "NomeCategoria");
                ViewBag.Locadores = new SelectList(locadores, "Id", "Nome");

                var resultadoLocadores = (from hab in _context.habitacoes
                                  join locador in _context.locadores on hab.LocadorId equals locador.Id
                                  join cat in _context.categoriaHabitacoes on hab.CategoriaHabitacaoId equals cat.Id
                                  where hab.Estado.Equals("Disponivel")
                                  select new
                                  {
                                      hab.Id,
                                      hab.ImagemUrl,
                                      hab.TipoHabitacao,
                                      hab.Localizacao,
                                      hab.CustoArrendamento,
                                      hab.Estado,
                                      locador.Nome,
                                      locador.AvaliacaoLocador,
                                      cat.NomeCategoria
                                  });
                ViewBag.PrecoSort = String.IsNullOrEmpty(sortOrder) ? "asc" : (sortOrder == "asc" ? "desc" : "");
                ViewBag.AvalSort = String.IsNullOrEmpty(aval) ? "aval_asc" : (aval == "aval_asc" ? "aval_desc" : "");


                switch (sortOrder)
{
    case "asc":
        resultadoLocadores = resultadoLocadores.OrderBy(h => h.CustoArrendamento);
        break;
    case "desc":
        resultadoLocadores = resultadoLocadores.OrderByDescending(h => h.CustoArrendamento);
        break;
    default:
        // Nenhum parâmetro ou valor desconhecido, mantenha a ordem padrão
        break;
}
                switch (aval)
                {
                    case "aval_asc":
                        resultadoLocadores = resultadoLocadores.OrderBy(h => h.AvaliacaoLocador);
                        break;
                    case "aval_desc":
                        resultadoLocadores = resultadoLocadores.OrderByDescending(h => h.AvaliacaoLocador);
                        break;
                    default:
                        // Nenhum parâmetro ou valor desconhecido, mantenha a ordem padrão
                        break;
                }

                if (!string.IsNullOrEmpty(categoriaSelecionada))
                {
                    resultadoLocadores = resultadoLocadores.Where(h => h.NomeCategoria == categoriaSelecionada);
                }
                if (!string.IsNullOrEmpty(Loc))
                {
                    resultadoLocadores = resultadoLocadores.Where(h => h.Nome == Loc);
                }
                if (!string.IsNullOrEmpty(SearchCategory))
                {
                    switch (SearchCategory)
                    {
                        case "Local":
                            resultadoLocadores = resultadoLocadores.Where(h => h.Localizacao == SearchTerm);

                            break;
                        case "Habit":
                            resultadoLocadores = resultadoLocadores.Where(h => h.TipoHabitacao == SearchTerm);

                            break;
                        default:
                            break;
                    }
                }
                ViewBag.ResultadoLocadores = await resultadoLocadores.ToListAsync();

        return View();
    }
    else
    {
        return Problem("Entity set 'ApplicationDbContext.habitacoes' is null.");
    }
}


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
    }
}