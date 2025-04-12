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
    [Authorize(Roles = "Gestor")]
    public class EncargosController : Controller
    {
        private readonly REGAL_ASISTENCIAContext _dbContext;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IConfiguration _config;
        private readonly IOrderDatosServices _orderdatos;
        private readonly IGremiosServices _gremios;
        private readonly INotasServices _notaService;

        public EncargosController(REGAL_ASISTENCIAContext dbContext, IRazorPartialToStringRenderer renderer, IConfiguration config, IOrderDatosServices orderdatos, IGremiosServices gremios, INotasServices notasservices)
        {
            _dbContext = dbContext;
            _renderer = renderer;
            _config = config;
            _orderdatos = orderdatos;
            _gremios = gremios;
            _notaService = notasservices;
        }

        public IActionResult Index()
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), "Entra en gestion encargos");

            VM_GestionEncargos vm = new VM_GestionEncargos();

            vm.listOrderDatos = _orderdatos.GetOrderDatos().Result;
            vm.listGremios = _gremios.GetGremiosAsync().Result;

            return View(vm);
        }

        public IActionResult VerDetallesEncargo(int OrderDatosPK)
        {
            VM_VerDetallesEncargo vm = new VM_VerDetallesEncargo();

            vm.OrderDatos = _orderdatos.GetOrderDatosByPK(OrderDatosPK).Result;
            vm.listGremios = _gremios.GetGremiosAsync().Result;
            vm.Notas.Encargo_PK = OrderDatosPK;
            vm.Notas.listNotas = vm.OrderDatos.SiniestroPkNavigation.NotasSiniestros.ToList();

            return View(vm);
        }

        public async Task<IActionResult> ActualizarGremioEncargo(int OrderDatosPK, int GremioPK)
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Entra en actualizar Gremio Encargo - OrderDatosPK = {OrderDatosPK} - GremioPK = {GremioPK}");

            var resultado = await _orderdatos.SetGremioAsync(OrderDatosPK, GremioPK);

            VM_VerDetallesEncargo vm = new VM_VerDetallesEncargo();

            vm.OrderDatos = await _orderdatos.GetOrderDatosByPK(OrderDatosPK);
            vm.listGremios = await _gremios.GetGremiosAsync();

            string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_detallesEncargo.cshtml", vm);

            if (resultado == Resultado.KO)
            {
                Func_Comunes.LogsAplicacion(TiposLogs.ERROR, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Error actualizando gremio - OrderDatosPK = {OrderDatosPK} - GremioPK = {GremioPK}");
                return Json(new { status = "KO", message = "Error actualizando gremio", partial = partialHtml });
            }
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Gremio actualizado correctamente - OrderDatosPK = {OrderDatosPK} - GremioPK = {GremioPK}");
            return Json(new { status = "OK", partial = partialHtml });
        }

        public async Task<IActionResult> GetCreateEditNotaEncargo(int NotasEncargo_PK, int Siniestro_PK)
        {
            VM_NotasEncargo vm = new VM_NotasEncargo();
            vm.Nota = new NotasSiniestro();
            vm.Nota.SiniestroPk = Siniestro_PK;

            if (NotasEncargo_PK != 0)
                vm.Nota = await _notaService.GetNotaSiniestroByIdAsync(NotasEncargo_PK);

            string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_crearNotaEncargo.cshtml", vm);

            return Json(new { status = "OK", partial = partialHtml });
        }

        public async Task<JsonResult> CreateEditNotaEncargo(VM_NotasEncargo notasSiniestro)
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota Siniestro - NotasSiniestroPk = {notasSiniestro.Nota.NotasSiniestroPk}");

            NotasSiniestro nota = notasSiniestro.Nota;

            bool success = nota.NotasSiniestroPk == 0
                ? await _notaService.CreateNotaSiniestroAsync(nota, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                : await _notaService.UpdateNotaSiniestroAsync(nota, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));

            if (success)
            {
                VM_NotasEncargo vm = new VM_NotasEncargo();

                var siniestro = await _orderdatos.GetOrderDatosByPK(nota.SiniestroPk);
                vm.listNotas = siniestro.SiniestroPkNavigation.NotasSiniestros.ToList();

                string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasEncargo.cshtml", vm);

                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota Siniestro correcto - NotasSiniestroPk = {notasSiniestro.Nota.NotasSiniestroPk}");
                return Json(new { status = "OK", partial = partialHtml });
            }
            else
            {
                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota Siniestro incorrecto - NotasSiniestroPk = {nota.NotasSiniestroPk}");
                return Json(new { status = "KO", message = "Error editando nota" });
            }
        }

        public async Task<JsonResult> DeleteNotaEncargo(int NotasSiniestro_PK, int Siniestro_PK)
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Siniestro - NotasSiniestro_PK = {NotasSiniestro_PK}");
            bool success = await _notaService.DeleteNotaSiniestroAsync(NotasSiniestro_PK);
            if (success)
            {
                VM_NotasEncargo vm = new VM_NotasEncargo();

                var siniestro = await _orderdatos.GetOrderDatosByPK(Siniestro_PK);
                vm.listNotas = siniestro.SiniestroPkNavigation.NotasSiniestros.ToList();

                string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasEncargo.cshtml", vm);

                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Siniestro correcto - NotasSiniestro_PK = {NotasSiniestro_PK}");
                return Json(new { status = "OK", partial = partialHtml });
            }
            else
            {
                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Siniestro incorrecto - NotasSiniestro_PK = {NotasSiniestro_PK}");
                return Json(new { status = "KO", message = "Error eliminando nota" });
            }
        }
    }
}
