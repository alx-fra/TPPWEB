using HabitAqui.Data;
using HabitAqui.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using System.Threading.Tasks;

[Authorize(Roles = "admin")]
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IActionResult Admin()
    {
        return View();
    }
    public async Task<IActionResult> ListaUsr()
    {
        if (!_userManager.Users.Any())
        {
            TempData["ErrorMessage"] = "Não existem utilizadores criados.";
            return RedirectToAction("Gestor");
        }

        var users = await _userManager.Users
            .Select(u => new UserViewModel
            {
                IdUser = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Roles = new List<string>(),
                inativo = u.LockoutEnabled
            })
            .ToListAsync();

        foreach (var user in users)
        {
            user.Roles = await _userManager.GetRolesAsync(_userManager.Users.FirstOrDefault(u => u.Id == user.IdUser));
        }

        return View(users);
    }

    public async Task<IActionResult> EditUsr(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        var roles = await _userManager.GetRolesAsync(user);

        ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();

        var userViewModel = new UserViewModel
        {
            IdUser = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Roles = roles.ToList(),
            inativo = user.LockoutEnabled
        };

        return View(userViewModel);
    }


    [HttpPost]
    public async Task<IActionResult> GuardarEdicao(string id, UserViewModel userViewModel)
    {
        try
        {
            var existingUser = await _userManager.FindByIdAsync(id);

            if (existingUser != null)
            {
                existingUser.UserName = userViewModel.UserName;
                existingUser.Email = userViewModel.Email;
                existingUser.PhoneNumber = userViewModel.PhoneNumber;

                await _userManager.UpdateAsync(existingUser);

                var userRoles = await _userManager.GetRolesAsync(existingUser);

                var rolesToAdd = userViewModel.Roles.Except(userRoles);
                var rolesToRemove = userRoles.Except(userViewModel.Roles);

                foreach (var roleToAdd in rolesToAdd)
                {
                    if (!await _userManager.IsInRoleAsync(existingUser, roleToAdd))

                    {
                        try
                        {
                            await _userManager.AddToRoleAsync(existingUser, roleToAdd);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Exception: {ex.Message}, Role: {roleToAdd}");
                        }
                    }
                }

                foreach (var roleToRemove in rolesToRemove)
                {
                    await _userManager.RemoveFromRoleAsync(existingUser, roleToRemove);
                }

                return RedirectToAction("ListaUsr");
            }
            else
            {
                return NotFound();
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return RedirectToAction("ListaUsr");
        }
    }



    public async Task<IActionResult> Ativar(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        user.LockoutEnabled = false;
        await _userManager.UpdateAsync(user);
        return RedirectToAction("ListaUsr");
    }

    public async Task<IActionResult> Inativar(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        user.LockoutEnabled = true;
        await _userManager.UpdateAsync(user);
        return RedirectToAction("ListaUsr");
    }

    public async Task<IActionResult> ListaLoc(string sortOrder)
    {
        if (!_context.locadores.Any())
        {
            TempData["ErrorMessage"] = "Não existem Locadores criados.";
            return RedirectToAction("Admin");
        }

        ViewBag.NomeSortParam = String.IsNullOrEmpty(sortOrder) ? "nome_desc" : "";
        ViewBag.EstadoSortParam = sortOrder == "estado" ? "estado_desc" : "estado";

        IQueryable<Locador> locadoresQuery = _context.locadores;

        switch (sortOrder)
        {
            case "nome_desc":
                locadoresQuery = locadoresQuery.OrderByDescending(l => l.Nome);
                break;
            case "estado":
                locadoresQuery = locadoresQuery.OrderBy(l => l.EstadoSubscricao);
                break;
            case "estado_desc":
                locadoresQuery = locadoresQuery.OrderByDescending(l => l.EstadoSubscricao);
                break;
            default:
                locadoresQuery = locadoresQuery.OrderBy(l => l.Nome);
                break;
        }

        var locadores = await locadoresQuery.ToListAsync();

        return View(locadores);
    }

    // GET:
    public IActionResult CreateLoc()
    {
        return View();
    }

    // POST:
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateLoc([Bind("Id,Nome,EstadoSubscricao")] Locador locador)
    {
        if (ModelState.IsValid)
        {
            _context.Add(locador);
            await _context.SaveChangesAsync();

            var user = new IdentityUser
            {
                //utilizador temporario pass==mail
                UserName = "gestor@" + locador.Nome + ".pt",//default
                Email = "gestor@" + locador.Nome + ".pt",
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = false,
                AccessFailedCount = 0,

            };

            //PalavraPass temporaria, Pass="PassTemp-1"
            var result = await _userManager.CreateAsync(user, "PassTemp-1");

            if (result.Succeeded)
            {

                var recursoHumano = new RecursoHumano
                {
                    IdRecHum = user.Id,
                    RoleNoLocador = "gestor",
                    LocadorId = locador.Id
                };

                _context.recursoshumanos.Add(recursoHumano);
                await _context.SaveChangesAsync();

                // Verifica se a função já existe; se não, a cria
                if (!await _roleManager.RoleExistsAsync("gestor"))
                {
                    var role = new IdentityRole { Name = "gestor" };
                    await _roleManager.CreateAsync(role);
                }

                if (user != null)
                {
                    await _userManager.AddToRoleAsync(user, "gestor");
                }
                TempData["SuccessMessage"] = "Gestor adicionado com sucesso com os dados:" + System.Environment.NewLine + "Email/UserName:" + user.UserName + System.Environment.NewLine + "PassWord: PassTemp-1";
                return RedirectToAction("ListaLoc");


            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            TempData["SuccessMessage"] = "Existe um gestor com os dados:" + System.Environment.NewLine + "Email/UserName:" + user.UserName + System.Environment.NewLine + "PassWord: PassTemp-1";
            return RedirectToAction(nameof(ListaLoc));
        }

        return View(locador);
    }

    // GET: 
    public async Task<IActionResult> DeleteLoc(int? id)
    {
        if (id == null || _context.locadores == null)
        {
            return NotFound();
        }

        var locador = await _context.locadores
            .FirstOrDefaultAsync(m => m.Id == id);
        if (locador == null)
        {
            return NotFound();
        }

        return View(locador);
    }

    // POST: 
    [HttpPost, ActionName("DeleteLoc")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.locadores == null)
        {
            return Problem("Entity set 'ApplicationDbContext.locadores' is null.");
        }

        var locador = await _context.locadores.FindAsync(id);
        if (locador == null)
        {
            return NotFound();
        }

        bool hasHabitacoes = await _context.habitacoes.AnyAsync(h => h.Locador.Id == id);

        if (hasHabitacoes)
        {
            TempData["ErrorMessage"] = "Não é possível excluir o locador já que existem habitações atribuídas.";
            return RedirectToAction(nameof(ListaLoc));
        }

        _context.locadores.Remove(locador);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ListaLoc));
    }
    /*
        //Apaga user (apenas usado depois de apagar locador) para apagar users associados
        public async Task<bool> ApagarId(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser != null && currentUser.Id == userId)
                {
                    TempData["ErrorMessage"] = "Não é possível excluir o seu próprio registo.";
                    return false;
                }

                if (_context == null)
                {
                    throw new InvalidOperationException("_context não foi inicializado corretamente.");
                }

                bool hasArrendamentos = await _context.arrendamentos.AnyAsync(a => a.UserId == userId);

                if (hasArrendamentos)
                {
                    TempData["ErrorMessage"] = "Não é possível excluir o utilizador já que existem arrendamentos atribuídos.";
                    return false;
                }


                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    // Apagar as roles nos recursos humanos
                    var recursoHumano = await _context.recursoshumanos.FirstOrDefaultAsync(rh => rh.IdRecHum == userId);

                    if (recursoHumano != null)
                    {
                        _context.recursoshumanos.Remove(recursoHumano);
                        await _context.SaveChangesAsync();
                    }
                }

                return result.Succeeded;
            }

            return false;
        }
    */

    private bool LocadorExists(int id)
    {
        return (_context.locadores?.Any(e => e.Id == id)).GetValueOrDefault();
    }

    // GET: 
    public async Task<IActionResult> EditLoc(int? id)
    {
        if (id == null || _context.locadores == null)
        {
            return NotFound();
        }

        var locador = await _context.locadores.FindAsync(id);
        if (locador == null)
        {
            return NotFound();
        }
        return View(locador);
    }

    // POST:
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditLoc(int id, [Bind("Id,Nome,EstadoSubscricao")] Locador locador)
    {
        if (id != locador.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(locador);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocadorExists(locador.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(ListaLoc));
        }
        return View(locador);
    }


    // GET:
    public async Task<IActionResult> ListaCat()
    {
        if (!_context.categoriaHabitacoes.Any())
        {
            TempData["ErrorMessage"] = "Não existem Categorias de Habitações criadas.";
            return RedirectToAction("Admin");
        }

        return _context.categoriaHabitacoes != null ?
                    View(await _context.categoriaHabitacoes.ToListAsync()) :
                    Problem("Entity set 'ApplicationDbContext.categoriaHabitacoes'  is null.");
    }

    // GET:
    public async Task<IActionResult> DetailsCat(int? id)
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

    // GET: 
    public IActionResult CreateCat()
    {
        return View();
    }

    // POST:
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCat([Bind("Id,NomeCategoria,ativo")] CategoriaHabitacao categoriaHabitacao)
    {
        if (ModelState.IsValid)
        {
            _context.Add(categoriaHabitacao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListaCat));
        }
        return View(categoriaHabitacao);
    }

    // GET: 
    public async Task<IActionResult> EditCat(int? id)
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

    // POST:
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCat(int id, [Bind("Id,NomeCategoria,ativo")] CategoriaHabitacao categoriaHabitacao)
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
            return RedirectToAction(nameof(ListaCat));
        }
        return View(categoriaHabitacao);
    }

    private bool CategoriaHabitacaoExists(int id)
    {
        return (_context.categoriaHabitacoes?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}