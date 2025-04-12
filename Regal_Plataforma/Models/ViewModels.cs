using Microsoft.Identity.Client;
using Regal_Plataforma.Models.BDD;
using System.Net;

namespace Regal_Plataforma.Models
{
    public class VM_Login
    {
        public string usuario { get; set; }
        public string contrasena { get; set; }
    }

    public class VM_GestionEncargos
    {
        public List<OrderDato> listOrderDatos { get; set; }
        public List<Gremio> listGremios { get; set; }
    }

    public class VM_VerDetallesEncargo
    {
        public OrderDato OrderDatos { get; set; }
        public List<Gremio> listGremios { get; set; }
        public VM_NotasEncargo Notas { get; set; } = new VM_NotasEncargo();
    }

    public class VM_NotasEncargo
    {
        public List<NotasSiniestro> listNotas { get; set; }
        public NotasSiniestro Nota { get; set; } = new NotasSiniestro();
        public int Encargo_PK { get; set; }
    }

    public class VM_GestionClientes
    {
        public List<Cliente> listClientes { get; set; }
    }

    public class VM_CreateUpdateClientes
    {
        public List<TiposCliente> listTiposClientes { get; set; }
        public Cliente Cliente { get; set; }
    }

    public class VM_NotasCliente
    {
        public List<NotasCliente> listNotas { get; set; }
        public NotasCliente Nota { get; set; } = new NotasCliente();
        public int Cliente_PK { get; set; }
    }   

    public class VM_GestionObras
    {
        public List<Obra> listObras { get; set; }
        public List<EstadoObra> listEstadosObras { get; set; }
    }

    public class VM_CreateUpdateObra
    {
        public Obra Obra { get; set; }
        public List<Cliente> listClientes { get; set; }
        public List<Usuario> listaGestores { get; set; }
        public List<EstadoObra> listEstadosObras { get; set; }
    }

    public class VM_NotasObra
    {
        public List<NotasObra> listNotas { get; set; }
        public NotasObra Nota { get; set; } = new NotasObra();
        public int Obra_PK { get; set; }
    }

    public class VM_GestionUsuarios
    {
        public List<Usuario> listUsuarios { get; set; } = new();
        public List<Role> listRoles { get; set; } = new();
    }
    public class VM_CreateUpdateUsuario
    {
        public Usuario Usuario { get; set; } = new();
        public List<Role> listRoles { get; set; } = new();
    }

    public class VM_DetallesUsuario
    {
        public Usuario Usuario { get; set; }
        public VM_NotasUsuario Notas { get; set; } = new VM_NotasUsuario();
        public VM_CalendarioUsuario Calendario { get; set; } = new VM_CalendarioUsuario();
    }

    public class VM_NotasUsuario
    {
        public List<NotasUsuario> listNotas { get; set; }
        public NotasUsuario Nota { get; set; } = new NotasUsuario();
        public int Usuario_PK { get; set; }
    }

    public class VM_listaTrabajos
    {
        public List<Trabajo> listTrabajos { get; set; }
        public string Dia { get; set; }
    }

    public class VM_CreateEditTrabajo
    {
        public Trabajo Trabajo { get; set; }
        public List<Usuario> listUsuariosGestores { get; set; } = new List<Usuario>();
        public List<Usuario> listUsuariosAsignados { get; set; } = new List<Usuario>();
        public List<Siniestro> listSiniestros { get; set; } = new List<Siniestro>();
        public List<Obra> listObras { get; set; } = new List<Obra>();
        public List<EstadosTrabajo> listEstadosTrabajo { get; set; } = new List<EstadosTrabajo>();
        VM_NotasTrabajo Notas { get; set; } = new VM_NotasTrabajo();
    }

    public class VM_NotasTrabajo
    {
        public List<NotasTrabajo> listNotas { get; set; }
        public NotasTrabajo Nota { get; set; } = new NotasTrabajo();
        public int Trabajo_PK { get; set; }
    }

    public class VM_CalendarioUsuario
    {
        public int Year { get; set; }
        public int UsuarioPk { get; set; }
        public List<VM_Month> Calendars { get; set; }
        // Nueva propiedad para introducir los días festivos.
        public List<DateTime> Holidays { get; set; }

        public VM_CalendarioUsuario()
        {
            Calendars = new List<VM_Month>();
        }
    }

    public class VM_Month
    {
        public int UsuarioPk { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public List<VM_Week> Weeks { get; set; }
        // Nueva propiedad para introducir los días festivos.
        public List<DateTime> Holidays { get; set; }
        // Indica si el día es el actual
        public bool actualMonth { get; set; }

        public VM_Month()
        {
            Weeks = new List<VM_Week>();
        }
    }

    public class VM_Week
    {
        // Lista de días que conforman una semana.
        public List<VM_Day> Days { get; set; }

        public VM_Week()
        {
            Days = new List<VM_Day>();
        }
    }

    public class VM_Day
    {
        // Fecha del día.
        public DateTime Date { get; set; }
        // Indica si el día pertenece al mes actual (útil para el calendario).
        public bool IsCurrentMonth { get; set; }
        // Indica si el día es el actual
        public bool actualDay { get; set; }
        // Lista de trabajos asignados a este día.
        public VM_listaTrabajos Trabajos { get; set; }

        public VM_Day()
        {
            Trabajos = new VM_listaTrabajos();
        }
    }
    public class VM_GaleriaArchivos
    {
        public string Entidad { get; set; }            // Ej: "Obra", "Cliente", "Siniestro"
        public int EntidadPk { get; set; }             // Primary Key de la entidad

        public List<ArchivoDto> ListaArchivos { get; set; } = new List<ArchivoDto>();
    }

    public class ArchivoDto
    {
        public string NombreArchivo { get; set; }      // Nombre del archivo (para mostrar)
        public string Ruta { get; set; }               // Ruta relativa del archivo (para acceder al archivo)
        public string Extension { get; set; }
    }
    public class ArchivoViewModel
    {
        public string Nombre { get; set; }
        public string Entidad { get; set; }
        public int Id { get; set; }
    }
}
