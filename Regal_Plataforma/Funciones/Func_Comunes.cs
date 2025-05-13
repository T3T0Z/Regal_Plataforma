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

            // Use MD5.Create() instead of MD5CryptoServiceProvider  
            using (var hashmd5 = MD5.Create())
            {
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(SemillaKey));
            }

            // Replace TripleDESCryptoServiceProvider with TripleDES.Create()
            using (var tdes = TripleDES.Create())
            {
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                var cTransform = tdes.CreateEncryptor();
                var ArrayResultado = cTransform.TransformFinalBlock(Arreglo_a_Cifrar, 0, Arreglo_a_Cifrar.Length);
                var result = Convert.ToBase64String(ArrayResultado, 0, ArrayResultado.Length);
                return result.Replace('_', '-').Replace('+', '_').Replace('/', '$');
            }
        }
    }
}
