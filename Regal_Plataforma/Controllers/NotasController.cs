using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Funciones;
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;
using System.Security.Claims;

[Route("Notas")]
[Authorize(Roles = "Administrador")]
public class NotasController : Controller
{
    private readonly INotasServices _notaService;
    private readonly IOrderDatosServices _orderdatos;
    private readonly IGremiosServices _gremios;
    private readonly IRazorPartialToStringRenderer _renderer;

    public NotasController(INotasServices notasServices, IOrderDatosServices orderdatos, IGremiosServices gremios, IRazorPartialToStringRenderer renderer)
    {
        _notaService = notasServices;
        _orderdatos = orderdatos;
        _gremios = gremios;
        _renderer = renderer;
    }

    // GET: /Notas/GetCreate
    [HttpGet("GetCreate")]
    public IActionResult GetCreate()
    {
        string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_crearNota.cshtml", "");

        return Json(new { status = "OK", partial = partialHtml });
    }

    // GET: /Notas/GetEdit
    [HttpGet("GetEdit")]
    public IActionResult GetEdit()
    {
        string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_editarNota.cshtml", "");

        return Json(new { status = "OK", partial = partialHtml });
    }

    // POST: /Notas/Create
    [HttpPost("Create")]
    public async Task<JsonResult> Create (string texto, int SiniestroPK, int OrderDatosPK)
    {
        Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota");

        NotasSiniestro nota = new NotasSiniestro
        {
            SiniestroPk = SiniestroPK,
            Nota = texto,
            UsuarioPk = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier))
        };

        bool success = await _notaService.CreateNotaSiniestroAsync(nota, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier))); 
        if (success)
        {
            VM_VerDetallesEncargo vm = new VM_VerDetallesEncargo();

            vm.OrderDatos = await _orderdatos.GetOrderDatosByPK(OrderDatosPK);
            vm.listGremios = await _gremios.GetGremiosAsync();

            string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasEncargo.cshtml", vm);

            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota correcto");
            return Json(new { status = "OK", partial = partialHtml });
        }
        else
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Crear nota incorrecto");
            return Json(new { status = "KO", message = "Error creando nota" });
        }
    }

    // POST: /Notas/Edit
    [HttpPost("Edit")]
    public async Task<JsonResult> Edit (string texto, int NotasSiniestro_PK, int OrderDatosPK)
    {
        Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Edita nota - NotasSiniestroPk = {NotasSiniestro_PK}");
        
        NotasSiniestro nota = new NotasSiniestro
        {
            NotasSiniestroPk = NotasSiniestro_PK,
            Nota = texto,
            UsuarioPk = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier))
        };

        bool success = await _notaService.UpdateNotaSiniestroAsync(nota, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));
        if (success)
        {
            VM_VerDetallesEncargo vm = new VM_VerDetallesEncargo();

            vm.OrderDatos = await _orderdatos.GetOrderDatosByPK(OrderDatosPK);
            vm.listGremios = await _gremios.GetGremiosAsync();

            string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasEncargo.cshtml", vm);

            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Edita nota correcto - NotasSiniestroPk = {nota.NotasSiniestroPk}");
            return Json(new { status = "OK", partial = partialHtml });
        }
        else
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Edita nota incorrecto - NotasSiniestroPk = {nota.NotasSiniestroPk}");
            return Json(new { status = "KO", message = "Error editando nota" });
        }
    }

    // POST: /Notas/Delete
    [HttpPost("Delete")]
    public async Task<JsonResult> Delete (int NotasSiniestro_PK, int OrderDatosPK)
    {
        Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota - NotasSiniestroPk = {NotasSiniestro_PK}");
        bool success = await _notaService.DeleteNotaSiniestroAsync(NotasSiniestro_PK);
        if (success)
        {
            VM_VerDetallesEncargo vm = new VM_VerDetallesEncargo();

            vm.OrderDatos = await _orderdatos.GetOrderDatosByPK(OrderDatosPK);
            vm.listGremios = await _gremios.GetGremiosAsync();

            string partialHtml = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_notasEncargo.cshtml", vm);

            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota correcto - NotasSiniestroPk = {NotasSiniestro_PK}");
            return Json(new { status = "OK", partial = partialHtml });
        }
        else
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.FindFirstValue(ClaimTypes.NameIdentifier), $"Eliminar nota incorrecto - NotasSiniestroPk = {NotasSiniestro_PK}");
            return Json(new { status = "KO", message = "Error eliminando nota" });
        }
    }
}
