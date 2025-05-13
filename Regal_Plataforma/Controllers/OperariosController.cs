using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;
using System.Globalization;
using System.Security.Claims;

namespace Regal_Plataforma.Controllers
{

    // Controlador
    [Authorize(Roles = "Operario")]
    public class OperarioController : Controller
    {
        private readonly IOperarioService _operarioService;
        private readonly ITrabajosServices _trabajoService;
        private readonly string _basePath = "wwwroot/Archivos";

        public OperarioController(IOperarioService operarioService, ITrabajosServices trabajoService)
        {
            _operarioService = operarioService;
            _trabajoService = trabajoService;
        }

        public async Task<IActionResult> Index()
        {
            int usuarioPk = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var trabajo = await _operarioService.ObtenerTrabajoAsignadoAsync(usuarioPk);

            if (trabajo == null)
            {
                trabajo = new Trabajo();
                trabajo.FechaAsignada = DateTime.Now;
                trabajo.FechaFinAsignada = DateTime.Now;
                trabajo.FechaEjecucion = DateTime.Now;
                trabajo.EstadoTrabajoPk = 4;
                trabajo.UasignadoPk = usuarioPk;
                trabajo.UgestorPk = usuarioPk;
                trabajo.FechaCreacion = DateTime.Now;
                trabajo.Activo = true;

                trabajo = await _operarioService.CreateTrabajoLibreAsync(trabajo);
            }
            else
            {
                if (trabajo.FechaEjecucion == null)
                    trabajo.FechaEjecucion = DateTime.Now;
                trabajo.EstadoTrabajoPk = 2;
                if(trabajo.SiniestroPk != null)
                {
                    trabajo.DescripcionParteGeneralli = "Visita operario";
                    trabajo.Dnifirma = trabajo.SiniestroPkNavigation.OrderDatos.FirstOrDefault()?.PaseguradoPkNavigation.CarneIdentidad;
                    trabajo.NombreAseguradoParteGeneralli = trabajo.SiniestroPkNavigation.OrderDatos.FirstOrDefault()?.PaseguradoPkNavigation.Nombre + " " + trabajo.SiniestroPkNavigation.OrderDatos.FirstOrDefault()?.PaseguradoPkNavigation.Apellido1 + " " + trabajo.SiniestroPkNavigation.OrderDatos.FirstOrDefault()?.PaseguradoPkNavigation.Apellido2;
                }

                await _operarioService.UpdateTrabajoAsync(trabajo);
            }

            var vm = new VM_TrabajoAsignado
            {
                Trabajo = trabajo
            };

            return View("TrabajoAsignado", vm);
        }

        [HttpPost]
        public async Task<IActionResult> FinalizarTrabajo(Trabajo trabajo, string? latitude, string? longitude, string? accuracy)
        {
            int usuarioPk = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var dirDestino = Path.Combine(_basePath, "Trabajo", trabajo.TrabajoPk.ToString());
            if(trabajo.EstadoTrabajoPk != 4)
            {
                trabajo.EstadoTrabajoPk = 3;
            }
            else
            {
                trabajo.FechaFinAsignada = DateTime.Now;
            }
                trabajo.FechaFinEjecucion = DateTime.Now;

            bool existenArchivosNoFirma = false;

            if (Directory.Exists(dirDestino))
            {
                existenArchivosNoFirma = Directory
                    .EnumerateFiles(dirDestino, "*", SearchOption.TopDirectoryOnly)
                    .Any(ruta =>
                    {
                        string nombre = Path.GetFileName(ruta).ToLowerInvariant();
                        return !nombre.Contains("firma");          // <- ajusta condición
                    });
            }

            if (!existenArchivosNoFirma)
                trabajo.Fotos = false;
            else
                trabajo.Fotos = true;

            //GUARDAR UBICACIONES DE LOS TRABAJOS
            if(latitude != null && longitude != null)
            {
                UbicacionesTrabajo ubicacion = new UbicacionesTrabajo
                {
                    Latitude = Convert.ToDecimal(latitude, CultureInfo.InvariantCulture),
                    Longitude = Convert.ToDecimal(longitude, CultureInfo.InvariantCulture),
                    Accuracy = Convert.ToDecimal(accuracy, CultureInfo.InvariantCulture),
                    FechaCreacion = DateTime.Now,
                    TrabajoPk = trabajo.TrabajoPk
                };

                await _operarioService.GuardarUbicacionTrabajoAsync(ubicacion);
            }

            var result = await _operarioService.UpdateTrabajoAsync(trabajo);
            if (result.resultado == Resultado.OK)
            {
                if(trabajo.SiniestroPk != null)
                {
                    var trabajoActa = await _trabajoService.GetTrabajoByPkAsync(trabajo.TrabajoPk);
                    await _trabajoService.GenerarActaTrabajo(trabajoActa);
                }
                return Json(new { status = "OK" });
            }

            return Json(new { status = "KO", message = result.mensaje });
        }


        [HttpPost]
        public async Task<IActionResult> GuardarUbicacion(int TrabajoPk, string? latitude, string? longitude, string? accuracy)
        {

            //GUARDAR UBICACIONES DE LOS TRABAJOS
            if (latitude != null && longitude != null)
            {
                UbicacionesTrabajo ubicacion = new UbicacionesTrabajo
                {
                    Latitude = Convert.ToDecimal(latitude, CultureInfo.InvariantCulture),
                    Longitude = Convert.ToDecimal(longitude, CultureInfo.InvariantCulture),
                    Accuracy = Convert.ToDecimal(accuracy, CultureInfo.InvariantCulture),
                    FechaCreacion = DateTime.Now,
                    TrabajoPk = TrabajoPk
                };

                await _operarioService.GuardarUbicacionTrabajoAsync(ubicacion);
            }

            return Json(new { status = "OK" });
        }

        public async Task<IActionResult> Descanso(int TrabajoPk)
        {
            DescansosTrabajo descanso = new DescansosTrabajo();
            DescansosTrabajo newDescanso = new DescansosTrabajo();

            descanso = new DescansosTrabajo
            {
                TrabajoPk = TrabajoPk
            };

            newDescanso = await _operarioService.GuardarDescansoTrabajoAsync(descanso);

            if (newDescanso == null) return RedirectToAction("Index");


            return View("Descanso", newDescanso);
        }

        public async Task<IActionResult> FinalizarDescanso(DescansosTrabajo descanso)
        {
            if(descanso.Descripcion == null)
                return Json(new { status = "KO", message = "La descripcion del descanso es obligatoria" });

            descanso = await _operarioService.GuardarDescansoTrabajoAsync(descanso);

            return Json(new { status = "OK" });
        }
    }
}
