using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Canvas相關參數及處理
/// </summary>
public class CanvasElement : MonoBehaviour
{
    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 將當前場景的Canvas登記到SceneController</para>
    /// </summary>
    private void Start()
    {
        SceneController.Instance.CanvasRegister(this);
    }

    //====================================================================
}
