using Regal_Plataforma.Models.BDD;

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
    }

    public class VM_NotasUsuario
    {
        public List<NotasUsuario> listNotas { get; set; }
        public NotasUsuario Nota { get; set; } = new NotasUsuario();
        public int Usuario_PK { get; set; }
    }
}
