using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

public class CryptoHelper
{
    // Encrypt text using AES
    public static string EncryptAES(string plainText, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = keyBytes;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            // Generate a random IV
            aesAlg.GenerateIV();
            byte[] ivBytes = aesAlg.IV;

            using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = PerformCryptography(plainBytes, encryptor);

                // Prepend IV to the encrypted data
                byte[] result = new byte[ivBytes.Length + encryptedBytes.Length];
                Array.Copy(ivBytes, 0, result, 0, ivBytes.Length);
                Array.Copy(encryptedBytes, 0, result, ivBytes.Length, encryptedBytes.Length);

                // Convert to Base64 string and return
                return Convert.ToBase64String(result);
            }
        }
    }

    // Decrypt text using AES
    public static string DecryptAES(string cipherText, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        // Extract the IV (first 16 bytes) from the ciphertext
        byte[] ivBytes = new byte[16];
        Array.Copy(cipherBytes, 0, ivBytes, 0, ivBytes.Length);

        // Extract the actual ciphertext (remaining bytes after the IV)
        byte[] actualCipherBytes = new byte[cipherBytes.Length - ivBytes.Length];
        Array.Copy(cipherBytes, ivBytes.Length, actualCipherBytes, 0, actualCipherBytes.Length);

        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = keyBytes;
            aesAlg.IV = ivBytes;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
            {
                byte[] decryptedBytes = PerformCryptography(actualCipherBytes, decryptor);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }

    // Helper method to perform encryption or decryption
    private static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
    {
        using (var memoryStream = new System.IO.MemoryStream())
        using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
        {
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            return memoryStream.ToArray();
        }
    }

    // Example Usage 
    //Encryption//

    //string secretKey = "1234567890123456"; // Ensure it's 16 bytes
    //string plainText = "Hello World";

    //string encryptedText = CryptoHelper.EncryptAES(plainText, secretKey);
    //Console.WriteLine($"Encrypted: {encryptedText}");

    //Decryption//
    //string decryptedText = CryptoHelper.DecryptAES(encryptedText, secretKey);
    //Console.WriteLine($"Decrypted: {decryptedText}");


//    Ensure Key Length:
//AES keys must be one of the following lengths:

//16 bytes(128 bits)
//24 bytes(192 bits)
//32 bytes(256 bits)
//If the secret key is shorter, pad it(e.g., with zeros or spaces) to meet the required length.
}
