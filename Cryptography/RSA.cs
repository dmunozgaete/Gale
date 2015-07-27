using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Gale.Security.Cryptography
{
    public class RSA
    {
        /// <summary>
        /// Encripta dado el Cifrado RSA
        /// </summary>
        /// <param name="data">Información a Encriptar</param>
        /// <param name="cypher">Clave a Utilizar para cifrar</param>
        /// <returns>Información Encriptada</returns>
        public static string Encrypt(string data, string xmlCypher)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlCypher);

            byte[] cipherBytes = rsa.Encrypt(Encoding.Default.GetBytes(data), false);
            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// Desencripta dado el Cifrado Rijndael
        /// </summary>
        /// <param name="data">Información a Desencriptar</param>
        /// <param name="cypher">Clave a Utilizar para descifrar</param>
        /// <returns>Información Desencriptada</returns>
        public static string Decrypt(string data, string xmlCypher)
        {
            //string ciphertext = data;
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(xmlCypher);
                byte[] datos = rsa.Decrypt(Convert.FromBase64String(data), false);
                return Encoding.Default.GetString(datos);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }
    }
}
