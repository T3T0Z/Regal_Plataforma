﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Regal_Plataforma.Models.BDD;

public partial class NotasSiniestro
{
    public int NotasSiniestroPk { get; set; }

    public int SiniestroPk { get; set; }

    public int UsuarioPk { get; set; }

    public string Nota { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Siniestro SiniestroPkNavigation { get; set; }

    public virtual Usuario UsuarioPkNavigation { get; set; }
}