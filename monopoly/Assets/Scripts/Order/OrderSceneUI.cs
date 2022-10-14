using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 處理OrderScene的基礎UI
/// <para>(目前主要負責 進行中/歷史 訂單的顯示切換)</para>
/// </summary>
public class OrderSceneUI : MonoBehaviour
{
    [SerializeField] MoveOut inProgressOrderScrollView_moveOut;
    [SerializeField] MoveOut completeOrderScrollView_moveOut;

    //====================================================================
    /// <summary>
    /// 顯示進行中訂單的畫面
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void DisplayInProgressOrder(bool _status_bl)
    {
        if (_status_bl)
        {
            inProgressOrderScrollView_moveOut.Move(true);
            completeOrderScrollView_moveOut.Move(false);
        }
    }

    //====================================================================
    /// <summary>
    /// 顯示歷史訂單的畫面
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void DisplayCompleteOrder(bool _status_bl)
    {
        if (_status_bl)
        {
            completeOrderScrollView_moveOut.Move(true);
            inProgressOrderScrollView_moveOut.Move(false);
        }
    }

    //====================================================================
}
