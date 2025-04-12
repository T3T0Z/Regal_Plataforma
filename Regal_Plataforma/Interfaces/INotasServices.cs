using Regal_Plataforma.Models.BDD;

public interface INotasServices
{
    Task<bool> CreateNotaSiniestroAsync(NotasSiniestro nota, int Usuario_PK);
    Task<bool> UpdateNotaSiniestroAsync(NotasSiniestro nota, int Usuario_PK);
    Task<bool> DeleteNotaSiniestroAsync(int NotasSiniestro_PK);
    Task<NotasSiniestro> GetNotaSiniestroByIdAsync(int NotasSiniestro_PK);
    Task<bool> CreateNotaClienteAsync(NotasCliente nota, int Usuario_PK);
    Task<bool> UpdateNotaClienteAsync(NotasCliente nota, int Usuario_PK);
    Task<bool> DeleteNotaClienteAsync(int NotasCliente_PK);
    Task<NotasCliente> GetNotaClienteByIdAsync(int NotasCliente_PK);
    Task<bool> CreateNotaObraAsync(NotasObra nota, int Usuario_PK);
    Task<bool> UpdateNotaObraAsync(NotasObra nota, int Usuario_PK);
    Task<bool> DeleteNotaObraAsync(int NotasObra_PK);
    Task<NotasObra> GetNotaObraByIdAsync(int NotasObra_PK);
    Task<bool> CreateNotaTrabajoAsync(NotasTrabajo nota, int Usuario_PK);
    Task<bool> UpdateNotaTrabajoAsync(NotasTrabajo nota, int Usuario_PK);
    Task<bool> DeleteNotaTrabajoAsync(int NotasTrabajo_PK);
    Task<NotasTrabajo> GetNotaTrabajoByIdAsync(int NotasTrabajo_PK);
}
