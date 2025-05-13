using Regal_Plataforma.Models.BDD;
using Regal_Plataforma.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Regal_Plataforma.Funciones;

namespace Regal_Plataforma.Controllers
{
    [Authorize]
    public class LoginController : Controller
    {
        private readonly REGAL_ASISTENCIAContext _context;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IConfiguration _config;
        private readonly IUsuarioServices _usuarios;

        public LoginController(REGAL_ASISTENCIAContext context, IRazorPartialToStringRenderer renderer, IConfiguration config, IUsuarioServices usuarios)
        {
            _context = context;
            _renderer = renderer;
            _config = config;
            _usuarios = usuarios;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Gestor"))
                {
                    return RedirectToAction("Index", "Gestion");
                }
                return RedirectToAction("Index", "Home");
            }

            VM_Login vm = new VM_Login();

            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Index(VM_Login vm)
        {
            await AppLogger.WriteAsync(LogType.Info, vm.usuario ?? "VACIO", "Entra en inicio de sesion");

            if (!string.IsNullOrEmpty(vm.usuario))
            {
                await AppLogger.WriteAsync(LogType.Info, vm.usuario ?? "VACIO", "1");

                Usuario usuario = _usuarios.GetUsuarioByUsuarioAsync(vm.usuario).Result;
                await AppLogger.WriteAsync(LogType.Info, vm.usuario ?? "VACIO", "1.1");
                if (usuario != null)
                {
                    await AppLogger.WriteAsync(LogType.Info, vm.usuario ?? "VACIO", "2");
                    //if (Func_Comunes.EncryptClaveUsuario(vm.contrasena) == usuario.Contrasena)
                    if (vm.contrasena == usuario.Contrasena)
                    {
                        await AppLogger.WriteAsync(LogType.Info, vm.usuario ?? "VACIO", "3");
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioPk.ToString()),
                            new Claim(ClaimTypes.Name, usuario.Usuario1),
                            new Claim(ClaimTypes.Role, usuario.RolPkNavigation.Rol),
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(5),
                            IsPersistent = true,
                        };

                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                        await AppLogger.WriteAsync(LogType.Info, vm.usuario, $"Inicio de sesion correcto - PK ({usuario.UsuarioPk}) - Rol ({usuario.RolPkNavigation.Rol}) - Name ({usuario.Usuario1})");

                        if (usuario.RolPkNavigation.Rol == "Gestor")
                            return RedirectToAction("Index", "Gestion");
                        else if (usuario.RolPkNavigation.Rol == "Operario")
                            return RedirectToAction("Index", "Operario");
                        else
                            return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        await AppLogger.WriteAsync(LogType.Error, vm.usuario, "Clave incorrecta");
                        ModelState.AddModelError("contrasena", "Clave incorrecta");
                    }
                }
                else
                {
                    await AppLogger.WriteAsync(LogType.Error, vm.usuario, "Usuario no encontrado");
                    ModelState.AddModelError("usuario", "Usuario no encontrado");
                }
            }

            return View();
        }

        public async Task<ActionResult> Logout(string? returnUrl = null)
        {
            await AppLogger.WriteAsync(LogType.Info, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Cerrar sesion usuario (AccountController/Logout)");

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/Login/Index");
        }
    }
}
