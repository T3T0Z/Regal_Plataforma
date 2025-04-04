using Regal_Plataforma.Models.BDD;
using Regal_Plataforma.Models;

public interface IObraService
{
    Task<List<Obra>> GetObrasAsync();
    Task<Obra> GetObraByPkAsync(int ObraPk);
    Task<List<EstadoObra>> GestEstadosObrasAsync();
    Task<Resultado> CreateObraAsync(Obra obra);
    Task<Resultado> UpdateObraAsync(Obra obra);
    Task<Resultado> DeleteObraAsync(int ObraPk);
}
