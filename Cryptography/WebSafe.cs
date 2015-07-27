using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Gale.Security.Cryptography
{
    /// <summary>
    /// Encripta y optimiza el resultado para el envio de variable en QueryString
    /// </summary>
    public class WebSafe
    {
        /// <summary>
        /// Encripta dado el Cifrado Rijndael
        /// </summary>
        /// <param name="data">Información a Encriptar</param>
        /// <returns>Información Encriptada</returns>
        public static string Encrypt(string data)
        {
            return Gale.Security.Cryptography.Rijndael.Encrypt(data, true);
        }

        /// <summary>
        /// Desencripta dado el Cifrado Rijndael
        /// </summary>
        /// <param name="data">Información a Desencriptar</param>
        /// <returns>Información Desencriptada</returns>
        public static string Decrypt(string data)
        {
            return Gale.Security.Cryptography.Rijndael.Decrypt(data, true);
        }
       
    }
}
