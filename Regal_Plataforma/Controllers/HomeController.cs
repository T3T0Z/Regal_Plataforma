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
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            string link = Url.Action("Index", "Home");

            if (User.IsInRole("Gestor"))
                link = Url.Action("Index", "Gestion");
            else if (!User.Identity.IsAuthenticated)
                link = Url.Action("Index", "Login"); ;

            ViewData["RedirectUrl"] = link;
            return View();
        }
    }
}
