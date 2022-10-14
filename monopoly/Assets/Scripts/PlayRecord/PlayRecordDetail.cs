using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCharts;

public class PlayRecordDetail : MonoBehaviour
{
    [SerializeField] MoveOut this_moveOut;
    [SerializeField] Text message_tx;
    [SerializeField] BarChart resultBarChart;
    private System.Action ConfirmEvent, CancelEvent;

    //====================================================================
    /// <summary>
    /// 設定資料
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 載入遊戲結果視窗的資料</para>
    /// </summary>
    public void Set(ResponseStruct.IPinballGameResult_struct _gameResultData)
    {

        System.Text.StringBuilder _string_strBd = new System.Text.StringBuilder();
        _string_strBd.Clear()
                    .Append("遊戲種類: 彈珠台\n")
                    .Append("機台編號: \n").Append(_gameResultData.machineInfo.details.managementId).Append("\n")
                    .Append("遊戲結束時間: \n\t").Append(_gameResultData.playDate).Append("\n");
        _string_strBd.Append("\n\n總獲得彩票: ").Append(_gameResultData.ticket);
        var _beads = _gameResultData.beadResult.Split(',');
        for (int i = 0; i < _beads.Length; i++)
        {
            resultBarChart.UpdateData(0, i, int.Parse(_beads[i]));
            //_string_strBd.Append("\n第 ").Append((i + 1).ToString("00")).Append(" 洞: ").Append(_gameResultData.bead[i]);
        }
        message_tx.text = _string_strBd.ToString();
        this_moveOut.Move(true);
        UnityEngine.Events.UnityEvent _ruturnEvent = new UnityEngine.Events.UnityEvent();
        _ruturnEvent.AddListener(Quit);
        ReturnManager.Instance.PushReturnEvent(_ruturnEvent);
    }
    public void Quit()
    {
        this_moveOut.Move(false);

        ReturnManager.Instance.PopReturnEvent();
        for (int i = 0; i < 16; i++)
        {
            resultBarChart.UpdateData(0, i, 0);
            //_string_strBd.Append("\n第 ").Append((i + 1).ToString("00")).Append(" 洞: ").Append(_gameResultData.bead[i]);
        }
    }
}
