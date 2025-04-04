using Microsoft.EntityFrameworkCore;
using Regal_Plataforma.Funciones;
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;
using System.Security.Claims;

public class NotasService : INotasServices
{
    private readonly REGAL_ASISTENCIAContext _dbContext;

    public NotasService(REGAL_ASISTENCIAContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CreateNotaSiniestroAsync(NotasSiniestro nota, int Usuario_PK)
    {
        try
        {
            nota.UsuarioPk = Usuario_PK;
            nota.Activo = true;

            await _dbContext.NotasSiniestros.AddAsync(nota);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateNotaSiniestroAsync(NotasSiniestro nota, int Usuario_PK)
    {
        try
        {
            var notaExistente = await _dbContext.NotasSiniestros.FirstOrDefaultAsync(x => x.NotasSiniestroPk == nota.NotasSiniestroPk);

            if (notaExistente == null)
                return false;

            notaExistente.Nota = nota.Nota;
            notaExistente.UsuarioPk = Usuario_PK;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteNotaSiniestroAsync(int NotasSiniestro_PK)
    {
        try
        {
            var nota = await _dbContext.NotasSiniestros.FirstOrDefaultAsync(n => n.NotasSiniestroPk == NotasSiniestro_PK);

            if (nota == null)
                return false;

            _dbContext.NotasSiniestros.Remove(nota);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<NotasSiniestro> GetNotaSiniestroByIdAsync(int NotasSiniestro_PK)
    {
        return await _dbContext.NotasSiniestros.Where(n => n.NotasSiniestroPk == NotasSiniestro_PK).Include(x=>x.UsuarioPkNavigation).FirstOrDefaultAsync();
    }


    public async Task<bool> CreateNotaClienteAsync(NotasCliente nota, int Usuario_PK)
    {
        try
        {
            nota.UsuarioPk = Usuario_PK;
            nota.Activo = true;

            await _dbContext.NotasClientes.AddAsync(nota);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateNotaClienteAsync(NotasCliente nota, int Usuario_PK)
    {
        try
        {
            var notaExistente = await _dbContext.NotasClientes.FirstOrDefaultAsync(x => x.NotasClientesPk == nota.NotasClientesPk);

            if (notaExistente == null)
                return false;

            notaExistente.Nota = nota.Nota;
            notaExistente.UsuarioPk = Usuario_PK;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteNotaClienteAsync(int NotasCliente_PK)
    {
        try
        {
            var nota = await _dbContext.NotasClientes.FirstOrDefaultAsync(n => n.NotasClientesPk == NotasCliente_PK);

            if (nota == null)
                return false;

            _dbContext.NotasClientes.Remove(nota);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<NotasCliente> GetNotaClienteByIdAsync(int NotasCliente_PK)
    {
        return await _dbContext.NotasClientes
            .Where(n => n.NotasClientesPk == NotasCliente_PK)
            .Include(x => x.UsuarioPkNavigation)
            .FirstOrDefaultAsync();
    }


    public async Task<bool> CreateNotaObraAsync(NotasObra nota, int Usuario_PK)
    {
        try
        {
            nota.UsuarioPk = Usuario_PK;
            nota.Activo = true;

            await _dbContext.NotasObras.AddAsync(nota);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateNotaObraAsync(NotasObra nota, int Usuario_PK)
    {
        try
        {
            var notaExistente = await _dbContext.NotasObras.FirstOrDefaultAsync(x => x.NotasObrasPk == nota.NotasObrasPk);

            if (notaExistente == null)
                return false;

            notaExistente.Nota = nota.Nota;
            notaExistente.UsuarioPk = Usuario_PK;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteNotaObraAsync(int NotasObra_PK)
    {
        try
        {
            var nota = await _dbContext.NotasObras.FirstOrDefaultAsync(n => n.NotasObrasPk == NotasObra_PK);

            if (nota == null)
                return false;

            _dbContext.NotasObras.Remove(nota);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<NotasObra> GetNotaObraByIdAsync(int NotasObras_PK)
    {
        return await _dbContext.NotasObras
            .Where(n => n.NotasObrasPk == NotasObras_PK)
            .Include(x => x.UsuarioPkNavigation)
            .FirstOrDefaultAsync();
    }
}
