
// Servicio
using Microsoft.EntityFrameworkCore;
using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;
using System;
using System.Security.Claims;

public class OperarioService : IOperarioService
{
    private readonly REGAL_ASISTENCIAContext _context;

    public OperarioService(REGAL_ASISTENCIAContext context)
    {
        _context = context;
    }

    public async Task<Trabajo> ObtenerTrabajoAsignadoAsync(int usuarioPk)
    {
        var ahora = DateTime.Now;
        var mediaHoraDespues = ahora.AddMinutes(30);
        
        var listTrabajos = _context.Trabajos
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.DetallesSiniestroPkNavigation)
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.OrderDatos)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.SiniestroPkNavigation)
            .ThenInclude(x => x.OrderDatos)
            .ThenInclude(x => x.PaseguradoPkNavigation)
            .Include(x => x.ObraPkNavigation)
            .ThenInclude(x => x.DireccionPkNavigation)
            .Include(x => x.UgestorPkNavigation)
            .Include(x => x.UasignadoPkNavigation)
            .Include(x => x.EstadoTrabajoPkNavigation)
            .Include(x => x.NotasTrabajos)
            .ThenInclude(x => x.UsuarioPkNavigation)
            .Include(x=>x.DescansosTrabajos)
            .Include(x=>x.UbicacionesTrabajos)
            .Where(t => t.UasignadoPk == usuarioPk &&
                        t.FechaAsignada.Date == DateTime.Now.Date &&
                        t.Activo)
            .OrderBy(x=>x.FechaAsignada)
            .ToList();

        // listTrabajos == LISTA DE TRABAJOS DE EL DIA ACTUAL Y EL USUARIO SOLICITADO
        // SI HAY UN TRABAJO EN CURSO SIEMPRE ESCOGE EL TRABAJO EN CURSO
        // SI NO, COGE EL TRAAJO QUE ESTE EN LA SIGUIENTE MEDIA HORA DESDE LA HORA DE SOLICITUD DEL TRABAJO
        if(listTrabajos.Any(t => t.EstadoTrabajoPk == 2))
        {
            return listTrabajos.Where(t => t.EstadoTrabajoPk == 2).FirstOrDefault();
        }

        return listTrabajos.Where(t =>
                    t.FechaAsignada <= mediaHoraDespues &&
                    (t.EstadoTrabajoPk == 1 || t.EstadoTrabajoPk == 2))
        .FirstOrDefault();
    }

    public async Task<M_Resultado> UpdateTrabajoAsync(Trabajo trabajo)
    {
        List<string> listErrores = new List<string>();
        if (ValidacionesTrabajo(trabajo, out listErrores) == Resultado.KO)
            return new M_Resultado { resultado = Resultado.KO, mensaje = string.Join("\n", listErrores) };

        using var trans = await _context.Database.BeginTransactionAsync();
        try
        {
            var trabajoExistente = await _context.Trabajos.Where(x=>x.TrabajoPk == trabajo.TrabajoPk).Include(x => x.DescansosTrabajos).FirstOrDefaultAsync();
            if (trabajoExistente == null) return 
                    new M_Resultado { resultado = Resultado.KO };
            if(trabajoExistente.SiniestroPk != null)
            {
                if (trabajoExistente.HayFirma == false)
                    return new M_Resultado { resultado = Resultado.KO, mensaje = "La firma es obligatoria" };
            }

            trabajoExistente.EstadoTrabajoPk = trabajo.EstadoTrabajoPk;
            trabajoExistente.FechaAsignada = trabajo.FechaAsignada;
            trabajoExistente.FechaFinAsignada = trabajo.FechaFinAsignada;
            trabajoExistente.FechaEjecucion = trabajo.FechaEjecucion;
            trabajoExistente.FechaFinEjecucion = trabajo.FechaFinEjecucion;
            trabajoExistente.NombreAseguradoParteGeneralli = trabajo.NombreAseguradoParteGeneralli;
            trabajoExistente.DescripcionParteGeneralli = trabajo.DescripcionParteGeneralli;
            trabajoExistente.DescripcionTrabajo = trabajo.DescripcionTrabajo;
            trabajoExistente.Daños = trabajo.Daños;
            trabajoExistente.Fotos = trabajo.Fotos;
            trabajoExistente.Dnifirma = trabajo.Dnifirma;
            trabajoExistente.HayDaños = trabajo.HayDaños;


            if (trabajo.EstadoTrabajoPk == 3 || trabajo.EstadoTrabajoPk == 4)
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

    public async Task<Trabajo> CreateTrabajoLibreAsync(Trabajo trabajo)
    {
        using var trans = await _context.Database.BeginTransactionAsync();
        try
        {
            var trabajoLibre = await _context.Trabajos.Where(x => x.FechaAsignada.Date == DateTime.Now.Date && x.EstadoTrabajoPk == 4 && x.FechaFinEjecucion == null).FirstOrDefaultAsync();
            if (trabajoLibre != null)
            {
                return trabajoLibre;
            }

            _context.Trabajos.Add(trabajo);
            await _context.SaveChangesAsync();

            await trans.CommitAsync();
            return trabajo;
        }
        catch
        {
            await trans.RollbackAsync();
            return null;
        }
    }


    public async Task<M_Resultado> GuardarUbicacionTrabajoAsync(UbicacionesTrabajo ubicacion)
    {
        using var trans = await _context.Database.BeginTransactionAsync();
        try
        {
            UbicacionesTrabajo newUbicacion = new UbicacionesTrabajo
            {
                Latitude = ubicacion.Latitude,
                Longitude = ubicacion.Longitude,
                Accuracy = ubicacion.Accuracy,
                FechaCreacion = DateTime.Now,
                TrabajoPk = ubicacion.TrabajoPk
            };

            _context.UbicacionesTrabajos.Add(newUbicacion);
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

    public async Task<DescansosTrabajo> GuardarDescansoTrabajoAsync(DescansosTrabajo descansoTrabajo)
    {
        List<string> listErrores = new List<string>();

        using var trans = await _context.Database.BeginTransactionAsync();

        DescansosTrabajo descansoNew = new DescansosTrabajo();
        try
        {
            if(_context.DescansosTrabajos.Any(x=> x.TrabajoPk == descansoTrabajo.TrabajoPk && x.FechaFin == null && x.FechaInicio.Day == DateTime.Now.Day))
            {
                return await _context.DescansosTrabajos.Where(x => x.TrabajoPk == descansoTrabajo.TrabajoPk && x.FechaFin == null && x.FechaInicio.Day == DateTime.Now.Day).FirstOrDefaultAsync();
            }
            if(descansoTrabajo.DescansosTrabajosPk != 0)
            {
                descansoNew = await _context.DescansosTrabajos.FindAsync(descansoTrabajo.DescansosTrabajosPk);
                if (descansoNew == null) return null;

                descansoNew.FechaFin = DateTime.Now;
                descansoNew.Descripcion = descansoTrabajo.Descripcion;

                _context.DescansosTrabajos.Update(descansoNew);
            }
            else
            {
                descansoNew = new DescansosTrabajo
                {
                    TrabajoPk = descansoTrabajo.TrabajoPk,
                    FechaInicio = DateTime.Now
                };

                _context.DescansosTrabajos.Add(descansoNew);
            }

            await _context.SaveChangesAsync();

            await trans.CommitAsync();
            return descansoNew;
        }
        catch
        {
            await trans.RollbackAsync();
            return null;
        }
    }

    public async Task<DescansosTrabajo> ObtenerDescansoTrabajoAsync(int descansoTrabajoPk)
    {
        return await _context.DescansosTrabajos.Where(x => x.DescansosTrabajosPk == descansoTrabajoPk).FirstOrDefaultAsync();
    }

    private Resultado ValidacionesTrabajo(Trabajo trabajo, out List<string> listErrores)
    {
        listErrores = new List<string>();
        if (string.IsNullOrEmpty(trabajo.DescripcionTrabajo))
            listErrores.Add("La descripción del trabajo es obligatoria");
        if(trabajo.SiniestroPk != null)
        {
            if (string.IsNullOrEmpty(trabajo.DescripcionParteGeneralli))
                listErrores.Add("La descripción del parte de Generalli es obligatoria");
            if (string.IsNullOrEmpty(trabajo.Dnifirma))
                listErrores.Add("El DNI del parte de Generalli es obligatorio");
            if (string.IsNullOrEmpty(trabajo.NombreAseguradoParteGeneralli))
                listErrores.Add("El nombre del asegurado del parte de Generalli es obligatorio");
            if (trabajo.HayDaños)
            {
                if (string.IsNullOrEmpty(trabajo.Daños))
                    listErrores.Add("Los daños son obligatorios");
            }
        }

        if (listErrores.Count() == 0)
            return Resultado.OK;
        else
            return Resultado.KO;
    }
}
