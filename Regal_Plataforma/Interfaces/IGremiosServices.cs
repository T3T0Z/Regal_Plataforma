using Regal_Plataforma.Models.BDD;

public interface IGremiosServices
{
    Task<List<Gremio>> GetGremiosAsync();
}