using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Belcorp.Encore.Application.Utilities
{
    public class EncryptionUtilities
    {
        /// <summary>
		/// Deciphers a base 64 encoded string.
		/// </summary>
		/// <param name="key">cipher key</param>
		/// <param name="base64Text">the base 64 encoded text to decipher.</param>
		/// <param name="salt">salt</param>
		/// <returns>The decrypted text, or empty string if failure</returns>
		public static string DecryptTripleDES(byte[] key, string base64Text, string salt)
        {
            if ((key != null) && (key.Length > 0) && (!String.IsNullOrWhiteSpace(base64Text)))
            {
                return DecryptTripleDES<string>(key, base64Text, salt);
            }

            return "";
        }

        /// <summary>
        /// Decryptns a base 64 encrypted string
        /// </summary>
        /// <typeparam name="T">The type of object to be returned</typeparam>
        /// <param name="key">The encryption key</param>
        /// <param name="base64Text">The base 64 encoded text to be decrypted</param>
        /// <param name="salt">The salt to be used</param>
        /// <returns>The decrypted text, or default(T) if failure</returns>
        public static T DecryptTripleDES<T>(byte[] key, string base64Text, string salt) where T : class
        {
            try
            {
                byte[] textBytes = Convert.FromBase64String(base64Text);
                string decryptedString = System.Text.ASCIIEncoding.ASCII.GetString(Decrypt(textBytes, key));

                if (salt != null && salt.Length > 0 && decryptedString.EndsWith(salt))
                    decryptedString = decryptedString.Substring(0, decryptedString.Length - salt.Length);

                if (typeof(T) == typeof(string))
                {
                    return decryptedString as T;
                }
                else
                {
                    return JsonConvert.DeserializeObject<T>(decryptedString);
                }
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Decrypts bytesToDecrypt with the provided key
        /// </summary>
        /// <param name="bytesToDecrypt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] bytesToDecrypt, byte[] key)
        {
            try
            {
                using (TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider())
                using (MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider())
                {
                    DES.Key = hashMD5.ComputeHash(key);
                    DES.Mode = System.Security.Cryptography.CipherMode.ECB;
                    System.Security.Cryptography.ICryptoTransform DESDecrypt = DES.CreateDecryptor();

                    return DESDecrypt.TransformFinalBlock(bytesToDecrypt, 0, bytesToDecrypt.Length);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Decryption failed. Original Message: {0}", ex.Message), ex);
            }
        }
    }
}
