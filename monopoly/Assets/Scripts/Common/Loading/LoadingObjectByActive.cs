using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 顯示載入中的畫面動畫
/// <para>操作方式: 繼承於LoadingObject，僅添加能根據Enable狀態觸發</para>
/// </summary>
public class LoadingObjectByActive : LoadingObject
{
    //====================================================================
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        StartLoading();
    }

    //====================================================================
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        StopLoading();
    }

    //====================================================================
}
