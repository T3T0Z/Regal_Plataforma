using Microsoft.EntityFrameworkCore;
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;

public class ClienteService : IClienteService
{
    private readonly REGAL_ASISTENCIAContext _context;

    public ClienteService(REGAL_ASISTENCIAContext context)
    {
        _context = context;
    }

    public async Task<List<Cliente>> GetClientesAsync()
    {
        return await _context.Clientes
            .Include(c => c.TipoClientePkNavigation)
            .Include(c => c.DireccionPkNavigation)
            .Include(c => c.NotasClientes)
            .ThenInclude(x => x.UsuarioPkNavigation)
            .Where(c => c.Activo)
            .ToListAsync();
    }

    public async Task<List<TiposCliente>> GetTiposClientesAsync()
    {
        return await _context.TiposClientes
            .Where(c => c.Activo)
            .ToListAsync();
    }

    public async Task<Cliente> GetClienteByPkAsync(int ClientePk)
    {
        return await _context.Clientes
            .Where(x=>x.ClientePk == ClientePk)
            .Include(c => c.TipoClientePkNavigation)
            .Include(c => c.DireccionPkNavigation)
            .Include(c => c.NotasClientes)
            .ThenInclude(x => x.UsuarioPkNavigation)
            .FirstOrDefaultAsync();
    }

    public async Task<Resultado> CreateClienteAsync(Cliente cliente)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            Direccion direccion = cliente.DireccionPkNavigation;
            // Crear dirección
            _context.Direccions.Add(direccion);
            await _context.SaveChangesAsync();

            // Asignar dirección al cliente
            cliente.DireccionPk = direccion.DireccionPk;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return Resultado.OK;
        }
        catch
        {
            await transaction.RollbackAsync();
            return Resultado.KO;
        }
    }

    public async Task<Resultado> UpdateClienteAsync(Cliente cliente)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var clienteExistente = await _context.Clientes.FindAsync(cliente.ClientePk);
            if (clienteExistente == null)
                return Resultado.KO;

            Direccion direccionExistente = await _context.Direccions.FindAsync(clienteExistente.DireccionPk);
            Direccion direccion = cliente.DireccionPkNavigation;

            // Actualizar la dirección si existe, sino crearla
            if (direccionExistente != null)
            {
                direccionExistente.NombreVia = direccion.NombreVia;
                direccionExistente.Poblacion = direccion.Poblacion;
                direccionExistente.CodigoPostal = direccion.CodigoPostal;
                direccionExistente.Provincia = direccion.Provincia;
                direccionExistente.Pais = direccion.Pais;

                _context.Direccions.Update(direccionExistente);
            }
            else
            {
                _context.Direccions.Add(direccion);
                await _context.SaveChangesAsync();
                clienteExistente.DireccionPk = direccion.DireccionPk;
            }

            // Actualizar cliente
            clienteExistente.Nombre = cliente.Nombre;
            clienteExistente.DocIdentidad = cliente.DocIdentidad;
            clienteExistente.Correo = cliente.Correo;
            clienteExistente.Movil = cliente.Movil;
            clienteExistente.Telefono = cliente.Telefono;
            clienteExistente.Ncuenta = cliente.Ncuenta;
            clienteExistente.NombreAdministracion = cliente.NombreAdministracion;
            clienteExistente.TipoClientePk = cliente.TipoClientePk;
            clienteExistente.Notas = cliente.Notas;

            _context.Clientes.Update(clienteExistente);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return Resultado.OK;
        }
        catch
        {
            await transaction.RollbackAsync();
            return Resultado.KO;
        }
    }

    public async Task<Resultado> DeleteClienteAsync(int ClientePk)
    {
        try
        {
            var cliente = await _context.Clientes.FindAsync(ClientePk);
            if (cliente != null)
            {
                cliente.Activo = false;
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
