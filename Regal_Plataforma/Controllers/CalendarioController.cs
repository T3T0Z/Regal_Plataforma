using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Funciones;
using Regal_Plataforma.Models;

namespace Regal_Plataforma.Controllers
{
    public class CalendarioController : Controller
    {
        private readonly ICalendarServices _calendario;
        private readonly IRazorPartialToStringRenderer _renderer;

        public CalendarioController(ICalendarServices calendario, IRazorPartialToStringRenderer renderer)
        {
            _calendario = calendario;
            _renderer = renderer;
        }
        // Partial view para el calendario anual.
        public async Task<ActionResult> YearCalendarPartial(int? UsuarioPk, int? ObraPk, int? SiniestroPk, int year = 2025)
        {
            VM_Calendario vm = await _calendario.GetYearCalendarViewModel(year, UsuarioPk, ObraPk, SiniestroPk);

            var partial = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_calendario.cshtml", vm);

            return Json(new { status = "OK", partial = partial });
        }

        // Partial view para el calendario mensual.
        public async Task<ActionResult> MonthCalendarPartial(int month, int? UsuarioPk, int? ObraPk, int? SiniestroPk, int year = 2025)
        {
            VM_Month vm = await _calendario.GetMonthCalendarViewModel(year, month, UsuarioPk, ObraPk, SiniestroPk);

            var partial = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_calendarioMensual.cshtml", vm);

            return Json(new { status = "OK", partial = partial });
        }
    }
}
