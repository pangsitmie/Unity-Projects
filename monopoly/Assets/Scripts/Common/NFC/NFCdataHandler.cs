using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class NFCdataHandler
{

    public static byte[] IntArrayToByteArray(int[] IntA)
    {
        byte[] ByteA = new byte[IntA.Length];
        for (int i = 0; i < IntA.Length; i++)
        {
            ByteA[i] = Convert.ToByte(IntA[i]);
        }
        return ByteA;
    }

    public static int[] ByteArrayToIntArray(byte[] ByteA)
    {
        int[] IntA = new int[ByteA.Length];
        for (int i = 0; i < ByteA.Length; i++)
        {
            IntA[i] = Convert.ToInt32(ByteA[i]);
        }
        return IntA;
    }

    public static byte[] HexStringArrayToByteArray(string[] HexA)
    {
        byte[] ByteA = new byte[HexA.Length];
        for (int i = 0; i < HexA.Length; i++)
        {
            ByteA[i] = Convert.ToByte(Convert.ToInt32(HexA[i], 16));
        }
        return ByteA;
    }

    public static string[] StringtoStringArray(string String, int Len)
    {
        string[] StringA = new string[String.Length / Len];
        for (int i = 0; i < String.Length / 2; i++)
        {
            StringA[i] = String.Substring(i * 2, 2);
        }
        return StringA;
    }

    public static string ByteArrayToString(byte[] ByteA)
    {
        return BitConverter.ToString(ByteA).Replace("-", string.Empty);
    }
}
