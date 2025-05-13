using iText.Forms;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Identity.Client;
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;

public class TrabajosServices : ITrabajosServices
{
    private readonly REGAL_ASISTENCIAContext _context;

    public TrabajosServices(REGAL_ASISTENCIAContext context)
    {
        _context = context;
    }

    public async Task<List<Trabajo>> GetTrabajosAsync()
    {
        return await _context.Trabajos
            .Include(x=>x.SiniestroPkNavigation)
            .ThenInclude(x=>x.DetallesSiniestroPkNavigation)
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.OrderDatos)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x=>x.ObraPkNavigation)
            .ThenInclude(x=>x.DireccionPkNavigation)
            .Include(x=>x.UgestorPkNavigation)
            .Include(x=>x.UasignadoPkNavigation)
            .Include(x=>x.EstadoTrabajoPkNavigation)
            .Include(x=>x.NotasTrabajos)
            .ThenInclude(x => x.UsuarioPkNavigation)
            .Include(x => x.DescansosTrabajos)
            .Include(x => x.UbicacionesTrabajos)
            .Where(x => x.Activo)
            .ToListAsync();
    }

    public async Task<List<Trabajo>> GetTrabajosByUsuarioPkAsync(int UsuarioPk)
    {
        return await _context.Trabajos
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.DetallesSiniestroPkNavigation)
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.OrderDatos)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.ObraPkNavigation)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.UgestorPkNavigation)
            .Include(x => x.UasignadoPkNavigation)
            .Include(x => x.EstadoTrabajoPkNavigation)
            .Include(x => x.NotasTrabajos)
            .ThenInclude(x => x.UsuarioPkNavigation)
            .Include(x => x.DescansosTrabajos)
            .Include(x => x.UbicacionesTrabajos)
            .Where(x => x.Activo && x.UasignadoPk == UsuarioPk)
            .ToListAsync();
    }
    public async Task<List<Trabajo>> GetTrabajosBySiniestroPkAsync(int SiniestroPk)
    {
        return await _context.Trabajos
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.DetallesSiniestroPkNavigation)
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.OrderDatos)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.ObraPkNavigation)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.UgestorPkNavigation)
            .Include(x => x.UasignadoPkNavigation)
            .Include(x => x.EstadoTrabajoPkNavigation)
            .Include(x => x.NotasTrabajos)
            .ThenInclude(x => x.UsuarioPkNavigation)
            .Include(x => x.DescansosTrabajos)
            .Include(x => x.UbicacionesTrabajos)
            .Where(x => x.Activo && x.SiniestroPk == SiniestroPk)
            .ToListAsync();
    }
    public async Task<List<Trabajo>> GetTrabajosByObraPkAsync(int ObraPk)
    {
        return await _context.Trabajos
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.DetallesSiniestroPkNavigation)
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.OrderDatos)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.ObraPkNavigation)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.UgestorPkNavigation)
            .Include(x => x.UasignadoPkNavigation)
            .Include(x => x.EstadoTrabajoPkNavigation)
            .Include(x => x.NotasTrabajos)
            .ThenInclude(x => x.UsuarioPkNavigation)
            .Include(x => x.DescansosTrabajos)
            .Include(x => x.UbicacionesTrabajos)
            .Where(x => x.Activo && x.ObraPk == ObraPk)
            .ToListAsync();
    }

    public async Task<Trabajo> GetTrabajoByPkAsync(int TrabajoPk)
    {
        return await _context.Trabajos
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.DetallesSiniestroPkNavigation)
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.OrderDatos)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.ObraPkNavigation)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.UgestorPkNavigation)
            .Include(x => x.UasignadoPkNavigation)
            .Include(x => x.EstadoTrabajoPkNavigation)
            .Include(x => x.NotasTrabajos)
            .ThenInclude(x => x.UsuarioPkNavigation)
            .Include(x => x.DescansosTrabajos)
            .Include(x => x.UbicacionesTrabajos)
            .FirstOrDefaultAsync(x=>x.TrabajoPk == TrabajoPk);
    }

    public async Task<List<EstadosTrabajo>> GetEstadosTrabajosAsync()
    {
        return await _context.EstadosTrabajos
            .Where(x=>x.Activo)
            .ToListAsync();
    }

    public async Task<M_Resultado> CreateTrabajoAsync(Trabajo trabajo)
    {
        List<string> listErrores = new List<string>();
        if(ValidacionesTrabajo(trabajo, out listErrores) == Resultado.KO)
            return new M_Resultado { resultado = Resultado.KO , mensaje = string.Join("\n", listErrores) };

        using var trans = await _context.Database.BeginTransactionAsync();
        try
        {
            if (trabajo.FechaAsignada.Date != trabajo.FechaFinAsignada.Date)
            {
                var fechaFin = trabajo.FechaFinAsignada.Date;
                while (trabajo.FechaAsignada.Date <= fechaFin.Date)
                {
                    trabajo.FechaAsignada = new DateTime(trabajo.FechaAsignada.Year, trabajo.FechaAsignada.Month, trabajo.FechaAsignada.Day, 8, 30, 0, 0, 0);
                    trabajo.FechaFinAsignada = new DateTime(trabajo.FechaAsignada.Year, trabajo.FechaAsignada.Month, trabajo.FechaAsignada.Day, 16, 30, 0, 0, 0);
                    trabajo.TrabajoPk = 0;

                    if (trabajo.EstadoTrabajoPk == 3)
                    {
                        double descanso = 0;
                        if (trabajo.DescansosTrabajos.Count() > 0)
                        {
                            foreach (var item in trabajo.DescansosTrabajos.ToList())
                            {
                                descanso += (item.FechaFin?.TimeOfDay - item.FechaInicio.TimeOfDay).Value.TotalMinutes;
                            }
                        }
                        trabajo.TiempoTranscurridoDescansos = descanso.ToString("#");
                        trabajo.TiempoTransacurridoTrabajo = ((trabajo.FechaFinEjecucion?.TimeOfDay - trabajo.FechaEjecucion?.TimeOfDay).Value.TotalMinutes - descanso).ToString("#");
                    }

                    trabajo.Activo = true;
                    _context.Trabajos.Add(trabajo);
                    await _context.SaveChangesAsync();

                    trabajo.FechaAsignada = trabajo.FechaAsignada.AddDays(1);
                    trabajo.FechaFinAsignada = trabajo.FechaFinAsignada.AddDays(1);
                }
            }
            else
            {
                if (trabajo.EstadoTrabajoPk == 3)
                {
                    double descanso = 0;
                    if (trabajo.DescansosTrabajos.Count() > 0)
                    {
                        foreach (var item in trabajo.DescansosTrabajos.ToList())
                        {
                            descanso += (item.FechaFin?.TimeOfDay - item.FechaInicio.TimeOfDay).Value.TotalMinutes;
                        }
                    }
                    trabajo.TiempoTranscurridoDescansos = descanso.ToString("#");
                    trabajo.TiempoTransacurridoTrabajo = ((trabajo.FechaFinEjecucion?.TimeOfDay - trabajo.FechaEjecucion?.TimeOfDay).Value.TotalMinutes - descanso).ToString("#");
                }

                trabajo.Activo = true;
                _context.Trabajos.Add(trabajo);
                await _context.SaveChangesAsync();


            }
            await trans.CommitAsync();

            //ENVIAR AVISO A USUARIO SI ESTA LIBRE
            //bool usuarioLibre = _context.Trabajos.Any(x => x.UasignadoPk == trabajo.UasignadoPk && x.FechaAsignada.Date == DateTime.Now.Date && x.EstadoTrabajoPk == 4 && x.FechaFinEjecucion == null);
            //if(usuarioLibre)
            //{

            //}

            return new M_Resultado{ resultado = Resultado.OK };
        }
        catch
        {
            await trans.RollbackAsync();
            return new M_Resultado { resultado = Resultado.KO };
        }
    }

    public async Task<M_Resultado> UpdateTrabajoAsync(Trabajo trabajo)
    {
        List<string> listErrores = new List<string>();
        if (ValidacionesTrabajo(trabajo, out listErrores) == Resultado.KO)
            return new M_Resultado { resultado = Resultado.KO, mensaje = string.Join("\n", listErrores) };

        using var trans = await _context.Database.BeginTransactionAsync();
        try
        {
            var trabajoExistente = await _context.Trabajos.FindAsync(trabajo.TrabajoPk);
            if (trabajoExistente == null) return new M_Resultado { resultado = Resultado.KO };

            if (trabajo.EstadoTrabajoPk == 3)
            {
                double descanso = 0;
                if (trabajoExistente.DescansosTrabajos.Count() > 0)
                {
                    foreach (var item in trabajoExistente.DescansosTrabajos.ToList())
                    {
                        descanso += (item.FechaFin?.TimeOfDay - item.FechaInicio.TimeOfDay).Value.TotalMinutes;
                    }
                }
                trabajoExistente.TiempoTranscurridoDescansos = descanso.ToString("#");
                trabajoExistente.TiempoTransacurridoTrabajo = ((trabajo.FechaFinEjecucion?.TimeOfDay - trabajo.FechaEjecucion?.TimeOfDay).Value.TotalMinutes - descanso).ToString("#");
            }

            trabajoExistente.ObraPk = trabajo.ObraPk;
            trabajoExistente.UgestorPk = trabajo.UgestorPk;
            trabajoExistente.UasignadoPk = trabajo.UasignadoPk;
            trabajoExistente.EstadoTrabajoPk = trabajo.EstadoTrabajoPk;
            trabajoExistente.FechaAsignada = trabajo.FechaAsignada;
            trabajoExistente.FechaFinAsignada = trabajo.FechaFinAsignada;
            trabajoExistente.FechaEjecucion = trabajo.FechaEjecucion;
            trabajoExistente.FechaFinEjecucion = trabajo.FechaFinEjecucion;
            trabajoExistente.DescripcionAdministrador = trabajo.DescripcionAdministrador;
            trabajoExistente.Fotos = trabajo.Fotos;
            trabajoExistente.HayFirma = trabajo.HayFirma;
            trabajoExistente.Dnifirma = trabajo.Dnifirma;
            trabajoExistente.HayDaños = trabajo.HayDaños;
            trabajoExistente.Daños = trabajo.Daños;
            trabajoExistente.Urgencia = trabajo.Urgencia;

            _context.Trabajos.Update(trabajoExistente);
            await _context.SaveChangesAsync();

            await trans.CommitAsync();
            return new M_Resultado { resultado = Resultado.OK };
        }
        catch
        {
            await trans.RollbackAsync();
            return new M_Resultado { resultado = Resultado.KO };
        }
    }

    public async Task<Resultado> DeleteTrabajoAsync(int TrabajoPk)
    {
        try
        {
            var trabajo = await _context.Trabajos.FindAsync(TrabajoPk);
            if (trabajo != null)
            {
                trabajo.Activo = false;
                await _context.SaveChangesAsync();
            }
            return Resultado.OK;
        }
        catch
        {
            return Resultado.KO;
        }
    }

    private Resultado ValidacionesTrabajo(Trabajo trabajo, out List<string> listErrores)
    {
        listErrores = new List<string>();
        if(trabajo.SiniestroPk == null && trabajo.ObraPk == null)
            listErrores.Add("Debes seleccionar al menos un Siniestro o una Obra");
        if(trabajo.SiniestroPk != null && trabajo.ObraPk != null)
            listErrores.Add("Debes seleccionar tan solo un Siniestro o una Obra");
        if(trabajo.UasignadoPk == 0)
            listErrores.Add("Debes asignar el trabajo a un usuario");
        if (trabajo.UgestorPk == 0)
            listErrores.Add("Debes asignar el trabajo a un gestor");
        if (trabajo.FechaAsignada == DateTime.MinValue)
            listErrores.Add("Debes asignar una fecha de partida aproximada");
        if (trabajo.FechaFinAsignada == DateTime.MinValue)
            listErrores.Add("Debes asignar una fecha de finalización aproximada");
        if (trabajo.FechaFinAsignada <= trabajo.FechaAsignada)
            listErrores.Add("Debes asignar una fecha de finalización posterior a la da inicio");

        if (listErrores.Count() == 0)
            return Resultado.OK;
        else
            return Resultado.KO;
    }


    public async Task<byte[]> GenerarActaTrabajo(Trabajo trabajo)
    {
        string _basePath = "wwwroot/Archivos";
        try
        {
            // 1. Abrir plantilla
            using var ms = new MemoryStream();
            var plantilla = Path.Combine(_basePath, "Plantillas", "PlantillaActaTrabajo.pdf");
            var pdfReader = new PdfReader(plantilla);
            var pdfWriter = new PdfWriter(ms);
            var pdfDoc = new PdfDocument(pdfReader, pdfWriter);

            var form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            var fields = form.GetAllFormFields();

            // 2. Rellenar campos texto
            fields["txtSiniestro"].SetValue(trabajo.SiniestroPkNavigation.DetallesSiniestroPkNavigation.NumeroSiniestro).SetFontSize(12);
            fields["txtNombre"].SetValue(trabajo.NombreAseguradoParteGeneralli).SetFontSize(12);
            fields["txtDomicilio"].SetValue(trabajo.SiniestroPkNavigation.OrderDatos.FirstOrDefault().DireccionPkNavigation.NombreVia).SetFontSize(12);
            fields["txtResumen"].SetValue(trabajo.DescripcionParteGeneralli).SetFontSize(12);
            fields["txtLugar"].SetValue(trabajo.SiniestroPkNavigation.OrderDatos.FirstOrDefault().DireccionPkNavigation.Poblacion).SetFontSize(12);
            fields["txtDia"].SetValue(trabajo.FechaEjecucion?.Day.ToString()).SetFontSize(12);
            fields["txtMes"].SetValue(trabajo.FechaEjecucion?.Month.ToString()).SetFontSize(12);
            fields["txtAnho"].SetValue(trabajo.FechaEjecucion?.Year.ToString().Substring(2)).SetFontSize(12);
            fields["txtDNI"].SetValue(trabajo.Dnifirma).SetFontSize(12);

            // 3) Insertar la imagen en una posición concreta
            string rutaimagen = Path.Combine(_basePath, "Trabajo", trabajo.TrabajoPk.ToString(), "firma.png");
            if (System.IO.File.Exists(rutaimagen))
            {
                var page = pdfDoc.GetFirstPage();
                var img = new iText.Layout.Element.Image(
                                iText.IO.Image.ImageDataFactory.Create(rutaimagen))
                                .ScaleToFit(150, 80)
                                .SetFixedPosition(1, 250, 75);
                new iText.Layout.Document(pdfDoc).Add(img);
            }

            form.FlattenFields();        // Opcional: bloquea los campos
            pdfDoc.Close();

            // 1) Generamos el PDF en memoria (método anterior)
            byte[] pdfBytes = ms.ToArray();   // <-- usa el método del ejemplo anterior

            // 2) Carpeta destino   wwwroot/archivos/Trabajo/{Pk}
            string carpeta = Path.Combine(_basePath, "Trabajo", trabajo.TrabajoPk.ToString());
            Directory.CreateDirectory(carpeta);

            // 3) Ruta final: acta.pdf
            string rutaDestino = Path.Combine(carpeta, $"ConformidadCliente_{trabajo.SiniestroPkNavigation.DetallesSiniestroPkNavigation.NumeroSiniestro}.pdf");

            // 4) Guardamos / sobre-escribimos
            await System.IO.File.WriteAllBytesAsync(rutaDestino, pdfBytes);

            return pdfBytes;         // => lo devuelves como File(...) o lo guardas
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
