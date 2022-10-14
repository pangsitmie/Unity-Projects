using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 
/// </summary>
public class SetValue
{
    //====================================================================
    /// <summary>
    /// 檢查更改前後是否相同
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 如果新舊數值相同將回傳false; 不同則回傳true，並將數值設定為新的</para>
    /// </summary>
    public static bool SetStructValue<T>(ref T currentValue, T newValue) where T : struct
    {
        if (currentValue.Equals(newValue))
            return false;

        currentValue = newValue;
        return true;
    }

    //====================================================================
    /// <summary>
    /// 檢查更改前後是否相同
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 如果新舊數值相同將回傳false; 不同則回傳true，並將數值設定為新的</para>
    /// </summary>
    public static bool SetClassValue<T>(ref T currentValue, T newValue) where T : class
    {
        if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
            return false;

        currentValue = newValue;
        return true;
    }

    //=========================================================================
}
