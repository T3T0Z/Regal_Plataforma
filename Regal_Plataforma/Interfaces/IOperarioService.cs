// Interfaz de servicio
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;

public interface IOperarioService
{
    Task<Trabajo> ObtenerTrabajoAsignadoAsync(int usuarioPk);
    Task<M_Resultado> UpdateTrabajoAsync(Trabajo trabajo);
    Task<Trabajo> CreateTrabajoLibreAsync(Trabajo trabajo);
    Task<M_Resultado> GuardarUbicacionTrabajoAsync(UbicacionesTrabajo ubicacion);
    Task<DescansosTrabajo> GuardarDescansoTrabajoAsync(DescansosTrabajo descansoTrabajo);
    Task<DescansosTrabajo> ObtenerDescansoTrabajoAsync(int descansoTrabajoPk);
}

