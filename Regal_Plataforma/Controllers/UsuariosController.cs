using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Funciones;
using Regal_Plataforma.Models.BDD;
using Regal_Plataforma.Models;
using System.Diagnostics;

namespace Regal_Plataforma.Controllers
{
    [Authorize(Roles = "Gestor")]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioServices _usuarios;
        private readonly IRazorPartialToStringRenderer _renderer;

        public UsuariosController(IUsuarioServices usuarios, IRazorPartialToStringRenderer renderer)
        {
            _usuarios = usuarios;
            _renderer = renderer;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new VM_GestionUsuarios
            {
                listUsuarios = await _usuarios.GetUsuariosAsync(),
                listRoles = await _usuarios.GetRolesAsync()
            };
        
            return View(vm);
        }

        public async Task<IActionResult> VerDetallesUsuario(int UsuarioPk)
        {
            var usuario = await _usuarios.GetUsuarioByPKAsync(UsuarioPk);

            if(usuario == null)
                return Json(new { status = "KO" });

            VM_DetallesUsuario vm = new VM_DetallesUsuario{ 
                Usuario = usuario,
                Notas = new VM_NotasUsuario
                {
                    listNotas = usuario.NotasUsuarioUsuarioPkNavigations.ToList(),
                    Usuario_PK = UsuarioPk
                },
                Calendario = await _usuarios.GetYearCalendarViewModel(DateTime.Now.Year, UsuarioPk),
            };

            vm.Calendario.UsuarioPk = UsuarioPk;

            return View(vm);
        }

        public async Task<IActionResult> GetCreateEditUsuario(int? usuarioPk)
        {
            var vm = new VM_CreateUpdateUsuario
            {
                listRoles = await _usuarios.GetRolesAsync()
            };

            if (usuarioPk.HasValue)
            {
                vm.Usuario = await _usuarios.GetUsuarioByPKAsync(usuarioPk.Value);
                if (vm.Usuario == null ) return Json(new { status = "KO"});
            }
            else
            {
                vm.Usuario = new Usuario();
            }

            var partial = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_crearUsuario.cshtml", vm);

            return Json(new { status = "OK", partial = partial });
        }

        public async Task<IActionResult> Guardar(Usuario usuario)
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.Identity.Name, $"Creando/Actualizando usuario con PK = {usuario.UsuarioPk}");
            var result = await _usuarios.CreateUpdateUsuarioAsync(usuario);
            if (result == Resultado.OK)
            {
                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.Identity.Name, $"Usuario con PK = {usuario.UsuarioPk} guardado correctamente");
                return Json(new { status = "OK" });
            }

            Func_Comunes.LogsAplicacion(TiposLogs.ERROR, User.Identity.Name, $"Error al guardar el usuario con PK = {usuario.UsuarioPk}");
            return Json(new { status = "KO", message = "Error al guardar el usuario" });
        }

        public async Task<IActionResult> Delete(int usuarioPk)
        {
            Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.Identity.Name, $"Eliminando usuario con PK = {usuarioPk}");
            var result = await _usuarios.DeleteUsuarioAsync(usuarioPk);
            if (result == Resultado.OK)
            {
                Func_Comunes.LogsAplicacion(TiposLogs.INFO, User.Identity.Name, $"Usuario con PK = {usuarioPk} eliminado correctamente");
                return Json(new { status = "OK" });
            }

            Func_Comunes.LogsAplicacion(TiposLogs.ERROR, User.Identity.Name, $"Error al eliminar el usuario con PK = {usuarioPk}");
            return Json(new { status = "KO", message = "El usuario no ha sido eliminado" });
        }

        // Partial view para el calendario anual.
        public ActionResult YearCalendarPartial(int UsuarioPk, int year = 2025)
        {
            VM_CalendarioUsuario vm = _usuarios.GetYearCalendarViewModel(year, UsuarioPk).Result;

            var partial = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_calendarioUsuario.cshtml", vm);

            return Json(new { status = "OK", partial = partial });
        }

        // Partial view para el calendario mensual.
        public ActionResult MonthCalendarPartial(int year, int month, int UsuarioPk)
        {
            VM_Month vm = _usuarios.GetMonthCalendarViewModel(year, month, UsuarioPk).Result;

            var partial = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_calendarioMensualUsuario.cshtml", vm);

            return Json(new { status = "OK", partial = partial });
        }
    }
}
