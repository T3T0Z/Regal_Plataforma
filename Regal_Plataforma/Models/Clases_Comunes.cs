namespace Regal_Plataforma.Models
{
    public enum TiposLogs
    { 
        ERROR,
        ERROR_ACCESO_BD,
        ADVERTENCIA,
        INFO
    }

    public enum Resultado
    {
        OK,
        KO
    }

    public class M_Resultado
    {
        public Resultado resultado { get; set; }
        public string mensaje {get; set;}
    }
}
