using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Funciones;
using Regal_Plataforma.Models.BDD;
using Regal_Plataforma.Models;

[Authorize(Roles = "Administrador")]
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
            listUsuarios = await _usuarios.GetUsuariosAsync()
        };
        return View(vm);
    }

    public async Task<IActionResult> Detalles(int UsuarioPk)
    {
        var usuario = await _usuarios.GetUsuarioByPKAsync(UsuarioPk);

        if(usuario == null)
            return Json(new { status = "KO" });

        VM_DetallesUsuario vm = new VM_DetallesUsuario{ Usuario = usuario };

        var partial = _renderer.RenderPartialToStringAsync("~/Views/Shared/PartialViews/_detallesUsuario.cshtml", vm);

        return Json(new { status = "OK", partial = partial });
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

    public async Task<IActionResult> CreateEditUsuario(Usuario usuario)
    {
        var result = await _usuarios.CreateUpdateUsuarioAsync(usuario);
        if (result == Resultado.OK)
            return Json(new { status = "OK" });

        return Json(new { status = "KO", message = "Error al guardar el usuario" });
    }

    public async Task<IActionResult> Delete(int usuarioPk)
    {
        var result = await _usuarios.DeleteUsuarioAsync(usuarioPk);
        if (result == Resultado.OK)
            return Json(new { status = "OK" });

        return Json(new { status = "KO", message = "El usuario no ha sido eliminado" });
    }
}
