using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using XCharts;
public class MachineGameResultObject : MonoBehaviour
{
    DateTime timeUp;
    [SerializeField] Text message_tx, confirm_tx;
    [SerializeField] BarChart resultBarChart;
    private System.Action ConfirmEvent, CancelEvent;

    //====================================================================
    /// <summary>
    /// 設定資料
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 載入遊戲結果視窗的資料</para>
    /// </summary>
    public void Set(SocketIOStruct.machineTypeResponse_struct _machineData, ResponseStruct.MachineGameResultResponse_struct _gameResultData, System.Action _ConfirmEvent,
                     System.Action _CancelEvent)
    {
        switch (_machineData.machineType)
        {
            default:
                System.Text.StringBuilder _string_strBd = new System.Text.StringBuilder();
                //if (DateTime.TryParse(_gameResultData.playDate, out timeUp))
                //{
                //    timeUp = DateTime.Now;
                //    timeUp = timeUp.AddSeconds(60);
                //    timeUp = DateTime.Now.AddSeconds(60);
                //    Debug.Log("Game again timeUp:" + timeUp.ToString());
                //}
                //else
                //{
                //    Debug.Log("DateTime.TryParse failed");
                //}

                timeUp = DateTime.Now;
                timeUp = timeUp.AddSeconds(60);
                timeUp = DateTime.Now.AddSeconds(60);
                Debug.Log("Game again timeUp:" + timeUp.ToString());

                _string_strBd.Clear()
                            .Append("遊戲種類: 彈珠台\n")
                            .Append("遊戲結束時間: \n\t").Append(_gameResultData.playDate).Append("\n");
                _string_strBd.Append("\n\n總獲得彩票: ").Append(_gameResultData.ticket);
                for (int i = 0; i < _gameResultData.beadResult.Length; i++)
                {
                    resultBarChart.UpdateData(0, i, _gameResultData.beadResult[i]);
                    Debug.Log(("第 " + (i + 1).ToString("00") + " 洞: " + _gameResultData.beadResult[i]));
                }

                message_tx.text = _string_strBd.ToString();
                ConfirmEvent = _ConfirmEvent;
                CancelEvent = _CancelEvent;
                break;
        }

    }
    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 按下確認按鍵時觸發</para>
    /// </summary>
    public void Confirm()
    {
        if (ConfirmEvent != null)
            ConfirmEvent();
        Destroy(this.gameObject);
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 按下取消按鍵時觸發</para>
    /// </summary>
    public void Cancel()
    {
        if (CancelEvent != null)
            CancelEvent();
        Destroy(this.gameObject);
    }
}
