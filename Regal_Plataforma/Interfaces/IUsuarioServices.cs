using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;

public interface IUsuarioServices
{
    Task<List<Usuario>> GetUsuariosAsync();
    Task<Usuario> GetUsuarioByPKAsync(int usuarioPk);
    Task<Resultado> CreateUpdateUsuarioAsync(Usuario usuario);
    Task<Resultado> DeleteUsuarioAsync(int usuarioPk);
    Task<List<Usuario>> GetGestoresAsync();
    Task<List<Usuario>> GetOperariosAsync();
    Task<List<Usuario>> GetExternosAsync();
    Task<Usuario> GetUsuarioByUsuarioAsync(string usuario);
    Task<List<Role>> GetRolesAsync();
    Task<VM_Month> GetMonthCalendarViewModel(int year, int month, int UsuarioPk);
    Task<VM_CalendarioUsuario> GetYearCalendarViewModel(int year, int UsuarioPk);
}