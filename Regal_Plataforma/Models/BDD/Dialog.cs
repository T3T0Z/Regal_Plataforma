﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Regal_Plataforma.Models.BDD;

public partial class Dialog
{
    public int DialogPk { get; set; }

    public int OrderPk { get; set; }

    public string Company { get; set; }

    public string IdDialog { get; set; }

    public string IdOrder { get; set; }

    public string IdParentDialog { get; set; }

    public string Transmitter { get; set; }

    public string Receiver { get; set; }

    public string Issue { get; set; }

    public string Message { get; set; }

    public string HasDocumentation { get; set; }

    public string AnswerRequired { get; set; }

    public string IdProfessional { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Order OrderPkNavigation { get; set; }
}