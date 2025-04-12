using Regal_Plataforma.Models.BDD;
using Regal_Plataforma.Models;

public interface ITrabajosServices
{
    Task<List<Trabajo>> GetTrabajosAsync();
    Task<Trabajo> GetTrabajoByPkAsync(int TrabajoPk);
    Task<List<Trabajo>> GetTrabajosByUsuarioPkAsync(int UsuarioPk);
    Task<List<Trabajo>> GetTrabajosBySiniestroPkAsync(int UsuarioPk);
    Task<List<Trabajo>> GetTrabajosByObraPkAsync(int UsuarioPk);
    Task<List<EstadosTrabajo>> GetEstadosTrabajosAsync();
    Task<M_Resultado> CreateTrabajoAsync(Trabajo trabajo);
    Task<M_Resultado> UpdateTrabajoAsync(Trabajo trabajo);
    Task<Resultado> DeleteTrabajoAsync(int TrabajoPk);
}
