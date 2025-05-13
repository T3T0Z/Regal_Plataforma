using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Funciones;
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;
using System;
using System.Diagnostics;

namespace Regal_Plataforma.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IArchivoService _archivos;

        public HomeController(IArchivoService archivos)
        {
            _archivos = archivos;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Privacy()
        {
            await AppLogger.WriteAsync(LogType.Info, "Anónimo", "Accediendo a la página de privacidad");
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            string link = Url.Action("Index", "Home");

            if (User.IsInRole("Gestor"))
                link = Url.Action("Index", "Gestion");  
            else if (User.IsInRole("Operario"))
                link = Url.Action("Index", "Operario");
            else if (!User.Identity.IsAuthenticated)
                link = Url.Action("Index", "Login");

            await AppLogger.WriteAsync(LogType.Error, User.Identity.Name, $"Error en la aplicación, redirigiendo a {link}");
            ViewData["RedirectUrl"] = link;
            return View();
        }
    }
}
