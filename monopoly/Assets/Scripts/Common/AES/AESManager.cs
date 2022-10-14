using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
public class AESManager : MonoBehaviour
{
    //加解密密钥,可自行替换
    protected static string tmpDESkey = "fuahgklanvariuabhkl";
    // Create sha256 hash
    static SHA256 mySHA256 = SHA256Managed.Create();
    protected static byte[] basekey = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(tmpDESkey));
    // Create secret IV
    protected static byte[] baseiv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
    #region DESEnCode DES加密   
    //====================================================================
    /// <summary>
    ///  DES加密
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public static string DESEnCode(string plainText)
    {
        byte[] key = basekey;
        byte[] iv = baseiv;

        Aes encryptor = Aes.Create();

        encryptor.Mode = CipherMode.CBC;

        // Set key and IV
        byte[] aesKey = new byte[32];
        Array.Copy(key, 0, aesKey, 0, 32);
        encryptor.Key = aesKey;
        encryptor.IV = iv;

        MemoryStream memoryStream = new MemoryStream();
        ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();
        CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);

        byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);
        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
        cryptoStream.FlushFinalBlock();
        byte[] cipherBytes = memoryStream.ToArray();

        memoryStream.Close();
        cryptoStream.Close();

        string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);

        return cipherText;

    }
    #endregion

    #region DESDeCode DES解密
    //====================================================================
    /// <summary>
    ///  DES解密
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public static string DESDeCode(string cipherText)
    {
        byte[] key = basekey;
        byte[] iv = baseiv;

        Aes encryptor = Aes.Create();

        encryptor.Mode = CipherMode.CBC;

        byte[] aesKey = new byte[32];
        Array.Copy(key, 0, aesKey, 0, 32);
        encryptor.Key = aesKey;
        encryptor.IV = iv;

        MemoryStream memoryStream = new MemoryStream();
        ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();
        CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);
        string plainText = String.Empty;

        try
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] plainBytes = memoryStream.ToArray();
            plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
        }
        finally
        {
            memoryStream.Close();
            cryptoStream.Close();
        }

        return plainText;
    }
    #endregion
}
