using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Funciones;
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;
using System.Diagnostics.Metrics;
using System.Security.Claims;

namespace Regal_Plataforma.Controllers
{
    [Authorize]
    public class ArchivosController : Controller
    {
        private readonly IArchivoService _archivoService;
        private readonly ITrabajosServices _trabajoService;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly string _webRootPath;

        public ArchivosController(IArchivoService archivoService, ITrabajosServices trabajoService, IRazorPartialToStringRenderer renderer, IWebHostEnvironment env)
        {
            _archivoService = archivoService;
            _trabajoService = trabajoService;
            _renderer = renderer;
            _webRootPath = env.WebRootPath;
        }

        public async Task<IActionResult> Galeria(string entidad, int entidadPk)
        {
            var archivos = await _archivoService.ObtenerArchivosAsync(entidad, entidadPk);

            // Agregar archivos de trabajos si es una Obra o un Siniestro
            if (entidad == "Obra" || entidad == "Siniestro")
            {
                var archivosTrabajos = await _archivoService.ObtenerArchivosYRelacionadosAsync(entidad, entidadPk);
                foreach (var archivoTrabajo in archivosTrabajos)
                {
                    archivoTrabajo.EsDeTrabajo = true; // bandera para diferenciarlos en la vista
                    archivos.Add(archivoTrabajo);
                }
            }

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
        public async Task<IActionResult> Subir(string entidad, int entidadPk, List<IFormFile> archivos, bool firma = false)
        {
            var resultado = await _archivoService.GuardarArchivosAsync(entidad, entidadPk, archivos, User.FindFirstValue(ClaimTypes.Name));

            if (entidad == "Trabajo")
            {
                var trabajo = await _trabajoService.GetTrabajoByPkAsync(entidadPk);
                if(firma == true)
                {
                    trabajo.HayFirma = true;
                    await _trabajoService.UpdateTrabajoAsync(trabajo);
                }
            }

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
            if (ruta.Contains("..")) return BadRequest("Ruta no válida");

            ruta = ruta.TrimStart('/', '\\');

            var rutaCompleta = Path.Combine(_webRootPath, ruta);
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
