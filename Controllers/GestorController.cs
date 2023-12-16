using HabitAqui.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HabitAqui.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using HabitAqui.Controllers;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "gestor")]
public class GestorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public GestorController(UserManager<IdentityUser> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _context = context;
        _roleManager = roleManager;
    }

    public IActionResult Gestor()
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
<<<<<<< HEAD
                Roles = new List<string>(),
=======
                Roles = new List<string>(), 
>>>>>>> 3f3738bf35ea9681ee4fa1e926fd74aa46043b83
                inativo = u.LockoutEnabled
            })
            .ToListAsync();
        foreach (var user in users)
        {
            user.Roles = await _userManager.GetRolesAsync(_userManager.Users.FirstOrDefault(u => u.Id == user.IdUser));
        }

        return View(users);
    }



    public async Task<IActionResult> ListaRecHum()
    {
        if (!_context.recursoshumanos.Any())
        {
            TempData["ErrorMessage"] = "Não existem Recursos Humanos criados.";
            return RedirectToAction("Gestor");
        }

        var recursosHumanos = await _context.recursoshumanos
            .Include(rh => rh.Locador)
            .Select(rh => new RecursoHumanoViewModel
            {
                IdRecHum = rh.IdRecHum,
                RoleNoLocador = rh.RoleNoLocador,
                UserName = _context.Users.Where(u => u.Id == rh.IdRecHum).Select(u => u.UserName).FirstOrDefault(),
                LocadorNome = rh.Locador != null ? rh.Locador.Nome : "N/A"
            })
            .ToListAsync();

        return recursosHumanos != null ?
            View(recursosHumanos) :
            Problem("Entity set 'ApplicationDbContext.recursoshumanos' is null.");
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




    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await ApagarId(id);

        if (success)
        {
            return RedirectToAction("ListaUsr");
        }

        return RedirectToAction("ListaUsr");
    }

    [HttpGet]
    public IActionResult CreateGest()
    {
        if (!_context.locadores.Any())
        {
            TempData["ErrorMessage"] = "Não existem locadores criados para associar ao gestor.";
            return RedirectToAction("Gestor");
        }

        ViewData["LocadorId"] = new SelectList(_context.locadores, "Id", "Nome");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateGest(CreateGestViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser
            {
                UserName = model.Email,//default
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = false,
                AccessFailedCount = 0
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {

                var recursoHumano = new RecursoHumano
                {
                    IdRecHum = user.Id,
                    RoleNoLocador = "gestor",
                    LocadorId = model.LocadorId
                };

                _context.recursoshumanos.Add(recursoHumano);
                await _context.SaveChangesAsync();

                // Verifica se a função já existe senão cria
                if (!await _roleManager.RoleExistsAsync("gestor"))
                {
                    var role = new IdentityRole { Name = "gestor" };
                    await _roleManager.CreateAsync(role);
                }

                // Atribui o user à função
                if (user != null)
                {
                    await _userManager.AddToRoleAsync(user, "gestor");
                    TempData["SuccessMessage"] = "Gestor adicionado com sucesso.";
                }

                return RedirectToAction("ListaUsr");


            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        TempData["ErrorMessage"] = "Formato do Email ou PassWord Inválidos";
        return CreateGest();
    }


    [HttpGet]
    public IActionResult CreateFunc()
    {
        if (!_context.locadores.Any())
        {
            TempData["ErrorMessage"] = "Não existem locadores criados para associar ao funcionário.";
            return RedirectToAction("gestor");
        }

        ViewData["LocadorId"] = new SelectList(_context.locadores, "Id", "Nome");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFunc(CreateFuncViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser
            {
                UserName = model.Email,//default
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = false,
                AccessFailedCount = 0
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {

                var recursoHumano = new RecursoHumano
                {
                    IdRecHum = user.Id,
                    RoleNoLocador = "funcionario",
                    LocadorId = model.LocadorId
                };

                _context.recursoshumanos.Add(recursoHumano);
                await _context.SaveChangesAsync();

                // Verifica se a função já existe senão cria
                if (!await _roleManager.RoleExistsAsync("funcionario"))
                {
                    var role = new IdentityRole { Name = "funcionario" };
                    await _roleManager.CreateAsync(role);
                }

                // Atribui o user à função
                if (user != null)
                {
                    await _userManager.AddToRoleAsync(user, "funcionario");
                    TempData["SuccessMessage"] = "Funcionário adicionado com sucesso.";
                }

                return RedirectToAction("ListaUsr");


            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        TempData["ErrorMessage"] = "Formato do Email ou PassWord Inválidos";
        return CreateFunc();
    }





}