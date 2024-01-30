using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace eheh.Helpers
{
    public static class Cryptography
    {
        public static string GetMD5Hash(string input)
        {
            byte[] array = MD5.Create().ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array[i].ToString("x2", CultureInfo.CurrentCulture));
            }
            return stringBuilder.ToString();
        }

        public static string GetFileMD5Hash(string fileName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            MD5 mD = MD5.Create();
            using (FileStream inputStream = File.OpenRead(fileName))
            {
                byte[] array = mD.ComputeHash(inputStream);
                for (int i = 0; i < array.Length; i++)
                {
                    byte b = array[i];
                    stringBuilder.Append(b.ToString("x2").ToLower());
                }
            }
            return stringBuilder.ToString();
        }

        public static string GetFileMD5HashBase64(string fileName)
        {
            using (MD5 mD = MD5.Create())
            {
                return Convert.ToBase64String(mD.ComputeHash(File.ReadAllBytes(fileName)));
            }
        }

        public static bool VerifyMD5Hash(string chaine, string hash)
        {
            string mD5Hash = GetMD5Hash(chaine);
            return StringComparer.OrdinalIgnoreCase.Compare(mD5Hash, hash) == 0;
        }

        public static string EncryptRSA(string encryptValue, RSAParameters parameters)
        {
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
            rSACryptoServiceProvider.ImportParameters(parameters);
            byte[] bytes = Encoding.UTF8.GetBytes(encryptValue);
            return Convert.ToBase64String(rSACryptoServiceProvider.Encrypt(bytes, fOAEP: false));
        }

        public static string DecryptRSA(byte[] encryptedValue, RSAParameters parameters)
        {
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
            rSACryptoServiceProvider.ImportParameters(parameters);
            return Encoding.UTF8.GetString(rSACryptoServiceProvider.Decrypt(encryptedValue, fOAEP: false));
        }
    }
}
