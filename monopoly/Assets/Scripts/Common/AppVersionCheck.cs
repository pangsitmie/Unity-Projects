using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 檢查版本號
/// <para>操作方式: </para>
/// </summary>
public class AppVersionCheck : MonoBehaviour
{
    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 請求檢查版本號再回傳結果</para>
    /// </summary>
    public static void CheckAppVersion(System.Action<int> _ResultCallback)
    {
        int _versionStatus = 1;
        /*
         * 需實作版本檢查，並根據檢查情況設定_versionStatus
         */
        if (_ResultCallback != null)
        {
            _ResultCallback(_versionStatus);
        }
    }

    //====================================================================
}
