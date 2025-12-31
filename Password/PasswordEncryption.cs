using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BudgetLedger.Password
{
    public static class PasswordEncryption
    {
        // Encryption key and IV (Initialization Vector)
        // Note: In a real application, store these securely (e.g., in Windows Data Protection API or a secure key vault).
        // For simplicity, we're hardcoding them here, but this is NOT recommended for production apps.
        private static readonly byte[] Key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
        private static readonly byte[] IV = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

        // Encrypt a string
        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create a memory stream
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // Create a crypto stream and write the encrypted data
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    // Return the encrypted bytes as a base64 string
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // Decrypt a string
        public static string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create a memory stream with the cipher text
                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    // Read the decrypted bytes and return as a string
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}
