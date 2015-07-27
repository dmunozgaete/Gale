using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Gale.Security.Cryptography
{
    public class MD5
    {
        /// <summary>
        /// Genera el HASH en base64 de acuerdo a la data
        /// </summary>
        /// <param name="data">Información Sensible</param>
        /// <returns>Hash generado en base a la data</returns>
        public static string GenerateHash(string data)
        {
            //Encrypt the password and generate the HASH
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] hash = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(data));

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();

          
        }

        /// <summary>
        /// Genera el HASH en base64 de acuerdo a la data
        /// </summary>
        /// <param name="data">Información Sensible</param>
        /// <returns>Hash generado en base a la data</returns>
        public static string GenerateHash(byte[] data)
        {
            //Encrypt the password and generate the HASH
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] hash = md5.ComputeHash(data);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }


        /// <summary>
        /// Genera el HASH en base64 de acuerdo a la data
        /// </summary>
        /// <param name="data">Información Sensible</param>
        /// <returns>Hash generado en base a la data</returns>
        public static string GenerateHash(System.IO.Stream data)
        {
            //Encrypt the password and generate the HASH
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] hash = md5.ComputeHash(data);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
