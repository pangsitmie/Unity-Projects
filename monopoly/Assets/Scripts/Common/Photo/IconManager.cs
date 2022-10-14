using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class IconManager : MonoBehaviour
{
    static IconManager instance = null;
    public static IconManager Instance { get => instance; }

    private Dictionary<int, string> icon_dict = new Dictionary<int, string>();// Dictionary<id, path>
    public System.Action OnIconUpdate;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        instance = this;
    }
    //==============================================================================================
    /// <summary>
    /// 開場讀取購物車
    /// </summary>
    private void Start()
    {
        ReadLocalData();
    }

    //==============================================================================================
    /// <summary>
    /// 讀取儲存於本地的頭像資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void ReadLocalData()
    {
        if (File.Exists(Application.persistentDataPath + "/IconData.json"))
        {
            using (StreamReader _file = new StreamReader(Application.persistentDataPath + "/IconData.json"))
            {
                try
                {
                    icon_dict = JsonUtility.FromJson<Serialization<int, string>>(_file.ReadToEnd()).ToDictionary();
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
        else
        {
            //baseData_dict.Clear();
            //baseData_dict = new Dictionary<int, int>();
            UpdateLocalData();
        }
        OnIconUpdate?.Invoke();
    }

    //==============================================================================================
    /// <summary>
    /// 將頭像資料記錄於本地
    /// <para>編輯者: zxft</para>
    /// </summary>
    private void UpdateLocalData()
    {
        using (StreamWriter _file = new StreamWriter(Application.persistentDataPath + "/IconData.json"))
        {
            try
            {
                _file.Write(JsonUtility.ToJson(new Serialization<int, string>(icon_dict)));
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        OnIconUpdate?.Invoke();
    }
    public void SetIcon(int _id_int, string _path_str)
    {
        if (icon_dict.ContainsKey(_id_int))
        {
            if (_path_str != icon_dict[_id_int])
            {
                icon_dict[_id_int] = _path_str;
                UpdateLocalData();
            }

        }
        else
        {
            icon_dict.Add(_id_int, _path_str);
            UpdateLocalData();
        }

    }
    public string GetIcon(int _id_int)
    {
        if (icon_dict.ContainsKey(_id_int))
        {
            return icon_dict[_id_int];
        }
        else
        {
            return null;
        }
    }
}
