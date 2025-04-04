using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Models;

namespace Regal_Plataforma.Controllers;


[Authorize(Roles = "Administrador")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        string link = Url.Action("Index", "Home");

        if (User.IsInRole("Administrador"))
            link = Url.Action("Index", "Gestion");
        else if (!User.Identity.IsAuthenticated)
            link = Url.Action("Index", "Login"); ;

        ViewData["RedirectUrl"] = link;
        return View();
    }
}
