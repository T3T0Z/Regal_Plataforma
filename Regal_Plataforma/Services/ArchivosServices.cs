
// Services/ArchivoService.cs
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Models;

public class ArchivoService : IArchivoService
{
    private readonly string _basePath = Path.Combine("wwwroot", "archivos");

    public async Task<List<ArchivoDto>> ObtenerArchivosAsync(string entidad, int entidadPk)
    {
        var path = Path.Combine(_basePath, entidad, entidadPk.ToString());
        if (!Directory.Exists(path)) return new List<ArchivoDto>();

        var files = Directory.GetFiles(path);
        return files.Select(f => new ArchivoDto
        {
            NombreArchivo = Path.GetFileName(f),
            Ruta = $"/archivos/{entidad}/{entidadPk}/{Path.GetFileName(f)}",
            Extension = Path.GetExtension(f).ToLower()
        }).ToList();
    }

    public async Task<bool> GuardarArchivosAsync(string entidad, int entidadPk, List<IFormFile> archivos)
    {
        try
        {
            var path = Path.Combine(_basePath, entidad, entidadPk.ToString());
            Directory.CreateDirectory(path);

            foreach (var archivo in archivos)
            {
                var ruta = Path.Combine(path, archivo.FileName);
                using var stream = new FileStream(ruta, FileMode.Create);
                await archivo.CopyToAsync(stream);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> EliminarArchivoAsync(string ruta)
    {
        try
        {
            var fullPath = Path.Combine("wwwroot", ruta.TrimStart('/'));
            if (File.Exists(fullPath)) File.Delete(fullPath);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
