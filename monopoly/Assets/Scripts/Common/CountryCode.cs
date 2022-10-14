using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 管理 使用者看的電話國際碼/國家GEC代碼
/// </summary>
public class CountryCode : MonoBehaviour
{
    //PhoneTitle : 給使用者看的電話國際碼 e.g. TW(+886), +886
    //Location : 國家 GEC代碼 e.g. TW, CN, etc.

    private static readonly Dictionary<string, string> countryPhoneTitleToLocation_dict = new Dictionary<string, string>()
    {
        { "TW(+886)", "TW" }
    };

    private static readonly Dictionary<string, string> countryLocationToPhoneTitle_dict = new Dictionary<string, string>()
    {
        { "TW", "+886"}
    };

    //====================================================================
    /// <summary>
    /// 取得電話國際碼的Dropdown列表
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public static List<Dropdown.OptionData> GetAllPhoneTitleOptions()
    {
        List<Dropdown.OptionData> _optionDatas = new List<Dropdown.OptionData>();
        foreach (var _country in countryPhoneTitleToLocation_dict)
        {
            _optionDatas.Add(new Dropdown.OptionData(_country.Key));
        }
        return _optionDatas;
    }

    //====================================================================
    /// <summary>
    /// 取得對應的國家代碼
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 如果找不到對應的Key會回傳Null</para>
    /// </summary>
    public static string GetCountryLocationByPhoneTitle(string _phoneTitle_str)
    {
        if (countryPhoneTitleToLocation_dict.TryGetValue(_phoneTitle_str, out string _location_str))
        {
            return _location_str;
        }
        else
        {
            return null;
        }
    }

    //====================================================================
    /// <summary>
    /// 取得對應的電話國際碼
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 如果找不到對應的Key會回傳Null</para>
    /// </summary>
    public static string GetCountryPhoneTitleByLocation(string _location_str)
    {
        if (countryLocationToPhoneTitle_dict.TryGetValue(_location_str, out string _phoneTitle_str))
        {
            return _phoneTitle_str;
        }
        else
        {
            return null;
        }
    }

    //====================================================================
}
