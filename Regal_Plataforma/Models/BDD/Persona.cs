﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Regal_Plataforma.Models.BDD;

public partial class Persona
{
    public int PersonaPk { get; set; }

    public string TipoPersona { get; set; }

    public string CarneIdentidad { get; set; }

    public string Nombre { get; set; }

    public string Apellido1 { get; set; }

    public string Apellido2 { get; set; }

    public string NombreCompany { get; set; }

    public string Email { get; set; }

    public string Nacionalidad { get; set; }

    public int? NumeroTelefonoPk { get; set; }

    public int? NumeroTelefono2Pk { get; set; }

    public int? DireccionPk { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Direccion DireccionPkNavigation { get; set; }

    public virtual Telefono NumeroTelefono2PkNavigation { get; set; }

    public virtual Telefono NumeroTelefonoPkNavigation { get; set; }

    public virtual ICollection<OrderDato> OrderDatoPafectadaPkNavigations { get; set; } = new List<OrderDato>();

    public virtual ICollection<OrderDato> OrderDatoPagentePolizaPkNavigations { get; set; } = new List<OrderDato>();

    public virtual ICollection<OrderDato> OrderDatoPaseguradoPkNavigations { get; set; } = new List<OrderDato>();

    public virtual ICollection<OrderDato> OrderDatoPcontactoPkNavigations { get; set; } = new List<OrderDato>();

    public virtual ICollection<OrderDato> OrderDatoPinterlocutorPkNavigations { get; set; } = new List<OrderDato>();

    public virtual ICollection<OrderDato> OrderDatoPprofesionalPkNavigations { get; set; } = new List<OrderDato>();

    public virtual ICollection<OrderDato> OrderDatoPtomadorPkNavigations { get; set; } = new List<OrderDato>();

    public virtual ICollection<OrderDato> OrderDatoPtramitadorPkNavigations { get; set; } = new List<OrderDato>();
}