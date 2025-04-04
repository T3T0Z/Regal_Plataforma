using Microsoft.EntityFrameworkCore;
using Regal_Plataforma.Models.BDD;

public class GremioServices : IGremiosServices
{
    private readonly REGAL_ASISTENCIAContext _dbContext;

    public GremioServices(REGAL_ASISTENCIAContext dbContext)
    {
        _dbContext = dbContext;
    }


    // Obtener todos los gremios
    public async Task<List<Gremio>> GetGremiosAsync()
    {
        return await _dbContext.Gremios.ToListAsync();
    }

}
