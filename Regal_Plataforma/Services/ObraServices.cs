using Regal_Plataforma.Models.BDD;
using Regal_Plataforma.Models;
using Microsoft.EntityFrameworkCore;

public class ObraService : IObraService
{
    private readonly REGAL_ASISTENCIAContext _context;

    public ObraService(REGAL_ASISTENCIAContext context)
    {
        _context = context;
    }

    public async Task<List<Obra>> GetObrasAsync()
    {
        return await _context.Obras
            .Include(x => x.ClientePkNavigation)
            .Include(x => x.DireccionPkNavigation)
            .Include(x => x.EstadoObraPkNavigation)
            .Include(x => x.UgestorPkNavigation)
            .Include(x => x.Trabajos)
            .Include(x => x.NotasObras)
            .ThenInclude(x => x.UsuarioPkNavigation)
            .Where(x => x.Activo)
            .ToListAsync();
    }

    public async Task<Obra> GetObraByPkAsync(int ObraPk)
    {
        return await _context.Obras
            .Include(x => x.ClientePkNavigation)
            .Include(x => x.DireccionPkNavigation)
            .Include(x=>x.EstadoObraPkNavigation)
            .Include(x => x.UgestorPkNavigation)
            .Include(x => x.NotasObras)
            .ThenInclude(x=>x.UsuarioPkNavigation)
            .FirstOrDefaultAsync(x => x.ObraPk == ObraPk);
    }

    public async Task<List<EstadoObra>> GestEstadosObrasAsync()
    {
        return await _context.EstadoObras.Where(x=>x.Activo).ToListAsync();
    }

    public async Task<Resultado> CreateObraAsync(Obra obra)
    {
        using var trans = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Direccions.Add(obra.DireccionPkNavigation);
            await _context.SaveChangesAsync();

            obra.DireccionPk = obra.DireccionPkNavigation.DireccionPk;
            obra.Activo = true;
            _context.Obras.Add(obra);
            await _context.SaveChangesAsync();

            await trans.CommitAsync();
            return Resultado.OK;
        }
        catch
        {
            await trans.RollbackAsync();
            return Resultado.KO;
        }
    }

    public async Task<Resultado> UpdateObraAsync(Obra obra)
    {
        using var trans = await _context.Database.BeginTransactionAsync();
        try
        {
            var obraExistente = await _context.Obras.FindAsync(obra.ObraPk);
            if (obraExistente == null) return Resultado.KO;

            var direccion = await _context.Direccions.FindAsync(obraExistente.DireccionPk);
            direccion.NombreVia = obra.DireccionPkNavigation.NombreVia;
            direccion.Poblacion = obra.DireccionPkNavigation.Poblacion;
            direccion.CodigoPostal = obra.DireccionPkNavigation.CodigoPostal;

            _context.Direccions.Update(direccion);

            obraExistente.Nombre = obra.Nombre;
            obraExistente.Fecha = obra.Fecha;
            obraExistente.FechaAsignada = obra.FechaAsignada;
            obraExistente.FechaFin = obra.FechaFin;
            obraExistente.FechaFinAsignada = obra.FechaFinAsignada;
            obraExistente.ClientePk = obra.ClientePk;

            _context.Obras.Update(obraExistente);
            await _context.SaveChangesAsync();
            await trans.CommitAsync();

            return Resultado.OK;
        }
        catch
        {
            await trans.RollbackAsync();
            return Resultado.KO;
        }
    }

    public async Task<Resultado> DeleteObraAsync(int ObraPk)
    {
        try
        {
            var obra = await _context.Obras.FindAsync(ObraPk);
            if (obra != null)
            {
                obra.Activo = false;
                await _context.SaveChangesAsync();
            }
            return Resultado.OK;
        }
        catch
        {
            return Resultado.KO;
        }
    }
}
