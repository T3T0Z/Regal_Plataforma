using Regal_Plataforma.Models;
using Regal_Plataforma.Models.BDD;

public class CalendarServices : ICalendarServices
{
    private readonly REGAL_ASISTENCIAContext _dbContext;

    public CalendarServices(REGAL_ASISTENCIAContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<VM_Month> GetMonthCalendarViewModel(int year, int month, int? UsuarioPk = null, int? ObraPk = null, int? SiniestroPk = null)
    {
        var calendar = new VM_Month { Month = month, Year = year, actualMonth = month == DateTime.Now.Month ? true : false };
        DateTime firstDayOfMonth = new DateTime(year, month, 1);

        // Ajuste: calcular el inicio del calendario como el lunes anterior al primer día del mes.
        int diff = (firstDayOfMonth.DayOfWeek == DayOfWeek.Sunday) ? 6 : ((int)firstDayOfMonth.DayOfWeek - 1);
        DateTime calendarStartDate = firstDayOfMonth.AddDays(-diff);

        // Se visualizan 6 semanas (42 días) en el calendario
        int totalDays = 42;
        VM_Week currentWeek = null;

        for (int i = 0; i < totalDays; i++)
        {
            DateTime currentDate = calendarStartDate.AddDays(i);
            bool isCurrentMonth = currentDate.Month == month;

            // Ejemplo: asignar un trabajo dummy para cada tercer día del mes
            List<Trabajo> listTrabajos = new List<Trabajo>();
            if (UsuarioPk != null && UsuarioPk != 0)
            {
                listTrabajos = _dbContext.Trabajos.Where(x => x.UasignadoPk == UsuarioPk && x.FechaAsignada.Date == currentDate.Date && x.Activo).ToList();
                calendar.UsuarioPk = UsuarioPk;
            }
            else if (ObraPk != null && ObraPk != 0)
            {
                listTrabajos = _dbContext.Trabajos.Where(x => x.ObraPk == ObraPk && x.FechaAsignada.Date == currentDate.Date && x.Activo).ToList();
                calendar.ObraPk = ObraPk;
            }
            else if (SiniestroPk != null && SiniestroPk != 0)
            {
                listTrabajos = _dbContext.Trabajos.Where(x => x.SiniestroPk == SiniestroPk && x.FechaAsignada.Date == currentDate.Date && x.Activo).ToList();
                calendar.SiniestroPk = SiniestroPk;
            }

            VM_listaTrabajos trabajos = new VM_listaTrabajos
            {
                listTrabajos = listTrabajos
            };

            var day = new VM_Day
            {
                Date = currentDate,
                IsCurrentMonth = isCurrentMonth,
                Trabajos = trabajos,
                HorasTotales = (listTrabajos.Sum(t => decimal.TryParse(t.TiempoTransacurridoTrabajo, out var m) ? m : 0m)/ 60m).ToString("0.##")
            };

            if (currentDate.Day == DateTime.Now.Day && currentDate.Month == DateTime.Now.Month && currentDate.Year == DateTime.Now.Year)
                day.actualDay = true;

            // Iniciar una nueva semana cada lunes (lunes es el primer día de la semana)
            if (currentDate.DayOfWeek == DayOfWeek.Monday)
            {
                currentWeek = new VM_Week();
                calendar.Weeks.Add(currentWeek);
            }

            currentWeek.Days.Add(day);
        }

        calendar.Holidays = vacaciones;
        calendar.HorasTotales = calendar.Weeks.SelectMany(x => x.Days.Where(y => y.Date.Month == month && y.Date.Year == year)).Sum(x => Convert.ToDecimal(x.HorasTotales)).ToString("0.##");

        return calendar;
    }

    public async Task<VM_Calendario> GetYearCalendarViewModel(int year, int? UsuarioPk = null, int? ObraPk = null, int? SiniestroPk = null)
    {
        var yearCalendar = new VM_Calendario { Year = year, Holidays = vacaciones };
        for (int month = 1; month <= 12; month++)
        {
            var monthlyCalendar = GetMonthCalendarViewModel(year, month, UsuarioPk, ObraPk, SiniestroPk);
            yearCalendar.Calendars.Add(await monthlyCalendar);
        }

        yearCalendar.UsuarioPk = UsuarioPk;
        yearCalendar.ObraPk = ObraPk;
        yearCalendar.SiniestroPk = SiniestroPk;

        return yearCalendar;
    }

    public List<DateTime> vacaciones = new List<DateTime>
    {
        new DateTime(2025, 1, 1),    // Año Nuevo
        new DateTime(2025, 1, 6),    // Reyes Magos

        new DateTime(2025, 3, 3),   // Construccion
        new DateTime(2025, 3, 4),   // Vigo
        new DateTime(2025, 3, 5),   // Construccion
        new DateTime(2025, 3, 28),   // Vigo

        new DateTime(2025, 4, 14),   // Construccion
        new DateTime(2025, 4, 15),   // Construccion
        new DateTime(2025, 4, 16),   // Construccion
        new DateTime(2025, 4, 17),   // Jueves Santo
        new DateTime(2025, 4, 18),   // Viernes Santo

        new DateTime(2025, 5, 1),    // Día del Trabajo
        new DateTime(2025, 5, 2),    // Construccion
        new DateTime(2025, 5, 17),    // Letras Gallegas

        new DateTime(2025, 7, 24),    // Construccion
        new DateTime(2025, 7, 25),    // Día del Trabajo

        new DateTime(2025, 8, 14),   // Construccion
        new DateTime(2025, 8, 15),   // Asunción de la Virgen

        new DateTime(2025, 10, 31),  // Construccion

        new DateTime(2025, 11, 1),   // Todos los Santos

        new DateTime(2025, 12, 6),   // Día de la Constitución
        new DateTime(2025, 12, 8),   // Inmaculada Concepción
        new DateTime(2025, 12, 24),   // Construccion
        new DateTime(2025, 12, 25),   // Navidad
        new DateTime(2025, 12, 26),   // Construccion
        new DateTime(2025, 12, 31)   // Construccion
    };
}
