using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 紀錄各筆訂單資訊，並處理簡略訂單資料的UI顯示
/// </summary>
public class OrderObject : MonoBehaviour
{
    [SerializeField] Text orderDate_tx, commodityAmount_tx, ticketTotal_tx;
    [SerializeField] LocalizationText orderStatus_tx;

    private ResponseStruct.OrderInfo_struct orderData;
    private System.Action<ResponseStruct.OrderInfo_struct> OpenOrderDetailAction;

    //====================================================================
    /// <summary>
    /// 設定訂單資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Set(ResponseStruct.OrderInfo_struct _orderData, System.Action<ResponseStruct.OrderInfo_struct> _OpenOrderDetailAction)
    {
        try
        {
            if (_OpenOrderDetailAction == null)
            {
                Debug.LogError("OpenOrderDetailAction cannot be null");
                //throw new System.ArgumentException("OpenOrderDetailAction cannot be null");
            }
            else
            {
                OpenOrderDetailAction = _OpenOrderDetailAction;
            }

            orderData = _orderData;

            if (System.DateTime.TryParse(orderData.orderDate, out System.DateTime _orderDate))
            {
                orderDate_tx.text = _orderDate.ToString(@"yyyy/MM/dd HH:mm:ss");
            }
            else
            {
                orderDate_tx.text = "Unknown";
                Debug.LogError("OrderObject.Set() Unable to parse " + orderData.orderDate);
            }

            switch (orderData.status.statement)
            {
                case "已出貨":
                    orderStatus_tx.Set(LocalizationManager.KeyTable.order_statusOK);
                    break;
                case "準備中":
                    orderStatus_tx.Set(LocalizationManager.KeyTable.order_statusPreparing);
                    break;
                default:
                    orderStatus_tx.Set(LocalizationManager.KeyTable.common_unknown);
                    break;
            }

            if (orderData.details != null)
            {
                commodityAmount_tx.text = orderData.details.Length.ToString();
            }
            else
            {
                commodityAmount_tx.text = "Unknown";
                Debug.LogError("OrderObject.Set() OrderData.Details is null");
            }

            ticketTotal_tx.text = orderData.commodityTotalPrice.ToString();
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex, this);
        }
    }

    //====================================================================
    /// <summary>
    /// 開啟訂單詳細的畫面
    /// <para>編輯者: Dimos</para>
    /// <para>OpenOrderDetailAction會在建立OrderObject時注入</para>
    /// </summary>
    public void OpenOrderDetail()
    {
        OpenOrderDetailAction.Invoke(orderData);
    }

    //====================================================================
}
