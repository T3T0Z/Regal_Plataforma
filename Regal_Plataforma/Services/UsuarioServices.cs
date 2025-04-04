using Microsoft.EntityFrameworkCore;
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;

public class UsuarioServices : IUsuarioServices
{
    private readonly REGAL_ASISTENCIAContext _dbContext;

    public UsuarioServices(REGAL_ASISTENCIAContext dbContext)
    {
        _dbContext = dbContext;
    }


    // Obtener todos los usuarios
    public async Task<List<Usuario>> GetUsuariosAsync()
    {
        return await _dbContext.Usuarios
            .Where(x=>x.Activo)
            .Include(x => x.RolPkNavigation)
            .Include(x => x.TrabajoUgestorPkNavigations)
            .Include(x => x.TrabajoUoperarioPkNavigations)
            .Include(x => x.AsignacionTrabajoUsuarioCreadorPkNavigations)
            .Include(x => x.AsignacionTrabajoUsuarioOperarioPkNavigations)
            .ToListAsync();
    }

    // Obtener usuario por PK
    public async Task<Usuario> GetUsuarioByPKAsync(int usuarioPk)
    {
        return await _dbContext.Usuarios
            .Where(x=>x.UsuarioPk == usuarioPk)
            .Include(x => x.RolPkNavigation)
            .Include(x => x.TrabajoUgestorPkNavigations)
            .Include(x => x.TrabajoUoperarioPkNavigations)
            .Include(x => x.AsignacionTrabajoUsuarioCreadorPkNavigations)
            .Include(x => x.AsignacionTrabajoUsuarioOperarioPkNavigations)
            .FirstOrDefaultAsync();
    }

    // Obtener todos los gestores
    public async Task<List<Usuario>> GetGestoresAsync()
    {
        return await _dbContext.Usuarios
            .Where(x => x.Activo && x.Gestor)
            .Include(x => x.RolPkNavigation)
            .Include(x => x.TrabajoUgestorPkNavigations)
            .Include(x => x.TrabajoUoperarioPkNavigations)
            .Include(x => x.AsignacionTrabajoUsuarioCreadorPkNavigations)
            .Include(x => x.AsignacionTrabajoUsuarioOperarioPkNavigations)
            .ToListAsync();
    }

    // Obtener usuario por usuario
    public async Task<Usuario> GetUsuarioByUsuarioAsync(string usuario)
    {
        return await _dbContext.Usuarios
            .Where(x => x.Nombre == usuario)
            .Include(x => x.RolPkNavigation)
            .Include(x => x.TrabajoUgestorPkNavigations)
            .Include(x => x.TrabajoUoperarioPkNavigations)
            .Include(x => x.AsignacionTrabajoUsuarioCreadorPkNavigations)
            .Include(x => x.AsignacionTrabajoUsuarioOperarioPkNavigations)
            .FirstOrDefaultAsync();
    }

    // Obtener roles
    public async Task<List<Role>> GetRolesAsync()
    {
        return await _dbContext.Roles
            .Where(x => x.Activo)
            .ToListAsync();
    }
    public async Task<Resultado> CreateUpdateUsuarioAsync(Usuario usuario)
    {
        try
        {
            var usuarioExistente = await _dbContext.Usuarios.FindAsync(usuario.UsuarioPk);

            if (usuarioExistente != null)
            {
                // Update
                usuarioExistente.Nombre = usuario.Nombre;
                usuarioExistente.Apellidos = usuario.Apellidos;
                usuarioExistente.Usuario1 = usuario.Usuario1;
                usuarioExistente.Contrasena = usuario.Contrasena;
                usuarioExistente.RolPk = usuario.RolPk;

                _dbContext.Usuarios.Update(usuarioExistente);
            }
            else
            {
                // Create
                usuario.FechaCreacion = DateTime.Now;
                usuario.Activo = true;
                _dbContext.Usuarios.Add(usuario);
            }

            await _dbContext.SaveChangesAsync();
            return Resultado.OK;
        }
        catch
        {
            return Resultado.KO;
        }
    }

    public async Task<Resultado> DeleteUsuarioAsync(int usuarioPk)
    {
        try
        {
            var usuario = await _dbContext.Usuarios.FindAsync(usuarioPk);
            if (usuario == null) return Resultado.KO;
            usuario.Activo = false;
            await _dbContext.SaveChangesAsync();
            return Resultado.OK;
        }
        catch
        {
            return Resultado.KO;
        }
    }
}
