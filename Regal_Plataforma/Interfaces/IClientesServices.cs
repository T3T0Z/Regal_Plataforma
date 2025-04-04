using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;

public interface IClienteService
{
    Task<List<Cliente>> GetClientesAsync();
    Task<List<TiposCliente>> GetTiposClientesAsync();
    Task<Cliente> GetClienteByPkAsync(int ClientePk);
    Task<Resultado> CreateClienteAsync(Cliente cliente);
    Task<Resultado> UpdateClienteAsync(Cliente cliente);
    Task<Resultado> DeleteClienteAsync(int ClientePk);
}
