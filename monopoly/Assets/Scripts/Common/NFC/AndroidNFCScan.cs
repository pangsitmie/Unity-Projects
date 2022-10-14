using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

public static class AndroidNFCScan
{

    public static AndroidJavaObject NowIntent()
    {
        return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getIntent");
    }

    public static string ReadNFCID()
    {
        AndroidJavaObject NFC = new AndroidJavaClass("com.winpro.winproeat.nfc_android.NfcParser").CallStatic<AndroidJavaObject>("getNTag", NowIntent());
        var NFCID = NFC.Call<string>("getUID");
        NFC.Call("release");
        return NFCID;
    }
    public static byte[] ReadPack(byte[] NFCKey)
    {
        AndroidJavaObject NFC = new AndroidJavaClass("com.winpro.winproeat.nfc_android.NfcParser").CallStatic<AndroidJavaObject>("getNTag", NowIntent());
        var Pack = NFC.Call<byte[]>("pwdAuth", NFCKey);
        NFC.Call("release");
        return Pack;
    }
    public static byte[] DecodeNFCData(byte[] NFCKey, int Page)
    {
        AndroidJavaObject NFC = new AndroidJavaClass("com.winpro.winproeat.nfc_android.NfcParser").CallStatic<AndroidJavaObject>("getNTag", NowIntent());
        var pack = NFC.Call<byte[]>("pwdAuth", NFCKey);
        Debug.Log("pack: " + NFCdataHandler.ByteArrayToString(pack));
        var NFCData = NFC.Call<byte[]>("read", Page);
        NFC.Call("release");
        return NFCData;
    }

}