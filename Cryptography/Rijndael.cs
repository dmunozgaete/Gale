using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Gale.Security.Cryptography
{
    public class Rijndael
    {
        private static byte[] defaultCypher = new byte[] { 94, 248, 50, 69, 230, 48, 109, 105, 52, 101, 53, 36, 97, 69, 70, 115, 78, 90, 78, 15, 248, 101, 53, 37 };

        /// <summary>
        /// Encripta dado el Cifrado Rijndael
        /// </summary>
        /// <param name="data">Información a Encriptar</param>
        /// <returns>Información Encriptada</returns>
        public static string Encrypt(string data)
        {
            return Encrypt(data, false);
        }

        /// <summary>
        /// Encripta dado el Cifrado Rijndael
        /// </summary>
        /// <param name="data">Información a Encriptar</param>
        /// <returns>Información Encriptada</returns>
        public static string Encrypt(string data, Boolean webSafe)
        {
            return Encrypt(data, System.Text.UTF8Encoding.UTF8.GetString(defaultCypher), webSafe);
        }

        /// <summary>
        /// Encripta dado el Cifrado Rijndael
        /// </summary>
        /// <param name="data">Información a Encriptar</param>
        /// <param name="cypher">Clave a Utilizar para cifrar</param>
        /// <returns>Información Encriptada</returns>
        public static string Encrypt(string data, string cypher, Boolean webSafe)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            byte[] plaintextByte = System.Text.Encoding.UTF8.GetBytes(data);
            byte[] saltByte = Encoding.ASCII.GetBytes(cypher.Length.ToString());

            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(cypher, saltByte);
            ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));

            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

            cryptoStream.Write(plaintextByte, 0, data.Length);
            cryptoStream.FlushFinalBlock();

            byte[] cipherBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();
            encryptor.Dispose();

            data = Convert.ToBase64String(cipherBytes);
            if (webSafe)
            {
                data = Gale.Serialization.ToBase64(data);
            }
            return data;
        }

        /// <summary>
        /// Desencripta dado el Cifrado Rijndael
        /// </summary>
        /// <param name="data">Información a Desencriptar</param>
        /// <returns>Información Desencriptada</returns>
        public static string Decrypt(string data)
        {
            return Decrypt(data, false);
        }

        public static string Decrypt(string data, Boolean webSafe)
        {
            return Decrypt(data, System.Text.UTF8Encoding.UTF8.GetString(defaultCypher), webSafe);
        }

        /// <summary>
        /// Desencripta dado el Cifrado Rijndael
        /// </summary>
        /// <param name="data">Información a Desencriptar</param>
        /// <param name="cypher">Clave a Utilizar para descifrar</param>
        /// <returns>Información Desencriptada</returns>
        public static string Decrypt(string data, string cypher, Boolean webSafe)
        {
            if (webSafe)
            {
                data = Gale.Serialization.FromBase64(data);
            }

            string ciphertext = data;
            try
            {
                RijndaelManaged rijndaelCipher = new RijndaelManaged();

                byte[] ciphertextByte = Convert.FromBase64String(ciphertext);
                byte[] saltByte = Encoding.ASCII.GetBytes(cypher.Length.ToString());

                PasswordDeriveBytes secretKey = new PasswordDeriveBytes(cypher, saltByte);
                ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
                System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(ciphertextByte);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                byte[] plainText = new byte[ciphertextByte.Length + 1];
                int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);

                memoryStream.Close();
                cryptoStream.Close();

                return Encoding.UTF8.GetString(plainText, 0, decryptedCount);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }
    }
}
