﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Regal_Plataforma.Models.BDD;

public partial class DetallesSiniestro
{
    public int DetallesSiniestroPk { get; set; }

    public string NumeroSiniestro { get; set; }

    public string NumeroPoliza { get; set; }

    public string Ramo { get; set; }

    public string NombreRamo { get; set; }

    public DateTime? FechaEfecto { get; set; }

    public DateTime? FechaApendice { get; set; }

    public string Condiciones { get; set; }

    public DateTime? FechaAperturaSiniestro { get; set; }

    public DateTime? FechaOcurrencia { get; set; }

    public string Causa { get; set; }

    public string Descripcion { get; set; }

    public bool? TienePoliza { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<Siniestro> Siniestros { get; set; } = new List<Siniestro>();
}