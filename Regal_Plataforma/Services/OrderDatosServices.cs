using Microsoft.EntityFrameworkCore;
using Regal_Plataforma.Funciones;
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;

public class OrderDatosServices : IOrderDatosServices
{
    private readonly REGAL_ASISTENCIAContext _dbContext;

    public OrderDatosServices(REGAL_ASISTENCIAContext dbContext)
    {
        _dbContext = dbContext;
    }


    // Obtener todos los order datos
    public async Task<List<OrderDato>> GetOrderDatos()
    {
        return await _dbContext.OrderDatos.Where(x => x.Activo)
            .Include(x => x.OrderPkNavigation)
            .Include(x=>x.DireccionPkNavigation)
            .Include(x=>x.SiniestroPkNavigation)
            .ThenInclude(x=>x.NotasSiniestros)
            .ThenInclude(x=>x.UsuarioPkNavigation)
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.DetallesSiniestroPkNavigation)
            .Include(x=>x.PaseguradoPkNavigation)
            .Include(x=>x.PafectadaPkNavigation)
            .Include(x=>x.GremioPkNavigation)
            .ToListAsync();
    }

    public async Task<OrderDato> GetOrderDatosByPK(int OrderDatosPK)
    {
        return await _dbContext.OrderDatos.Where(x => x.OrderDatosPk == OrderDatosPK)
            .Include(x => x.OrderPkNavigation)
            .Include(x => x.DireccionPkNavigation)
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.NotasSiniestros)
            .ThenInclude(x => x.UsuarioPkNavigation)
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.DetallesSiniestroPkNavigation)
            .Include(x => x.PaseguradoPkNavigation)
            .ThenInclude(x=>x.DireccionPkNavigation)
            .Include(x => x.PafectadaPkNavigation)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.GremioPkNavigation)
            .FirstAsync(); 
    }

    public async Task<Resultado> SetGremioAsync(int OrderDatosPK, int GremioPK)
    {
        var order = await _dbContext.OrderDatos.Where(x=>x.OrderDatosPk == OrderDatosPK).FirstAsync();

        if (order == null)
        {
            return Resultado.KO;
        }

        _dbContext.OrderDatos.Entry(order).Property(x => x.GremioPk).CurrentValue = GremioPK;
        _dbContext.OrderDatos.Update(order);

        try
        {
            await _dbContext.SaveChangesAsync();
            return Resultado.OK;
        }
        catch (Exception ex)
        {
            return Resultado.KO;
        }
    }
}