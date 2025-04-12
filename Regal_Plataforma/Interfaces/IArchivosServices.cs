// Interfaces/IArchivoService.cs
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Models;

public interface IArchivoService
{
    Task<List<ArchivoDto>> ObtenerArchivosAsync(string entidad, int entidadPk);
    Task<bool> GuardarArchivosAsync(string entidad, int entidadPk, List<IFormFile> archivos);
    Task<bool> EliminarArchivoAsync(string ruta);
}