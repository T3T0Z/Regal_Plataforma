using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Funciones;
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;
using System.Security.Claims;

namespace Regal_Plataforma.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ClientesController : Controller
    {
        private readonly INotasServices _notaService;
        private readonly IClienteService _cliente;
        private readonly REGAL_ASISTENCIAContext _dbContext;
        private readonly IRazorPartialToStringRenderer _renderer;

        public ClientesController(INotasServices notaService, IClienteService cliente, REGAL_ASISTENCIAContext dbContext, IRazorPartialToStringRenderer renderer)
        {
            _notaService = notaService;
            _cliente = cliente;
            _dbContext = dbContext;
            _renderer = renderer;
        }

        public async Task<IActionResult> Index()
        {
            VM_GestionClientes vm = new VM_GestionClientes();

            vm.listClientes = await _cliente.GetClientesAsync();

            return View(vm);
        }

        public async Task<IActionResult> GetCreateEdit(int Cliente_PK)
        {
            VM_CreateUpdateClientes vm = new VM_CreateUpdateClientes();

            if(Cliente_PK == 0)
            {
                vm.listTiposClientes = await _cliente.GetTiposClientesAsync();
            }
            else
            {

                vm.Cliente = await _cliente.GetClienteByPkAsync(Cliente_PK);
                vm.listTiposClientes = await _cliente.GetTiposClientesAsync();
            }


            string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_crearCliente.cshtml", vm);

            return Json(new { status = "OK", partial = partialHtml });
        }

        public async Task<JsonResult> Guardar(Cliente cliente)
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Entra en guardar cliente - Cliente_PK = {cliente.ClientePk}");

            Resultado resultado = cliente.ClientePk == 0
                ? await _cliente.CreateClienteAsync(cliente)
                : await _cliente.UpdateClienteAsync(cliente);

            if (resultado == Resultado.KO)
                return Json(new { status = "KO", message = "Error actualizando cliente" });
            else
                return Json(new { status = "OK" });
        }

        public async Task<IActionResult> GetNotasCliente(int Cliente_PK)
        {
            VM_NotasCliente vm = new VM_NotasCliente();

            var cliente = await _cliente.GetClienteByPkAsync(Cliente_PK);
            vm.listNotas = cliente.NotasClientes.ToList();
            vm.Cliente_PK = Cliente_PK;

            string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasCliente.cshtml", vm);

            return Json(new { status = "OK", partial = partialHtml });
        }

        public async Task<IActionResult> GetCreateEditNotaCliente(int NotasCliente_PK, int Cliente_PK)
        {
            VM_NotasCliente vm = new VM_NotasCliente();
            vm.Nota = new NotasCliente();
            vm.Nota.ClientePk = Cliente_PK;

            if (NotasCliente_PK != 0)
                vm.Nota = await _notaService.GetNotaClienteByIdAsync(NotasCliente_PK);

            string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_crearNotaCliente.cshtml", vm);

            return Json(new { status = "OK", partial = partialHtml });
        }

        public async Task<JsonResult> CreateEditNotaCliente (VM_NotasCliente notasCliente)
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota Cliente - NotasClientePk = {notasCliente.Nota.NotasClientesPk}");

            NotasCliente nota = notasCliente.Nota;

            bool success = nota.NotasClientesPk == 0
                ? await _notaService.CreateNotaClienteAsync(nota, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                : await _notaService.UpdateNotaClienteAsync(nota, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));

            if (success)
            {
                VM_NotasCliente vm = new VM_NotasCliente();

                var cliente = await _cliente.GetClienteByPkAsync(nota.ClientePk);
                vm.listNotas = cliente.NotasClientes.ToList();

                string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasCliente.cshtml", vm);

                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota Cliente correcto - NotasClientePk = {notasCliente.Nota.NotasClientesPk}");
                return Json(new { status = "OK", partial = partialHtml });
            }
            else
            {
                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota Cliente incorrecto - NotasClientePk = {nota.NotasClientesPk}");
                return Json(new { status = "KO", message = "Error editando nota" });
            }
        }

        public async Task<JsonResult> DeleteNotaCliente (int NotasCliente_PK, int Cliente_PK)
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Cliente - NotasClientePk = {NotasCliente_PK}");
            bool success = await _notaService.DeleteNotaClienteAsync(NotasCliente_PK);
            if (success)
            {
                VM_NotasCliente vm = new VM_NotasCliente();

                var cliente = await _cliente.GetClienteByPkAsync(Cliente_PK);
                vm.listNotas = cliente.NotasClientes.ToList();

                string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasCliente.cshtml", vm);

                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Cliente correcto - NotasClientePk = {NotasCliente_PK}");
                return Json(new { status = "OK", partial = partialHtml });
            }
            else
            {
                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Cliente incorrecto - NotasClientePk = {NotasCliente_PK}");
                return Json(new { status = "KO", message = "Error eliminando nota" });
            }
        }
    }
}
