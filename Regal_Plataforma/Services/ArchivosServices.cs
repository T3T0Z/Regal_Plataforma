
// Services/ArchivoService.cs
using Microsoft.AspNetCore.Mvc;
using Regal_Plataforma.Models;
public class ArchivoService : IArchivoService
{
    private readonly string _basePath = "wwwroot/Archivos";

    public async Task<List<ArchivoDto>> ObtenerArchivosAsync(string entidad, int entidadPk)
    {
        var path = Path.Combine(_basePath, entidad, entidadPk.ToString());
        return await LeerArchivosDesdeRuta(path);
    }

    public async Task<List<ArchivoDto>> ObtenerArchivosYRelacionadosAsync(string entidad, int entidadPk)
    {
        var lista = await ObtenerArchivosAsync(entidad, entidadPk);

        if (entidad == "Obra" || entidad == "Siniestro")
        {
            string trabajoPath = Path.Combine(_basePath, "Trabajo");
            if (Directory.Exists(trabajoPath))
            {
                var carpetasTrabajo = Directory.GetDirectories(trabajoPath);
                foreach (var carpeta in carpetasTrabajo)
                {
                    var nombre = Path.GetFileName(carpeta);
                    if (int.TryParse(nombre, out int trabajoPk))
                    {
                        // Puedes aquí enlazar con la DB si necesitas validar que pertenece a la obra o siniestro
                        var archivosTrabajo = await LeerArchivosDesdeRuta(carpeta, true, trabajoPk);
                        lista.AddRange(archivosTrabajo);
                    }
                }
            }
        }

        return lista;
    }
    public async Task<bool> GuardarArchivosAsync(string entidad, int entidadPk, List<IFormFile> archivos, string usuario)
    {
        try
        {
            var dirDestino = Path.Combine(_basePath, entidad, entidadPk.ToString());
            Directory.CreateDirectory(dirDestino);

            foreach (var archivo in archivos)
            {
                // Nombre base y extensión
                var nombre = $"{usuario}-{Path.GetFileNameWithoutExtension(archivo.FileName)}";
                var extension = Path.GetExtension(archivo.FileName);

                var rutaDestino = Path.Combine(dirDestino, archivo.FileName);
                int contador = 1;

                // ‑‑ Si el fichero existe, buscamos un nombre libre ------------
                while (System.IO.File.Exists(rutaDestino))
                {
                    var nuevoNombre = $"{nombre} ({contador++}){extension}";
                    rutaDestino = Path.Combine(dirDestino, nuevoNombre);
                }

                // Guardamos el archivo
                await using var stream = new FileStream(rutaDestino, FileMode.CreateNew);
                await archivo.CopyToAsync(stream);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
    //public async Task<bool> GuardarArchivosAsync(string entidad, int entidadPk, List<IFormFile> archivos)
    //{
    //    try
    //    {
    //        var path = Path.Combine(_basePath, entidad, entidadPk.ToString());
    //        Directory.CreateDirectory(path);

    //        foreach (var archivo in archivos)
    //        {
    //            var ruta = Path.Combine(path, archivo.FileName);
    //            using var stream = new FileStream(ruta, FileMode.Create);
    //            await archivo.CopyToAsync(stream);
    //        }

    //        return true;
    //    }
    //    catch
    //    {
    //        return false;
    //    }
    //}

    public async Task<bool> EliminarArchivoAsync(string rutaRelativa)
    {
        try
        {
            var rutaCompleta = Path.Combine("wwwroot", rutaRelativa.TrimStart('/'));
            if (System.IO.File.Exists(rutaCompleta))
            {
                System.IO.File.Delete(rutaCompleta);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<FileStreamResult> ObtenerArchivoAsync(string rutaRelativa)
    {
        var rutaCompleta = Path.Combine("wwwroot", rutaRelativa.TrimStart('/'));
        var stream = new FileStream(rutaCompleta, FileMode.Open, FileAccess.Read);
        var extension = Path.GetExtension(rutaCompleta).ToLowerInvariant();
        var mimeType = extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
        return new FileStreamResult(stream, mimeType);
    }

    private async Task<List<ArchivoDto>> LeerArchivosDesdeRuta(string ruta, bool esTrabajo = false, int? trabajoPk = null)
    {
        var lista = new List<ArchivoDto>();
        if (!Directory.Exists(ruta)) return lista;

        var archivos = Directory.GetFiles(ruta);
        foreach (var archivo in archivos)
        {
            var nombre = Path.GetFileName(archivo);
            var extension = Path.GetExtension(nombre);
            var relativa = archivo.Replace("wwwroot", "").Replace("\\", "/");
            
            if(nombre != "firma.png")
            {
                lista.Add(new ArchivoDto
                {
                    NombreArchivo = nombre,
                    Ruta = relativa,
                    Extension = extension,
                    EsDeTrabajo = esTrabajo,
                    TrabajoPk = trabajoPk
                });
            }
        }

        return lista;
    }
}
