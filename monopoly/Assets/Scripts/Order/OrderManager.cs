using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using GraphQlClient.Core;
/// <summary>
/// 
/// </summary>
public class OrderManager : MonoBehaviour
{
    [SerializeField] LoadingObjectManager loadingObjectManager;

    /// <summary>
    /// 現有的進行中訂單資料
    /// </summary>
    private List<OrderObject> inProgressOrderObjects_list = new List<OrderObject>();

    /// <summary>
    /// 現有的已完成訂單資料
    /// </summary>
    private List<OrderObject> completedOrderObjects_list = new List<OrderObject>();

    [SerializeField] Transform inProgressContent_transform, completedContent_transform;
    [SerializeField] GameObject inProgressEmptyHint_gObj, completedEmptyHint_gObj;

    [SerializeField] OrderObject orderObject_pf;
    [SerializeField] OrderDetail orderDetail;

    //====================================================================
    private void Start()
    {
        GetAllOrderInformation();
    }

    //====================================================================
    /// <summary>
    /// 取得所有訂單紀錄
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void GetAllOrderInformation()
    {
        loadingObjectManager.LoadingCountAdd();
        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetQueryByName.allOrderInformation, GraphApi.Query.Type.Query);
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, GetAllOrderInformationCallback));
    }

    //====================================================================
    /// <summary>
    /// 取得所有訂單紀錄的回傳
    /// <para>編輯者: Dimos</para>
    /// <para>建立前會先根據運送資料的有無分類成進行中訂單與歷史訂單</para>
    /// </summary>
    private void GetAllOrderInformationCallback(string _responseData_str)
    {
        if (!string.IsNullOrEmpty(_responseData_str))
        {
            try
            {
                var _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.OrderInfoResponse_struct>>(_responseData_str);

                if (_responseData.errors == null)
                {
                    List<ResponseStruct.OrderInfo_struct> _inProgressOrderDatas_list = new List<ResponseStruct.OrderInfo_struct>();
                    List<ResponseStruct.OrderInfo_struct> _completedOrderDatas_list = new List<ResponseStruct.OrderInfo_struct>();
                    for (int i = 0; i < _responseData.data.allOrderInfo.Length; i++)
                    {
                        var _orderData = _responseData.data.allOrderInfo[i];
                        if (string.IsNullOrEmpty(_orderData.deliveryId))
                        {
                            _inProgressOrderDatas_list.Add(_orderData);
                        }
                        else
                        {
                            _completedOrderDatas_list.Add(_orderData);
                        }
                    }

                    BuildOrderList(_inProgressOrderDatas_list.ToArray(), inProgressContent_transform, inProgressEmptyHint_gObj, ref inProgressOrderObjects_list);
                    BuildOrderList(_completedOrderDatas_list.ToArray(), completedContent_transform, completedEmptyHint_gObj, ref completedOrderObjects_list);
                }
                else
                {
                    switch (_responseData.errors[0].extensions.code)
                    {
                        default:
                            GraphQLManager.ErrorCodeHandle(_responseData.errors[0]);
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex, this);
            }
        }

        loadingObjectManager.LoadingCountSub();
    }

    //====================================================================
    /// <summary>
    /// 建立訂單物件列表
    /// <para>編輯者: Dimos</para>
    /// </summary>
    /// <param name="_newOrderData_arry">新的訂單資料</param>
    /// <param name="_content_transform">訂單資料的顯示(生成)位置</param>
    /// <param name="_emptyHint_gObj">當新訂單資料為空時的提示物件</param>
    /// <param name="_orderObjects_list">現有的訂單資料</param>
    public void BuildOrderList(ResponseStruct.OrderInfo_struct[] _newOrderData_arry, Transform _content_transform, GameObject _emptyHint_gObj, ref List<OrderObject> _orderObjects_list)
    {
        if (_newOrderData_arry != null)
        {
            if (_newOrderData_arry.Length <= _orderObjects_list.Count)
            {
                int _count = 0;

                for (; _count < _newOrderData_arry.Length; _count++)
                {
                    _orderObjects_list[_count].Set(_newOrderData_arry[_count], OpenOrderDetail);
                }

                for (; _count < _orderObjects_list.Count;)
                {
                    Destroy(_orderObjects_list[_count].gameObject);
                    _orderObjects_list.Remove(_orderObjects_list[_count]);
                }
            }
            else
            {
                int _count = 0;

                for (; _count < _orderObjects_list.Count; _count++)
                {
                    _orderObjects_list[_count].Set(_newOrderData_arry[_count], OpenOrderDetail);
                }

                for (; _count < _newOrderData_arry.Length; _count++)
                {
                    var _orderObject = Instantiate(orderObject_pf, _content_transform);
                    _orderObject.Set(_newOrderData_arry[_count], OpenOrderDetail);
                    _orderObjects_list.Add(_orderObject);
                }
            }
        }
        else
        {
            for (int i = 0; i < _orderObjects_list.Count; i++)
            {
                Destroy(_orderObjects_list[i].gameObject);
            }
            _orderObjects_list.Clear();
        }

        _emptyHint_gObj.SetActive(_newOrderData_arry == null || _newOrderData_arry.Length == 0);
    }

    //====================================================================
    /// <summary>
    /// 開啟訂單詳細的畫面
    /// <para>編輯者: Dimos</para>
    /// <para>此Function會在建立OrderObject時注入OrderObject.cs</para>
    /// </summary>
    public void OpenOrderDetail(ResponseStruct.OrderInfo_struct _orderData)
    {
        orderDetail.Set(_orderData);
    }

    //====================================================================
    private EventView closeView;

    /// <summary>
    /// 點擊返回鍵後的顯示與處理(此函式由ReturnEventHandler添加)
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void AddReturnEvent()
    {
        closeView = NotificationManager.Instance.CreateEventView(LocalizationManager.KeyTable.common_exitComfirmViewText, CloseViewConfirmCallback, CloseViewCancelCallback);
        UnityEvent _CloseViewReturnEvent = new UnityEvent();
        _CloseViewReturnEvent.AddListener(CloseViewCancelCallback);
        ReturnManager.Instance.PushReturnEvent(_CloseViewReturnEvent);
    }

    //====================================================================
    private void CloseViewConfirmCallback()
    {
        Application.Quit();
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
    private void CloseViewCancelCallback()
    {
        Destroy(closeView.gameObject);
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
}
