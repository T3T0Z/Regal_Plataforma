using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Funciones;
using Regal_Plataforma.Models.BDD;
using Regal_Plataforma.Models;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

namespace Regal_Plataforma.Controllers
{
    [Authorize(Roles = "Gestor")]
    public class TrabajosController : Controller
    {
        private readonly INotasServices _notaService;
        private readonly ITrabajosServices _trabajo;
        private readonly IObraService _obra;
        private readonly IOrderDatosServices _orderdatosService;
        private readonly IClienteService _cliente;
        private readonly IUsuarioServices _usuarios;
        private readonly IRazorPartialToStringRenderer _renderer;

        public TrabajosController(INotasServices notasServices, ITrabajosServices trabajoService, IObraService obraService, IOrderDatosServices orderdatosService, IClienteService cliente, IUsuarioServices usuarios, IRazorPartialToStringRenderer renderer)
        {
            _notaService = notasServices;
            _trabajo = trabajoService;
            _obra = obraService;
            _orderdatosService = orderdatosService;
            _cliente = cliente;
            _usuarios = usuarios;
            _renderer = renderer;
        }

        public async Task<IActionResult> GetCreateEdit(int TrabajoPk, int? UsuarioPk, int? SiniestroPk, int? ObraPk)
        {
            VM_CreateEditTrabajo vm = new VM_CreateEditTrabajo();

            if (TrabajoPk == 0)
            {
                vm = new VM_CreateEditTrabajo
                {
                    Trabajo = new Trabajo
                    {
                        EstadoTrabajoPk = 1,
                        UgestorPk = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    },
                    listObras = await _obra.GetObrasAsync(),
                    listSiniestros = await _orderdatosService.GetSiniestros(),
                    listUsuariosAsignados = await _usuarios.GetUsuariosAsync(),
                    listUsuariosGestores = await _usuarios.GetGestoresAsync(),
                    listEstadosTrabajo = await _trabajo.GetEstadosTrabajosAsync(),
                };

                vm.Trabajo.SiniestroPk = SiniestroPk == 0 ? null : SiniestroPk;
                vm.Trabajo.ObraPk = ObraPk == 0 ? null : ObraPk;
                vm.Trabajo.UasignadoPk = (int)((UsuarioPk == null || UsuarioPk == 0) ? 0 : UsuarioPk);
            }
            else
            {
                vm = new VM_CreateEditTrabajo
                {
                    Trabajo = await _trabajo.GetTrabajoByPkAsync(TrabajoPk),
                    listObras = await _obra.GetObrasAsync(),
                    listSiniestros = await _orderdatosService.GetSiniestros(),
                    listUsuariosAsignados = await _usuarios.GetUsuariosAsync(),
                    listUsuariosGestores = await _usuarios.GetGestoresAsync(),
                    listEstadosTrabajo = await _trabajo.GetEstadosTrabajosAsync(),
                };

                if (vm.Trabajo.EstadoTrabajoPk != 1)
                {
                    var partial = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_detallesTrabajo.cshtml", vm);
                    return Json(new { status = "OK", partial = partial });
                }

            }

            var html = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_crearTrabajo.cshtml", vm);
            return Json(new { status = "OK", partial = html });
        }

        public async Task<IActionResult> Guardar(Trabajo Trabajo)
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.Identity.Name, $"Guardando Trabajo con PK = {Trabajo.TrabajoPk}");

            var result = Trabajo.TrabajoPk == 0
                ? await _trabajo.CreateTrabajoAsync(Trabajo)
                : await _trabajo.UpdateTrabajoAsync(Trabajo);

            if (result.resultado == Resultado.OK)
            {
                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.Identity.Name, $"Trabajo con PK = {Trabajo.TrabajoPk} guardada correctamente");
                return Json(new { status = "OK" });
            }

            Func_Comunes.LogsAplicacion(TiposLogs.ERROR, User.Identity.Name, $"Error guardando la Trabajo con PK = {Trabajo.TrabajoPk}");
            return Json(new { status = "KO", message = result.mensaje });
        }

        public async Task<IActionResult> GetTrabajos(int? UsuarioPk, int? SiniestroPk, int? ObraPk, DateTime? date)
        {
            VM_listaTrabajos vm = new VM_listaTrabajos();
            List<Trabajo> listTrabajos = new List<Trabajo>();
            if (UsuarioPk != null && UsuarioPk != 0)
                listTrabajos = await _trabajo.GetTrabajosByUsuarioPkAsync((int)UsuarioPk);
            else if (SiniestroPk != null && SiniestroPk != 0)
                listTrabajos = await _trabajo.GetTrabajosBySiniestroPkAsync((int)SiniestroPk);
            else if (ObraPk != null && ObraPk != 0)
                listTrabajos = await _trabajo.GetTrabajosByObraPkAsync((int)ObraPk);

            vm.listTrabajos = listTrabajos;

            if (date != null)
            {
                vm.Dia = date?.ToString("dd/MM/yyyy");
                vm.listTrabajos = listTrabajos.Where(x => x.FechaAsignada.Date.Equals(date)).ToList();
            }

            var html = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_listaTrabajos.cshtml", vm);
            return Json(new { status = "OK", partial = html });
        }

        public async Task<JsonResult> Eliminar(int TrabajoPk, int UsuarioPk, DateTime date)
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar Trabajo - TrabajoPk = {TrabajoPk}");
            Resultado resultado = await _trabajo.DeleteTrabajoAsync(TrabajoPk);
            if (resultado == Resultado.OK)
            {
                VM_listaTrabajos vm = new VM_listaTrabajos();

                var listaTrabajosUsuario = await _trabajo.GetTrabajosByUsuarioPkAsync(UsuarioPk);

                vm.listTrabajos = listaTrabajosUsuario.Where(x => x.FechaAsignada.Date.Equals(date)).ToList();
                vm.Dia = date.ToString("dd/MM/yyyy");

                var html = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_listaTrabajos.cshtml", vm);
                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar Trabajo correcto - TrabajoPk = {TrabajoPk}");
                return Json(new { status = "OK", partial = html });
            }
            else
            {
                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar Trabajo incorrecto - TrabajoPk = {TrabajoPk}");
                return Json(new { status = "KO", message = "Error eliminando trabajo" });
            }
        }

        public async Task<IActionResult> GetNotasTrabajo(int Trabajo_PK)
        {
            VM_NotasTrabajo vm = new VM_NotasTrabajo();

            var Trabajo = await _trabajo.GetTrabajoByPkAsync(Trabajo_PK);
            vm.listNotas = Trabajo.NotasTrabajos.ToList();
            vm.Trabajo_PK = Trabajo_PK;

            string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasTrabajo.cshtml", vm);

            return Json(new { status = "OK", partial = partialHtml });
        }

        public async Task<IActionResult> GetCreateEditNotaTrabajo(int NotasTrabajo_PK, int Trabajo_PK)
        {
            VM_NotasTrabajo vm = new VM_NotasTrabajo();
            vm.Nota = new NotasTrabajo();
            vm.Nota.TrabajoPk = Trabajo_PK;

            if (NotasTrabajo_PK != 0)
                vm.Nota = await _notaService.GetNotaTrabajoByIdAsync(NotasTrabajo_PK);

            string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_crearNotaTrabajo.cshtml", vm);

            return Json(new { status = "OK", partial = partialHtml });
        }

        public async Task<JsonResult> CreateEditNotaTrabajo(VM_NotasTrabajo notasTrabajo)
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota Obra - NotasTrabajoPk = {notasTrabajo.Nota.NotasTrabajoPk}");

            NotasTrabajo nota = notasTrabajo.Nota;

            bool success = nota.NotasTrabajoPk == 0
                ? await _notaService.CreateNotaTrabajoAsync(nota, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                : await _notaService.UpdateNotaTrabajoAsync(nota, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));

            if (success)
            {
                VM_NotasTrabajo vm = new VM_NotasTrabajo();

                var Trabajo = await _trabajo.GetTrabajoByPkAsync(nota.TrabajoPk);
                vm.listNotas = Trabajo.NotasTrabajos.ToList();
                vm.Trabajo_PK = nota.TrabajoPk;

                string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasTrabajo.cshtml", vm);

                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota Trabajo correcto - NotasTrabajoPk = {notasTrabajo.Nota.NotasTrabajoPk}");
                return Json(new { status = "OK", partial = partialHtml });
            }
            else
            {
                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota Trabajo incorrecto - NotasTrabajoPk = {nota.NotasTrabajoPk}");
                return Json(new { status = "KO", message = "Error editando nota" });
            }
        }

        public async Task<JsonResult> DeleteNota(int NotasTrabajo_PK, int Trabajo_PK)
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Trabajo - NotasTrabajoPk = {NotasTrabajo_PK}");
            bool success = await _notaService.DeleteNotaTrabajoAsync(NotasTrabajo_PK);
            if (success)
            {
                VM_NotasTrabajo vm = new VM_NotasTrabajo();

                var Trabajo = await _trabajo.GetTrabajoByPkAsync(Trabajo_PK);
                vm.listNotas = Trabajo.NotasTrabajos.ToList();

                string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasTrabajo.cshtml", vm);

                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Trabajo correcto - NotasTrabajoPk = {NotasTrabajo_PK}");
                return Json(new { status = "OK", partial = partialHtml });
            }
            else
            {
                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Trabajo incorrecto - NotasTrabajoPk = {NotasTrabajo_PK}");
                return Json(new { status = "KO", message = "Error eliminando nota" });
            }
        }
    }
}
