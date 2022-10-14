using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayRecordObject : MonoBehaviour
{
    [SerializeField] Text playDate_tx, ticketTotal_tx;
    [SerializeField] Text machineType_tx;
    private ResponseStruct.IPinballGameResult_struct playRecordData;
    private System.Action<ResponseStruct.IPinballGameResult_struct> OpenPlayRecordDetailAction;

    //====================================================================
    /// <summary>
    /// 設定遊玩紀錄資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Set(ResponseStruct.IPinballGameResult_struct _playRecordData, System.Action<ResponseStruct.IPinballGameResult_struct> _OpenPlayRecordDetailAction)
    {
        try
        {
            if (_OpenPlayRecordDetailAction == null)
            {
                Debug.LogError("OpenOrderDetailAction cannot be null");
                //throw new System.ArgumentException("OpenOrderDetailAction cannot be null");
            }
            else
            {
                OpenPlayRecordDetailAction = _OpenPlayRecordDetailAction;
            }

            playRecordData = _playRecordData;

            if (System.DateTime.TryParse(playRecordData.playDate, out System.DateTime _orderDate))
            {
                playDate_tx.text = _orderDate.ToString(@"yyyy/MM/dd HH:mm:ss");
            }
            else
            {
                playDate_tx.text = "Unknown";
                Debug.LogError("OrderObject.Set() Unable to parse " + playRecordData.playDate);
            }

            machineType_tx.text = playRecordData.machineInfo.details.typeInfo.name;

            ticketTotal_tx.text = playRecordData.ticket.ToString();
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex, this);
        }
    }

    //====================================================================
    /// <summary>
    /// 開啟遊玩紀錄詳細的畫面
    /// <para>編輯者: Dimos</para>
    /// <para>OpenPlayRecordDetailAction會在建立PlayRecordObject時注入</para>
    /// </summary>
    public void OpenPlayRecordDetail()
    {
        OpenPlayRecordDetailAction.Invoke(playRecordData);
    }

    //====================================================================
}
