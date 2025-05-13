using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Funciones;
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;
using System.Security.Claims;

namespace Regal_Plataforma.Controllers
{
    [Authorize(Roles = "Gestor")]
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
            await AppLogger.WriteAsync(LogType.Info, User.Identity.Name, "Accediendo a la gestión de clientes");
            VM_GestionClientes vm = new VM_GestionClientes
            {
                listClientes = await _cliente.GetClientesAsync()
            };
            await AppLogger.WriteAsync(LogType.Info, User.Identity.Name, $"Datos cargados - Total clientes: {vm.listClientes.Count}");
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
            await AppLogger.WriteAsync(LogType.Info, User.Identity.Name, $"Guardando cliente - ClientePK: {cliente.ClientePk}, Nombre: {cliente.Nombre}");
            var resultado = cliente.ClientePk == 0
                ? await _cliente.CreateClienteAsync(cliente)
                : await _cliente.UpdateClienteAsync(cliente);

            if (resultado == Resultado.OK)
            {
                await AppLogger.WriteAsync(LogType.Info, User.Identity.Name, $"Cliente guardado correctamente - ClientePK: {cliente.ClientePk}");
                return Json(new { status = "OK" });
            }
            else
            {
                await AppLogger.WriteAsync(LogType.Error, User.Identity.Name, $"Error guardando cliente - ClientePK: {cliente.ClientePk}");
                return Json(new { status = "KO", message = "Error guardando cliente" });
            }
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
            await AppLogger.WriteAsync(LogType.Info, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota Cliente - NotasClientePk = {notasCliente.Nota.NotasClientesPk}");

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

                await AppLogger.WriteAsync(LogType.Info, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota Cliente correcto - NotasClientePk = {notasCliente.Nota.NotasClientesPk}");
                return Json(new { status = "OK", partial = partialHtml });
            }
            else
            {
                await AppLogger.WriteAsync(LogType.Info, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota Cliente incorrecto - NotasClientePk = {nota.NotasClientesPk}");
                return Json(new { status = "KO", message = "Error editando nota" });
            }
        }

        public async Task<JsonResult> DeleteNotaCliente (int NotasCliente_PK, int Cliente_PK)
        {
            await AppLogger.WriteAsync(LogType.Info, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Cliente - NotasClientePk = {NotasCliente_PK}");
            bool success = await _notaService.DeleteNotaClienteAsync(NotasCliente_PK);
            if (success)
            {
                VM_NotasCliente vm = new VM_NotasCliente();

                var cliente = await _cliente.GetClienteByPkAsync(Cliente_PK);
                vm.listNotas = cliente.NotasClientes.ToList();

                string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasCliente.cshtml", vm);

                await AppLogger.WriteAsync(LogType.Info, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Cliente correcto - NotasClientePk = {NotasCliente_PK}");
                return Json(new { status = "OK", partial = partialHtml });
            }
            else
            {
                await AppLogger.WriteAsync(LogType.Info, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Cliente incorrecto - NotasClientePk = {NotasCliente_PK}");
                return Json(new { status = "KO", message = "Error eliminando nota" });
            }
        }
    }
}
