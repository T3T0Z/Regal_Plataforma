using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;

public interface ICalendarServices
{
    Task<VM_Month> GetMonthCalendarViewModel(int year, int month, int? UsuarioPk = null, int? ObraPk = null, int? SiniestroPk = null);
    Task<VM_Calendario> GetYearCalendarViewModel(int year, int? UsuarioPk = null, int? ObraPk = null, int? SiniestroPk = null);
}