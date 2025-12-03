using System.Security.Cryptography;

namespace PwCracker;

public class PasswordCracker
{

    public static readonly byte[] ENCRYPTION_KEY = [45, 103, 246, 79, 36, 99, 167, 3];

    public static string Encrypt(string password)
    {
        byte[] eNCRYPTION_KEY = ENCRYPTION_KEY;
        byte[] rgbIV = new byte[8] { 55, 103, 246, 79, 36, 99, 167, 3 };
        if (!string.IsNullOrEmpty(password))
        {
            DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(eNCRYPTION_KEY, rgbIV), CryptoStreamMode.Write);
            StreamWriter streamWriter = new StreamWriter(cryptoStream);
            streamWriter.Write(password);
            streamWriter.Flush();
            cryptoStream.FlushFinalBlock();
            memoryStream.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Length));
        }

        return string.Empty;
    }

    public static string Decrypt(string encryptedPassword)
    {
        byte[] eNCRYPTION_KEY = ENCRYPTION_KEY;
        byte[] rgbIV = new byte[8] { 55, 103, 246, 79, 36, 99, 167, 3 };
        if (!string.IsNullOrEmpty(encryptedPassword))
        {
            DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
            byte[] array = null;
            try
            {
                array = Convert.FromBase64String(encryptedPassword);
            }
            catch
            {
                return string.Empty;
            }

            return new StreamReader(new CryptoStream(new MemoryStream(array), dESCryptoServiceProvider.CreateDecryptor(eNCRYPTION_KEY, rgbIV), CryptoStreamMode.Read)).ReadToEnd();
        }

        return string.Empty;
    }
}
