using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 處理詳細訂單資料的UI顯示
/// </summary>
public class OrderDetail : MonoBehaviour
{
    private const int commodityStartIndex = 5;

    //需求的UI與功能，和商城結帳畫面內的商品物件相似，故使用同樣Prefab/Script
    [SerializeField] CheckoutCommodityObject commodityObject_pf;
    [SerializeField] Transform content_transform;

    [SerializeField] GameObject deliveryTitle_gObj, deliveryBlock_gObj;
    [SerializeField] Text logisticsName_tx, deliveryId_tx, deliveryTime_tx;

    [SerializeField] Text recipientName_tx, recipientPhone_tx, recipientAddress_tx;

    [SerializeField] Text subtotal_tx, deliveryFee_tx, total_tx;

    [SerializeField] LocalizationText orderStatus_tx;
    [SerializeField] Text orderId_tx, orderDate_tx;

    [SerializeField] MoveOut this_moveOut;

    [SerializeField] List<CheckoutCommodityObject> commodityObjects_list = new List<CheckoutCommodityObject>();
    [SerializeField] ResponseStruct.OrderInfo_struct orderData;

    //====================================================================
    /// <summary>
    /// 設定訂單詳細資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Set(ResponseStruct.OrderInfo_struct _orderData)
    {
        orderData = _orderData;

        LoadDeliveryInformation();
        LoadRecipientInformaion();
        LoadOrderContent();
        LoadOrderInformation();

        this_moveOut.Move(true);

        //將關閉視窗加入返回事件-------------------------------------------------------
        UnityEngine.Events.UnityEvent _returnEvent = new UnityEngine.Events.UnityEvent();
        _returnEvent.AddListener(CloseOrderDetailView);
        ReturnManager.Instance.PushReturnEvent(_returnEvent);
        //---------------------------------------------------------------------------
    }

    //====================================================================
    /// <summary>
    /// 載入運送資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void LoadDeliveryInformation()
    {
        try
        {
            if (string.IsNullOrEmpty(orderData.logisticsName) && string.IsNullOrEmpty(orderData.deliveryId))
            {
                deliveryTitle_gObj.SetActive(false);
                deliveryBlock_gObj.SetActive(false);
            }
            else
            {
                deliveryTitle_gObj.SetActive(true);
                deliveryBlock_gObj.SetActive(true);

                if (!string.IsNullOrEmpty(orderData.logisticsName))
                {
                    logisticsName_tx.text = orderData.logisticsName;
                }
                else
                {
                    logisticsName_tx.text = "Unknown";
                    Debug.LogError("OrderDetail.LoadDeliveryInformation() LogisticsName is null");
                }

                if (!string.IsNullOrEmpty(orderData.deliveryId))
                {
                    deliveryId_tx.text = orderData.deliveryId;
                }
                else
                {
                    deliveryId_tx.text = "Unknown";
                    Debug.LogError("OrderDetail.LoadDeliveryInformation() DeliveryId is null");
                }

                if (System.DateTime.TryParse(orderData.deliveryTime, out System.DateTime _deliveryTime))
                {
                    deliveryTime_tx.text = _deliveryTime.ToString(@"yyyy/MM/dd HH:mm:ss");
                }
                else
                {
                    deliveryTime_tx.text = "Unknown";
                    Debug.LogError("OrderDetail.LoadDeliveryInformation() Unable to parse " + orderData.deliveryTime);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex, this);
        }
    }

    //====================================================================
    /// <summary>
    /// 載入收件人資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void LoadRecipientInformaion()
    {
        try
        {
            if (!string.IsNullOrEmpty(orderData.name))
            {
                recipientName_tx.text = orderData.name;
            }
            else
            {
                recipientName_tx.text = "Unknown";
                Debug.LogError("OrderDetail.LoadRecipientInformaion() Name is null");
            }

            if (!string.IsNullOrEmpty(orderData.phone.number))
            {
                recipientPhone_tx.text = orderData.phone.number;
            }
            else
            {
                recipientPhone_tx.text = "Unknown";
                Debug.LogError("OrderDetail.LoadRecipientInformaion() Phone Number is null");
            }

            if (!string.IsNullOrEmpty(orderData.address))
            {
                recipientAddress_tx.text = orderData.address;
            }
            else
            {
                recipientAddress_tx.text = "Unknown";
                Debug.LogError("OrderDetail.LoadRecipientInformaion() Address is null");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex, this);
        }
    }

    //====================================================================
    /// <summary>
    /// 載入訂單內容
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void LoadOrderContent()
    {
        try
        {
            for (int i = 0; i < commodityObjects_list.Count; i++)
            {
                var _commodityObject = commodityObjects_list[i];
                Destroy(_commodityObject.gameObject);
            }
            commodityObjects_list.Clear();

            if (orderData.details != null)
            {
                for (int i = 0; i < orderData.details.Length; i++)
                {
                    var _detail = orderData.details[i];
                    var _commodityObject = Instantiate(commodityObject_pf, content_transform);
                    _commodityObject.transform.SetSiblingIndex(i + commodityStartIndex);
                    _commodityObject.Set(_detail.commodityInfo, _detail.commodityInfo.quantity);
                    commodityObjects_list.Add(_commodityObject);
                }
            }
            else
            {
                Debug.LogError("OrderDetail.LoadOrderContent() OrderData.Details is null");
            }

            //伺服器僅提供總金額與運費，故小計為自己計算的--------------------------------------------
            subtotal_tx.text = (orderData.commodityTotalPrice - orderData.deliveryFee).ToString();
            deliveryFee_tx.text = orderData.deliveryFee.ToString();
            total_tx.text = orderData.commodityTotalPrice.ToString();
            //--------------------------------------------------------------------------------
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex, this);
        }
    }

    //====================================================================
    /// <summary>
    /// 載入訂單資訊(訂單狀態、ID、建立時間)
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void LoadOrderInformation()
    {
        try
        {
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

            orderId_tx.text = orderData.id.ToString();

            if (System.DateTime.TryParse(orderData.orderDate, out System.DateTime _orderDate))
            {
                orderDate_tx.text = _orderDate.ToString(@"yyyy/MM/dd HH:mm:ss");
            }
            else
            {
                orderDate_tx.text = "Unknown";
                Debug.LogError("OrderDetail.LoadOrderInformation() Unable to parse " + orderData.orderDate);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex, this);
        }
    }

    //====================================================================
    /// <summary>
    /// 關閉訂單詳細視窗
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void CloseOrderDetailView()
    {
        this_moveOut.Move(false);
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
}
