using Regal_Plataforma.Models.BDD;
using Regal_Plataforma.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Identity.Client;

public class TrabajosServices : ITrabajosServices
{
    private readonly REGAL_ASISTENCIAContext _context;

    public TrabajosServices(REGAL_ASISTENCIAContext context)
    {
        _context = context;
    }

    public async Task<List<Trabajo>> GetTrabajosAsync()
    {
        return await _context.Trabajos
            .Include(x=>x.SiniestroPkNavigation)
            .ThenInclude(x=>x.DetallesSiniestroPkNavigation)
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.OrderDatos)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x=>x.ObraPkNavigation)
            .ThenInclude(x=>x.DireccionPkNavigation)
            .Include(x=>x.UgestorPkNavigation)
            .Include(x=>x.UasignadoPkNavigation)
            .Include(x=>x.EstadoTrabajoPkNavigation)
            .Include(x=>x.NotasTrabajos)
            .ThenInclude(x => x.UsuarioPkNavigation)
            .Where(x => x.Activo)
            .ToListAsync();
    }

    public async Task<List<Trabajo>> GetTrabajosByUsuarioPkAsync(int UsuarioPk)
    {
        return await _context.Trabajos
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.DetallesSiniestroPkNavigation)
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.OrderDatos)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.ObraPkNavigation)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.UgestorPkNavigation)
            .Include(x => x.UasignadoPkNavigation)
            .Include(x => x.EstadoTrabajoPkNavigation)
            .Include(x => x.NotasTrabajos)
            .ThenInclude(x => x.UsuarioPkNavigation)
            .Where(x => x.Activo && x.UasignadoPk == UsuarioPk)
            .ToListAsync();
    }
    public async Task<List<Trabajo>> GetTrabajosBySiniestroPkAsync(int SiniestroPk)
    {
        return await _context.Trabajos
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.DetallesSiniestroPkNavigation)
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.OrderDatos)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.ObraPkNavigation)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.UgestorPkNavigation)
            .Include(x => x.UasignadoPkNavigation)
            .Include(x => x.EstadoTrabajoPkNavigation)
            .Include(x => x.NotasTrabajos)
            .ThenInclude(x => x.UsuarioPkNavigation)
            .Where(x => x.Activo && x.SiniestroPk == SiniestroPk)
            .ToListAsync();
    }
    public async Task<List<Trabajo>> GetTrabajosByObraPkAsync(int ObraPk)
    {
        return await _context.Trabajos
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.DetallesSiniestroPkNavigation)
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.OrderDatos)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.ObraPkNavigation)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.UgestorPkNavigation)
            .Include(x => x.UasignadoPkNavigation)
            .Include(x => x.EstadoTrabajoPkNavigation)
            .Include(x => x.NotasTrabajos)
            .ThenInclude(x => x.UsuarioPkNavigation)
            .Where(x => x.Activo && x.ObraPk == ObraPk)
            .ToListAsync();
    }

    public async Task<Trabajo> GetTrabajoByPkAsync(int TrabajoPk)
    {
        return await _context.Trabajos
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.DetallesSiniestroPkNavigation)
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.OrderDatos)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.ObraPkNavigation)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.UgestorPkNavigation)
            .Include(x => x.UasignadoPkNavigation)
            .Include(x => x.EstadoTrabajoPkNavigation)
            .Include(x => x.NotasTrabajos)
            .ThenInclude(x => x.UsuarioPkNavigation)
            .FirstOrDefaultAsync(x=>x.TrabajoPk == TrabajoPk);
    }

    public async Task<List<EstadosTrabajo>> GetEstadosTrabajosAsync()
    {
        return await _context.EstadosTrabajos
            .Where(x=>x.Activo)
            .ToListAsync();
    }

    public async Task<M_Resultado> CreateTrabajoAsync(Trabajo trabajo)
    {
        List<string> listErrores = new List<string>();
        if(ValidacionesTrabajo(trabajo, out listErrores) == Resultado.KO)
            return new M_Resultado { resultado = Resultado.KO , mensaje = string.Join("\n", listErrores) };

        using var trans = await _context.Database.BeginTransactionAsync();
        try
        {
            trabajo.Activo = true;
            _context.Trabajos.Add(trabajo);
            await _context.SaveChangesAsync();

            await trans.CommitAsync();
            return new M_Resultado{ resultado = Resultado.OK };
        }
        catch
        {
            await trans.RollbackAsync();
            return new M_Resultado { resultado = Resultado.KO };
        }
    }

    public async Task<M_Resultado> UpdateTrabajoAsync(Trabajo trabajo)
    {
        List<string> listErrores = new List<string>();
        if (ValidacionesTrabajo(trabajo, out listErrores) == Resultado.KO)
            return new M_Resultado { resultado = Resultado.KO, mensaje = string.Join("\n", listErrores) };

        using var trans = await _context.Database.BeginTransactionAsync();
        try
        {
            var trabajoExistente = await _context.Trabajos.FindAsync(trabajo.TrabajoPk);
            if (trabajoExistente == null) return new M_Resultado { resultado = Resultado.KO };

            trabajoExistente.ObraPk = trabajo.ObraPk;
            trabajoExistente.UgestorPk = trabajo.UgestorPk;
            trabajoExistente.UasignadoPk = trabajo.UasignadoPk;
            trabajoExistente.EstadoTrabajoPk = trabajo.EstadoTrabajoPk;
            trabajoExistente.FechaAsignada = trabajo.FechaAsignada;
            trabajoExistente.FechaFinAsignada = trabajo.FechaFinAsignada;
            trabajoExistente.FechaEjecucion = trabajo.FechaEjecucion;
            trabajoExistente.FechaFinEjecucion = trabajo.FechaFinEjecucion;
            trabajoExistente.Descripcion = trabajo.Descripcion;
            trabajoExistente.Fotos = trabajo.Fotos;
            trabajoExistente.HayFirma = trabajo.HayFirma;
            trabajoExistente.Dnifirma = trabajo.Dnifirma;
            trabajoExistente.HayDaños = trabajo.HayDaños;
            trabajoExistente.Daños = trabajo.Daños;
            trabajoExistente.Urgencia = trabajo.Urgencia;


            _context.Trabajos.Update(trabajoExistente);
            await _context.SaveChangesAsync();

            await trans.CommitAsync();
            return new M_Resultado { resultado = Resultado.OK };
        }
        catch
        {
            await trans.RollbackAsync();
            return new M_Resultado { resultado = Resultado.KO };
        }
    }

    public async Task<Resultado> DeleteTrabajoAsync(int TrabajoPk)
    {
        try
        {
            var trabajo = await _context.Trabajos.FindAsync(TrabajoPk);
            if (trabajo != null)
            {
                trabajo.Activo = false;
                await _context.SaveChangesAsync();
            }
            return Resultado.OK;
        }
        catch
        {
            return Resultado.KO;
        }
    }

    private Resultado ValidacionesTrabajo(Trabajo trabajo, out List<string> listErrores)
    {
        listErrores = new List<string>();
        if(trabajo.SiniestroPk == null && trabajo.ObraPk == null)
            listErrores.Add("Debes seleccionar al menos un Siniestro o una Obra");
        if(trabajo.SiniestroPk != null && trabajo.ObraPk != null)
            listErrores.Add("Debes seleccionar tan solo un Siniestro o una Obra");
        if(trabajo.UasignadoPk == 0)
            listErrores.Add("Debes asignar el trabajo a un usuario");
        if (trabajo.UgestorPk == 0)
            listErrores.Add("Debes asignar el trabajo a un gestor");
        if (trabajo.FechaAsignada == DateTime.MinValue)
            listErrores.Add("Debes asignar una fecha de partida aproximada");
        if (trabajo.FechaFinAsignada == DateTime.MinValue)
            listErrores.Add("Debes asignar una fecha de finalización aproximada");
        if (trabajo.FechaFinAsignada <= trabajo.FechaAsignada)
            listErrores.Add("Debes asignar una fecha de finalización posterior a la da inicio");

        if (listErrores.Count() == 0)
            return Resultado.OK;
        else
            return Resultado.KO;
    }
}
