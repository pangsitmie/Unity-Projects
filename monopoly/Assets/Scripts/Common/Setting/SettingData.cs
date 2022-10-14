using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Localization.Settings;

[System.Serializable]
public class SettingDataStruct
{
    public string phoneTitle = "+886";
    public string phoneNumber = "";
    public string language = LocalizationSettings.SelectedLocale.name;
    public string colorMode = "darkMode";
    public string firebase = "";
    public bool keepLogin = true;
    public bool skipTicketAnimation = false;
    public string token = "";
    public string SMSTime = "can";
}

/// <summary>
/// 設定檔管理
/// <para>編輯者: Dimos</para>
/// <para>SettingData.Data裡為設定檔的資料</para>
/// <para>初始化時須執行ReadData()以讀取檔案</para>
/// </summary>
public class SettingData : MonoBehaviour
{
    static SettingDataStruct data;
    public static SettingDataStruct Data { get { return data; } set { data = value; } }

    //==============================================================================================
    /// <summary>
    /// 讀取設定檔
    /// <para>編輯者: zxft</para>
    /// <para>如設定檔存在(SettingData.json)，讀取其資料</para>
    /// <para>如設定檔不存在(SettingData.json)，生成新設定檔</para>
    /// </summary>
    public static void ReadData()
    {
        if (File.Exists(Application.persistentDataPath + "/SettingData"))
        {
            using (StreamReader _file = new StreamReader(Application.persistentDataPath + "/SettingData"))
            {
                //try
                //{
                //    data = JsonUtility.FromJson<SettingDataStruct>(_file.ReadToEnd());
                //    return;
                //}
                //catch (System.Exception ex)
                //{
                //    Debug.LogException(ex);
                //}
                try
                {
                    data = JsonUtility.FromJson<SettingDataStruct>(AESManager.DESDeCode(_file.ReadToEnd()));
                    return;
                }
                catch (System.Exception ex)
                {

                    Debug.LogException(ex);
                }
            }
            data = new SettingDataStruct();
            UpdateData();
            /*2021_07_29改寫為上方
            StreamReader _file = new StreamReader(Application.persistentDataPath + "/SettingData.json");
            data = JsonUtility.FromJson<SettingDataStruct>(_file.ReadToEnd());
            _file.Close();
            */
        }
        else
        {
            data = new SettingDataStruct();
            UpdateData();
        }
    }

    //==============================================================================================
    /// <summary>
    /// 寫入設定檔
    /// <para>編輯者: zxft</para>
    /// <para>以Json格式儲存Data</para>
    /// </summary>
    public static void UpdateData()
    {
        using (StreamWriter _file = new StreamWriter(Application.persistentDataPath + "/SettingData"))
        {
            try
            {
                _file.Write(AESManager.DESEnCode(JsonUtility.ToJson(data)));
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /*2021_07_29改寫為上方
        StreamWriter _file = new StreamWriter(Application.persistentDataPath + "/SettingData.json");
        _file.Write(JsonUtility.ToJson(data));
        _file.Close();
        */
    }

    //==============================================================================================
}
