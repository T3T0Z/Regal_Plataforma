using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Funciones;
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;
using System.Security.Claims;

namespace Regal_Plataforma.Controllers
{
    // Controllers/ArchivosController.cs
    [Authorize]
    public class ArchivosController : Controller
    { 
        private readonly IArchivoService _archivoService;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly string _webRootPath;

        public ArchivosController(IArchivoService archivoService, IRazorPartialToStringRenderer renderer, IWebHostEnvironment env)
        {
            _archivoService = archivoService;
            _renderer = renderer;
            _webRootPath = env.WebRootPath; // Esto da la ruta física a wwwroot
        }

        public async Task<IActionResult> Galeria(string entidad, int entidadPk)
        {
            var archivos = await _archivoService.ObtenerArchivosAsync(entidad, entidadPk);
            var vm = new VM_GaleriaArchivos
            {
                Entidad = entidad,
                EntidadPk = entidadPk,
                ListaArchivos = archivos
            };

            string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_galeriaArchivos.cshtml", vm);

            return Json(new { status = "OK", partial = partialHtml });
        }

        [HttpPost]
        public async Task<IActionResult> Subir(string entidad, int entidadPk, List<IFormFile> archivos)
        {
            var resultado = await _archivoService.GuardarArchivosAsync(entidad, entidadPk, archivos);
            return Json(new { status = resultado ? "OK" : "KO" });
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar([FromBody] ArchivoDto archivo)
        {
            var resultado = await _archivoService.EliminarArchivoAsync(archivo.Ruta);
            return Json(new { status = resultado ? "OK" : "KO" });
        }

        public IActionResult Ver(string ruta)
        {
            // Seguridad: evita rutas fuera de wwwroot
            if (ruta.Contains("..")) return BadRequest("Ruta no válida");

            ruta = ruta.TrimStart('/', '\\');

            var rutaCompleta = Path.Combine(_webRootPath, ruta); // Ruta absoluta
            if (!System.IO.File.Exists(rutaCompleta))
                return NotFound();

            var extension = Path.GetExtension(rutaCompleta).ToLowerInvariant();
            var mimeType = extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            var stream = new FileStream(rutaCompleta, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(stream, mimeType);
        }
    }
}
