using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;

public interface IOrderDatosServices
{
    Task<List<OrderDato>> GetOrderDatos();
    Task<OrderDato> GetOrderDatosByPK(int OrderDatosPK);
    Task<Resultado> SetGremioAsync(int OrderDatosPK, int GremioPK);
}