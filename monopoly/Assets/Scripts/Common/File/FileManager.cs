using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager
{
    public static class FileName
    {
        public const string PrivateMsgHaveReadFile = "PrivateMsgHaveRead";
    }
    //==============================================================================================
    /// <summary>
    /// 讀取檔案
    /// </summary>
    /// <param name="_fileName_str">該檔案名稱</param>
    /// <returns>讀取失敗為傳Null</returns>
    public static string ReadData(string _fileName_str)
    {
        if (!File.Exists(Application.persistentDataPath + "/" + _fileName_str))
        {
            return null;
        }

        using (StreamReader _file = new StreamReader(Application.persistentDataPath + "/" + _fileName_str))
        {
            try
            {
                return AESManager.DESDeCode(_file.ReadToEnd());
            }
            catch (System.Exception ex)
            {

                Debug.LogException(ex);
            }
        }

        return null;

    }

    //==============================================================================================
    /// <summary>
    /// 寫入檔案
    /// </summary>
    /// <param name="_fileName_str">該檔案名稱</param>
    /// <param name="_contnet_str">寫入內容</param>
    /// <returns>回傳true為成功，false為失敗</returns>
    public static bool WriteData(string _fileName_str, string _contnet_str)
    {
        using (StreamWriter _file = new StreamWriter(Application.persistentDataPath + "/" + _fileName_str))
        {
            try
            {
                _file.Write(AESManager.DESEnCode(_contnet_str));
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        return false;
    }

}
