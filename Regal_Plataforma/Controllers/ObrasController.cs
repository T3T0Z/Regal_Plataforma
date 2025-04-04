using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Funciones;
using Regal_Plataforma.Models.BDD;
using Regal_Plataforma.Models;
using System.Security.Claims;
using System.Text.Json;

[Authorize(Roles = "Administrador")]
public class ObrasController : Controller
{
    private readonly INotasServices _notaService;
    private readonly IObraService _obra;
    private readonly IClienteService _cliente;
    private readonly IUsuarioServices _usuarios;
    private readonly IRazorPartialToStringRenderer _renderer;

    public ObrasController(INotasServices notasServices, IObraService obraService, IClienteService cliente, IUsuarioServices usuarios, IRazorPartialToStringRenderer renderer)
    {
        _notaService = notasServices;
        _obra = obraService;
        _cliente = cliente;
        _usuarios = usuarios;
        _renderer = renderer;
    }

    public async Task<IActionResult> Index()
    {
        var model = new VM_GestionObras
        {
            listObras = await _obra.GetObrasAsync(),
            listEstadosObras = await _obra.GestEstadosObrasAsync()
        };
        return View(model);
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
        var result = obra.ObraPk == 0
            ? await _obra.CreateObraAsync(obra)
            : await _obra.UpdateObraAsync(obra);

        if (result == Resultado.OK)
            return Json(new { status = "OK" });

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

        if (NotasObra_PK != 0)
            vm.Nota = await _notaService.GetNotaObraByIdAsync(NotasObra_PK);

        string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_crearNotaObra.cshtml", vm);

        return Json(new { status = "OK", partial = partialHtml });
    }

    public async Task<JsonResult> CreateEditNotaObra(VM_NotasObra notasObra)
    {
        Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota Obra - NotasObraPk = {notasObra.Nota.NotasObrasPk}");

        NotasObra nota = notasObra.Nota;

        bool success = nota.NotasObrasPk == 0
            ? await _notaService.CreateNotaObraAsync(nota, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            : await _notaService.UpdateNotaObraAsync(nota, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));

        if (success)
        {
            VM_NotasObra vm = new VM_NotasObra();

            var obra = await _obra.GetObraByPkAsync(nota.ObraPk);
            vm.listNotas = obra.NotasObras.ToList();

            string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasObra.cshtml", vm);

            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota Obra correcto - NotasObraPk = {notasObra.Nota.NotasObrasPk}");
            return Json(new { status = "OK", partial = partialHtml });
        }
        else
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota Obra incorrecto - NotasObraPk = {nota.NotasObrasPk}");
            return Json(new { status = "KO", message = "Error editando nota" });
        }
    }

    public async Task<JsonResult> DeleteNotaObra(int NotasObra_PK, int Obra_PK)
    {
        Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Obra - NotasObraPk = {NotasObra_PK}");
        bool success = await _notaService.DeleteNotaObraAsync(NotasObra_PK);
        if (success)
        {
            VM_NotasObra vm = new VM_NotasObra();

            var obra = await _obra.GetObraByPkAsync(Obra_PK);
            vm.listNotas = obra.NotasObras.ToList();

            string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasObra.cshtml", vm);

            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Obra correcto - NotasObraPk = {NotasObra_PK}");
            return Json(new { status = "OK", partial = partialHtml });
        }
        else
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota Obra incorrecto - NotasObraPk = {NotasObra_PK}");
            return Json(new { status = "KO", message = "Error eliminando nota" });
        }
    }
}
