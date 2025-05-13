using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Funciones;
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

namespace Regal_Plataforma.Controllers
{
    [Authorize(Roles = "Gestor")]
    public class ObrasController : Controller
    {
        private readonly INotasServices _notaService;
        private readonly IObraService _obra;
        private readonly IClienteService _cliente;
        private readonly IUsuarioServices _usuarios;
        private readonly ICalendarServices _calendario;
        private readonly IRazorPartialToStringRenderer _renderer;

        public ObrasController(INotasServices notasServices, IObraService obraService, ICalendarServices calendario, IClienteService cliente, IUsuarioServices usuarios, IRazorPartialToStringRenderer renderer)
        {
            _notaService = notasServices;
            _obra = obraService;
            _cliente = cliente;
            _usuarios = usuarios;
            _calendario = calendario;
            _renderer = renderer;
        }

        public async Task<IActionResult> Index()
        {
            await AppLogger.WriteAsync(LogType.Info, User.Identity.Name, "Accediendo a la gestión de obras");
            var model = new VM_GestionObras
            {
                listObras = await _obra.GetObrasAsync(),
                listEstadosObras = await _obra.GestEstadosObrasAsync()
            };
            await AppLogger.WriteAsync(LogType.Info, User.Identity.Name, $"Datos cargados - Total obras: {model.listObras.Count}, Total estados: {model.listEstadosObras.Count}");
            return View(model);
        }

        public async Task<IActionResult> VerDetallesObra(int ObraPK)
        {
            await AppLogger.WriteAsync(LogType.Info, User.Identity.Name, $"Accediendo a los detalles de la obra - ObraPK: {ObraPK}");
            VM_VerDetallesObra vm = new VM_VerDetallesObra
            {
                Obra = await _obra.GetObraByPkAsync(ObraPK),
                Calendario = await _calendario.GetYearCalendarViewModel(DateTime.Now.Year,null, ObraPK,null),
            };
            vm.Notas.Obra_PK = ObraPK;
            vm.Notas.listNotas = vm.Obra.NotasObras.ToList();
            await AppLogger.WriteAsync(LogType.Info, User.Identity.Name, $"Detalles de la obra cargados - ObraPK: {ObraPK}");
            return View(vm);
        }

        public async Task<IActionResult> GetCreateEdit(int ObraPk)
        {
            VM_CreateUpdateObra vm = new VM_CreateUpdateObra();
            if (ObraPk == 0)
            {
                vm = new VM_CreateUpdateObra
                {
                    Obra = new Obra(),
                    listClientes = await _cliente.GetClientesAsync(),
                    listaGestores = await _usuarios.GetGestoresAsync(),
                    listEstadosObras = await _obra.GestEstadosObrasAsync()
                };
            }
            else
            {
                vm = new VM_CreateUpdateObra
                {
                    Obra = await _obra.GetObraByPkAsync(ObraPk),
                    listClientes = await _cliente.GetClientesAsync(),
                    listaGestores = await _usuarios.GetGestoresAsync(),
                    listEstadosObras = await _obra.GestEstadosObrasAsync()
                };
            }

            var html = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_crearObra.cshtml", vm);
            return Json(new { status = "OK", partial = html });
        }

        public async Task<IActionResult> Guardar(Obra obra)
        {
            await AppLogger.WriteAsync(LogType.Info, User.Identity.Name, $"Guardando obra con PK = {obra.ObraPk}");

            var result = obra.ObraPk == 0
                ? await _obra.CreateObraAsync(obra)
                : await _obra.UpdateObraAsync(obra);

            if (result == Resultado.OK)
            {
                await AppLogger.WriteAsync(LogType.Info, User.Identity.Name, $"Obra con PK = {obra.ObraPk} guardada correctamente");
                return Json(new { status = "OK" });
            }

            await AppLogger.WriteAsync(LogType.Error, User.Identity.Name, $"Error guardando la obra con PK = {obra.ObraPk}");
            return Json(new { status = "KO", message = "Error guardando la obra" });
        }

        public async Task<IActionResult> GetNotasObra(int Obra_PK)
        {
            VM_NotasObra vm = new VM_NotasObra();

            var obra = await _obra.GetObraByPkAsync(Obra_PK);
            vm.listNotas = obra.NotasObras.ToList();
            vm.Obra_PK = Obra_PK;

            string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasObra.cshtml", vm);

            return Json(new { status = "OK", partial = partialHtml });
        }

        public async Task<IActionResult> GetCreateEditNotaObra(int NotasObra_PK, int Obra_PK)
        {
            VM_NotasObra vm = new VM_NotasObra();
            vm.Nota = new NotasObra();
            vm.Nota.ObraPk = Obra_PK;
            vm.Obra_PK = Obra_PK;

            if (NotasObra_PK != 0)
                vm.Nota = await _notaService.GetNotaObraByIdAsync(NotasObra_PK);

            string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_crearNotaObra.cshtml", vm);

            return Json(new { status = "OK", partial = partialHtml });
        }

        public async Task<JsonResult> CreateEditNotaObra(VM_NotasObra notasObra)
        {
            await AppLogger.WriteAsync(LogType.Info, User.Identity.Name, $"Creando/Actualizando nota de obra - NotasObraPK: {notasObra.Nota.NotasObrasPk}, ObraPK: {notasObra.Nota.ObraPk}");
            bool success = notasObra.Nota.NotasObrasPk == 0
                ? await _notaService.CreateNotaObraAsync(notasObra.Nota, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                : await _notaService.UpdateNotaObraAsync(notasObra.Nota, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));

            if (success)
            {
                VM_NotasObra vm = new VM_NotasObra();

                var obra = await _obra.GetObraByPkAsync(notasObra.Obra_PK);
                vm.listNotas = obra.NotasObras.ToList();

                string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasObra.cshtml", vm);
                await AppLogger.WriteAsync(LogType.Info, User.Identity.Name, $"Nota de obra guardada correctamente - NotasObraPK: {notasObra.Nota.NotasObrasPk}");
                return Json(new { status = "OK" });
            }
            else
            {
                await AppLogger.WriteAsync(LogType.Error, User.Identity.Name, $"Error guardando nota de obra - NotasObraPK: {notasObra.Nota.NotasObrasPk}");
                return Json(new { status = "KO", message = "Error guardando nota de obra" });
            }
        }

        public async Task<JsonResult> DeleteNotaObra(int NotasObra_PK, int Obra_PK)
        {
            await AppLogger.WriteAsync(LogType.Info, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Obra - NotasObraPk = {NotasObra_PK}");
            bool success = await _notaService.DeleteNotaObraAsync(NotasObra_PK);
            if (success)
            {
                VM_NotasObra vm = new VM_NotasObra();

                var obra = await _obra.GetObraByPkAsync(Obra_PK);
                vm.listNotas = obra.NotasObras.ToList();

                string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasObra.cshtml", vm);

                await AppLogger.WriteAsync(LogType.Info, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Obra correcto - NotasObraPk = {NotasObra_PK}");
                return Json(new { status = "OK", partial = partialHtml });
            }
            else
            {
                await AppLogger.WriteAsync(LogType.Info, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Obra incorrecto - NotasObraPk = {NotasObra_PK}");
                return Json(new { status = "KO", message = "Error eliminando nota" });
            }
        }
    }
}
