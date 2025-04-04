using Regal_Plataforma.Models;
using System.Security.Cryptography;
using System.Text;

namespace Regal_Plataforma.Funciones
{
    public class Func_Comunes
    {

        static string SemillaKey = "mQ2Nf7hAwc2CmjFBQ9ScCvlTEiMoU63wXAckg5sWoKSOMS4d4p";
        public static string EncryptClaveUsuario(string cadena)
        {
            byte[] keyArray;
            var Arreglo_a_Cifrar = Encoding.UTF8.GetBytes(cadena);
            var hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(SemillaKey));
            hashmd5.Clear();

            var tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            var cTransform = tdes.CreateEncryptor();
            var ArrayResultado = cTransform.TransformFinalBlock(Arreglo_a_Cifrar, 0, Arreglo_a_Cifrar.Length);
            tdes.Clear();
            var result = Convert.ToBase64String(ArrayResultado, 0, ArrayResultado.Length);
            return result.Replace('_', '-').Replace('+', '_').Replace('/', '$');

        }

        public static void LogsAplicacion(TiposLogs tipoLog, string usuario, string mensaje)
        {
            //Escribir log en una ruta superior a la de la aplicación.
            string rutaLogs = Directory.GetCurrentDirectory() + "/../Logs";
            string comentario = "";
            switch (tipoLog)
            {
                case TiposLogs.ERROR:
                case TiposLogs.ADVERTENCIA:
                case TiposLogs.ERROR_ACCESO_BD:
                    comentario = $"[{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}] - {usuario} - {tipoLog.ToString()} - {mensaje}";
                    break;
                case TiposLogs.INFO:
                    comentario = $"[{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}] - {usuario} - {mensaje}";
                    break;
                default:
                    //No debería entrar nunca porque tiene que ser un tipo definido
                    break;
            }

            try
            {
                if (!Directory.Exists(rutaLogs))
                {
                    Directory.CreateDirectory(rutaLogs);
                }
            }
            catch (Exception ex)
            {

                comentario = $"[{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}] - ERROR - No se ha podido crear el directorio de logs - {ex.Message}";
            }

            StreamWriter tw = new StreamWriter(rutaLogs + @"\" + DateTime.Now.ToString("yyyyMMdd") + ".txt", true);
            try
            {
                //Console.WriteLine(result);
                tw.WriteLine(comentario);
            }
            catch (Exception)
            {
                //Delay e intentar otra vez. Por si la app esta usando el archivo
                Thread.Sleep(500);
                try
                {
                    tw.WriteLine(comentario);
                }
                catch (Exception)
                {

                }
            }
            finally
            {
                tw.Close();
            }
        }
    }
}
