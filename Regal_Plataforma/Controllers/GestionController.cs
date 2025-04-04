using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Funciones;
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;
using System.Security.Claims;

namespace Regal_Plataforma.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class GestionController : Controller
    {
        // GET: GestionController
        public IActionResult Index()
        {
            return View();
        }

    }
}
