﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Regal_Plataforma.Models.BDD;

public partial class NotasTrabajo
{
    public int NotasTrabajoPk { get; set; }

    public int TrabajoPk { get; set; }

    public int UsuarioPk { get; set; }

    public string Nota { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Trabajo TrabajoPkNavigation { get; set; }

    public virtual Usuario UsuarioPkNavigation { get; set; }
}