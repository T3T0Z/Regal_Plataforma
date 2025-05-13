// AppLogger.cs  (puedes ponerlo donde prefieras)
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Regal_Plataforma.Funciones
{
    public enum LogType
    {
        Info,
        Advertencia,
        Error,
        ErrorAccesoBd
    }

    public static class AppLogger
    {
        private static readonly SemaphoreSlim _sem = new(1, 1);     // para concurrencia
        private static readonly string _logDir =
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "Logs"));

        /// <summary>
        /// Escribe una línea de log en formato:
        /// [2025-05-05 13:42:00] - user - Tipo - Mensaje
        /// Para Info no se muestra el tipo.
        /// </summary>
        public static async Task WriteAsync(LogType tipo, string usuario, string mensaje)
        {
            var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var txt = tipo == LogType.Info
                      ? $"[{ts}] - {usuario} - {mensaje}"
                      : $"[{ts}] - {usuario} - {tipo} - {mensaje}";

            var file = Path.Combine(_logDir, $"{DateTime.Today:yyyyMMdd}.log");

            Directory.CreateDirectory(_logDir);          // idempotente

            await _sem.WaitAsync();                      // sección crítica
            try
            {
                await File.AppendAllTextAsync(file,
                        txt + Environment.NewLine,
                        Encoding.UTF8);
            }
            catch (Exception ex)
            {
                // último recurso: consola de error
                Console.Error.WriteLine($"No se pudo escribir log: {ex}");
            }
            finally
            {
                _sem.Release();
            }
        }
    }
}
