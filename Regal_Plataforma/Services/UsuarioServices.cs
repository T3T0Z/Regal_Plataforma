using Microsoft.EntityFrameworkCore;
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;
using System.Globalization;
using System.Net;

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
            .Include(x => x.NotasUsuarioUcreadorPkNavigations)
            .Include(x => x.NotasUsuarioUsuarioPkNavigations)
            .Include(x => x.TrabajoUgestorPkNavigations)
            .Include(x => x.TrabajoUasignadoPkNavigations)
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
            .Include(x => x.NotasUsuarioUcreadorPkNavigations)
            .Include(x => x.NotasUsuarioUsuarioPkNavigations)
            .Include(x => x.TrabajoUgestorPkNavigations)
            .Include(x => x.TrabajoUasignadoPkNavigations)
            .Include(x => x.AsignacionTrabajoUsuarioCreadorPkNavigations)
            .Include(x => x.AsignacionTrabajoUsuarioOperarioPkNavigations)
            .FirstOrDefaultAsync();
    }

    // Obtener todos los gestores
    public async Task<List<Usuario>> GetGestoresAsync()
    {
        return await _dbContext.Usuarios
            .Where(x => x.Activo && x.RolPk == 1)
            .Include(x => x.RolPkNavigation)
            .Include(x => x.TrabajoUgestorPkNavigations)
            .Include(x => x.TrabajoUasignadoPkNavigations)
            .Include(x => x.AsignacionTrabajoUsuarioCreadorPkNavigations)
            .Include(x => x.AsignacionTrabajoUsuarioOperarioPkNavigations)
            .ToListAsync();
    }

    // Obtener todos los gestores
    public async Task<List<Usuario>> GetOperariosAsync()
    {
        return await _dbContext.Usuarios
            .Where(x => x.Activo && x.RolPk == 2)
            .Include(x => x.RolPkNavigation)
            .Include(x => x.TrabajoUgestorPkNavigations)
            .Include(x => x.TrabajoUasignadoPkNavigations)
            .Include(x => x.AsignacionTrabajoUsuarioCreadorPkNavigations)
            .Include(x => x.AsignacionTrabajoUsuarioOperarioPkNavigations)
            .ToListAsync();
    }

    // Obtener todos los gestores
    public async Task<List<Usuario>> GetExternosAsync()
    {
        return await _dbContext.Usuarios
            .Where(x => x.Activo && x.RolPk == 3)
            .Include(x => x.RolPkNavigation)
            .Include(x => x.TrabajoUgestorPkNavigations)
            .Include(x => x.TrabajoUasignadoPkNavigations)
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
            .Include(x => x.TrabajoUasignadoPkNavigations)
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

    public async Task<VM_Month> GetMonthCalendarViewModel(int year, int month, int UsuarioPk)
    {
        var calendar = new VM_Month{ Month = month, Year = year, actualMonth = month == DateTime.Now.Month ? true : false };
        DateTime firstDayOfMonth = new DateTime(year, month, 1);

        // Ajuste: calcular el inicio del calendario como el lunes anterior al primer día del mes.
        int diff = (firstDayOfMonth.DayOfWeek == DayOfWeek.Sunday) ? 6 : ((int)firstDayOfMonth.DayOfWeek - 1);
        DateTime calendarStartDate = firstDayOfMonth.AddDays(-diff);

        // Se visualizan 6 semanas (42 días) en el calendario
        int totalDays = 42;
        VM_Week currentWeek = null;

        for (int i = 0; i < totalDays; i++)
        {
            DateTime currentDate = calendarStartDate.AddDays(i);
            bool isCurrentMonth = currentDate.Month == month;

            // Ejemplo: asignar un trabajo dummy para cada tercer día del mes
            var trabajosUsuariosDia = _dbContext.Trabajos.Where(x => x.UasignadoPk == UsuarioPk && x.FechaAsignada.Date == currentDate.Date && x.Activo).ToList();

            VM_listaTrabajos trabajos = new VM_listaTrabajos{
                listTrabajos = trabajosUsuariosDia
            };

            var day = new VM_Day
            {
                Date = currentDate,
                IsCurrentMonth = isCurrentMonth,
                Trabajos = trabajos
            };

            if (currentDate.Day == DateTime.Now.Day && currentDate.Month == DateTime.Now.Month && currentDate.Year == DateTime.Now.Year)
                day.actualDay = true;

            // Iniciar una nueva semana cada lunes (lunes es el primer día de la semana)
            if (currentDate.DayOfWeek == DayOfWeek.Monday)
            {
                currentWeek = new VM_Week();
                calendar.Weeks.Add(currentWeek);
            }

            currentWeek.Days.Add(day);
        }

        calendar.UsuarioPk = UsuarioPk;
        calendar.Holidays = vacaciones;

        return calendar;
    }

    public async Task<VM_CalendarioUsuario> GetYearCalendarViewModel(int year, int UsuarioPk)
    {
        var yearCalendar = new VM_CalendarioUsuario { Year = year, Holidays = vacaciones };
        for (int month = 1; month <= 12; month++)
        {
            var monthlyCalendar = GetMonthCalendarViewModel(year, month, UsuarioPk);
            yearCalendar.Calendars.Add(await monthlyCalendar);
        }

        yearCalendar.UsuarioPk = UsuarioPk;

        return yearCalendar;
    }

    public List<DateTime> vacaciones = new List<DateTime>
    {
        new DateTime(2025, 1, 1),    // Año Nuevo
        new DateTime(2025, 1, 6),    // Reyes Magos

        new DateTime(2025, 3, 3),   // Construccion
        new DateTime(2025, 3, 4),   // Vigo
        new DateTime(2025, 3, 5),   // Construccion
        new DateTime(2025, 3, 28),   // Vigo

        new DateTime(2025, 4, 14),   // Construccion
        new DateTime(2025, 4, 15),   // Construccion
        new DateTime(2025, 4, 16),   // Construccion
        new DateTime(2025, 4, 17),   // Jueves Santo
        new DateTime(2025, 4, 18),   // Viernes Santo

        new DateTime(2025, 5, 1),    // Día del Trabajo
        new DateTime(2025, 5, 2),    // Construccion
        new DateTime(2025, 5, 17),    // Letras Gallegas

        new DateTime(2025, 7, 24),    // Construccion
        new DateTime(2025, 7, 25),    // Día del Trabajo

        new DateTime(2025, 8, 14),   // Construccion
        new DateTime(2025, 8, 15),   // Asunción de la Virgen

        new DateTime(2025, 10, 31),  // Construccion

        new DateTime(2025, 11, 1),   // Todos los Santos

        new DateTime(2025, 12, 6),   // Día de la Constitución
        new DateTime(2025, 12, 8),   // Inmaculada Concepción
        new DateTime(2025, 12, 24),   // Construccion
        new DateTime(2025, 12, 25),   // Navidad
        new DateTime(2025, 12, 26),   // Construccion
        new DateTime(2025, 12, 31)   // Construccion
    };
}
