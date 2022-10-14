using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 根據地址資訊取得郵遞區號
/// </summary>
public class GetDistricts : MonoBehaviour
{
    private static Dictionary<string, Dictionary<string, string>> taiwanDistricts_dict = new Dictionary<string, Dictionary<string, string>>();

    //====================================================================
    /// <summary>
    /// 讀取郵遞區號文本資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private static void LoadDistricts()
    {
        TextAsset _targetFile = Resources.Load<TextAsset>(@"Data/taiwan_districts");
        var _taiwanDistrictsData = JsonUtility.FromJson<DataStruct.TaiwanDistrictsData_struct>(_targetFile.text);

        for (int i = 0; i < _taiwanDistrictsData.taiwan_districts.Length; i++)
        {
            DataStruct.CountryData_struct _countryData_struct = _taiwanDistrictsData.taiwan_districts[i];
            taiwanDistricts_dict.Add(_countryData_struct.name, new Dictionary<string, string>());
            for (int j = 0; j < _countryData_struct.districts.Length; j++)
            {
                DataStruct.DistrictsData_struct _districtsData_struct = _countryData_struct.districts[j];
                taiwanDistricts_dict[_countryData_struct.name].Add(_districtsData_struct.name, _districtsData_struct.zip);
            }
        }
    }

    //====================================================================
    /// <summary>
    /// 取得縣市的下拉選單列表
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public static List<Dropdown.OptionData> GetCountryOptions()
    {
        if (taiwanDistricts_dict.Count == 0)
        {
            LoadDistricts();
        }

        List<Dropdown.OptionData> _counties_list = new List<Dropdown.OptionData>();

        var _enumerator = taiwanDistricts_dict.GetEnumerator();
        while (_enumerator.MoveNext())
        {
            _counties_list.Add(new Dropdown.OptionData(_enumerator.Current.Key));
        }
        return _counties_list;
    }

    //====================================================================
    /// <summary>
    /// 取得鄉鎮的下拉選單列表
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public static List<Dropdown.OptionData> GetDistrictOptions(string _countryName_str)
    {
        if (taiwanDistricts_dict.Count == 0)
        {
            LoadDistricts();
        }

        if (taiwanDistricts_dict.TryGetValue(_countryName_str, out var districts_dict))
        {
            List<Dropdown.OptionData> _districts_list = new List<Dropdown.OptionData>();

            var _enumerator = districts_dict.GetEnumerator();
            while (_enumerator.MoveNext())
            {
                _districts_list.Add(new Dropdown.OptionData(_enumerator.Current.Key));
            }
            return _districts_list;
        }
        else
        {
            return null;
        }
    }

    //====================================================================
    /// <summary>
    /// 取得郵遞區號
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public static string GetDistrictZip(string _countryName_str, string _districtName_str)
    {
        if (taiwanDistricts_dict.Count == 0)
        {
            LoadDistricts();
        }

        if (taiwanDistricts_dict.TryGetValue(_countryName_str, out var districts_dict))
        {
            if (districts_dict.TryGetValue(_districtName_str, out var _zip_str))
            {
                return _zip_str;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    //====================================================================
}
