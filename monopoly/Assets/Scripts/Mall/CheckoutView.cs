using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GraphQlClient.Core;
/// <summary>
/// 結帳畫面(讓使用者確認訂單的內容、收件人資訊、彩票花費，確認完即可送出)
/// </summary>
public class CheckoutView : MonoBehaviour
{
    private LoadingObjectManager loadingObjectManager;
    private ResponseStruct.RecipientInfo_struct recipientData;

    private readonly int commodityStartIndex = 1;

    private int shipping = 0;
    private int subtotal = 0;
    private int ticketLeft_int = 0;

    [SerializeField] CheckoutCommodityObject checkoutCommodityObject_pf;
    [SerializeField] Transform checkoutContent_transform;
    [SerializeField] Text currentTicketAmount_tx, subtotal_tx, shipping_tx, ticketLeft_tx;

    [SerializeField] List<RequestStruct.OrderCommodity_struct> commodities_list;
    [SerializeField] CheckoutRecipient checkoutRecipient;

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 登記更新文字事件</para>
    /// </summary>
    private void OnEnable()
    {
        RecipientManager.Instance.UpdateRecipientInfo += UpdateRecipient;
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 解除更新文字事件</para>
    /// </summary>
    private void OnDisable()
    {
        RecipientManager.Instance.UpdateRecipientInfo -= UpdateRecipient;
    }

    //====================================================================
    /// <summary>
    /// 設定結帳商品資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Set(List<DataStruct.CommodityCheckoutData_struct> _commodities, LoadingObjectManager _loadingObjectManager)
    {
        loadingObjectManager = _loadingObjectManager;
        subtotal = 0;
        commodities_list.Clear();

        UpdateRecipient();

        for (int i = 0; i < _commodities.Count; i++)
        {
            DataStruct.CommodityCheckoutData_struct _commodityCheckoutData_struct = _commodities[i];

            var _checkoutCommodityObject = Instantiate(checkoutCommodityObject_pf, checkoutContent_transform);
            _checkoutCommodityObject.transform.SetSiblingIndex(i + commodityStartIndex);
            _checkoutCommodityObject.Set(_commodityCheckoutData_struct.commodityData, _commodityCheckoutData_struct.amount);

            commodities_list.Add(new RequestStruct.OrderCommodity_struct()
            {
                commodityId = _commodityCheckoutData_struct.commodityData.id,
                quantity = _commodityCheckoutData_struct.amount
            });

            subtotal += _commodityCheckoutData_struct.amount * _commodityCheckoutData_struct.commodityData.price;
        }

        currentTicketAmount_tx.text = MemberInformationManager.Ticket_int.ToString();
        subtotal_tx.text = "-" + subtotal.ToString();
        shipping_tx.text = "-" + shipping.ToString();
        ticketLeft_int = MemberInformationManager.Ticket_int - subtotal - shipping;
        ticketLeft_tx.text = ticketLeft_int.ToString();
    }

    //====================================================================
    /// <summary>
    /// 更新收件人資料
    /// <para>編輯者: Dimos</para>
    /// <para>此Function也會在向伺服器請求收件人資料後觸發</para>
    /// </summary>
    private void UpdateRecipient()
    {
        recipientData = RecipientManager.Instance.GetRecipientData(0);
        Debug.Log("CheckoutView.UpdateRecipient() Get recipient data: " + JsonUtility.ToJson(recipientData));
        checkoutRecipient.Set(recipientData);
    }

    //====================================================================
    /// <summary>
    /// 使用者點擊結帳，經檢查無誤後，顯示再次確認視窗(EventView)
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Checkout()
    {
        if (recipientData.id == -1)
        {
            RecipientManager.Instance.OpenRecipientEditView();
        }
        else if (ticketLeft_int < 0)
        {
            NotificationManager.Instance.CreateConfirmView(LocalizationManager.KeyTable.mall_ticketNotEnoughError);
        }
        else
        {
            var _checkoutEventView = NotificationManager.Instance.CreateEventView(
                LocalizationManager.KeyTable.mall_checkoutWarnning, CheckoutEventConfirm, CheckoutEventCancel, LocalizationManager.KeyTable.mall_placeOrder);

            UnityEngine.Events.UnityEvent _QuitViewReturnEvent = new UnityEngine.Events.UnityEvent();
            _QuitViewReturnEvent.AddListener(() =>
            {
                Destroy(_checkoutEventView.gameObject);
                ReturnManager.Instance.PopReturnEvent();
            });
            ReturnManager.Instance.PushReturnEvent(_QuitViewReturnEvent);
        }
    }

    //====================================================================
    /// <summary>
    /// 再次確認視窗內點擊"確認"
    /// </summary>
    private void CheckoutEventConfirm()
    {
        loadingObjectManager.LoadingCountAdd();

        RequestStruct.RecipientInformation_struct _recipientInfo = new RequestStruct.RecipientInformation_struct
        {
            name = recipientData.name,
            phone = recipientData.phone,
            postalCode = recipientData.postalCode,
            countyCity = recipientData.countyCity,
            address = recipientData.address,
        };

        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetMutationByName.placeOrder, GraphApi.Query.Type.Mutation);
        _graphQuery.SetArgs(new { commodity = commodities_list.ToArray(), recipientInfo = _recipientInfo });
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, CheckoutCallback));

        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
    /// <summary>
    /// 再次確認視窗內點擊"取消"
    /// </summary>
    private void CheckoutEventCancel()
    {
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
    /// <summary>
    /// 結帳請求的回傳
    /// <para>編輯者: Dimos</para>
    /// <para>請求成功後會刷新使用者資料、消除購物車內已結帳的商品、關閉此畫面、關閉購物車畫面</para>
    /// </summary>
    public void CheckoutCallback(string _responseData_str)
    {
        if (!string.IsNullOrEmpty(_responseData_str))
        {
            try
            {
                var _response = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.commodityInfoResponse_struct>>(_responseData_str);

                if (_response.errors == null)
                {
                    MemberInformationManager.Instance.RequestMemberInformation();
                    for (int i = 0; i < commodities_list.Count; i++)
                    {
                        ShoppingCart.Instance.RemoveCommodityData(commodities_list[i].commodityId);
                    }
                    this.Quit();
                    MallManager.Instance.RefreshCommodity();
                    ShoppingCart.Instance.QuitView();
                    NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.mall_orderSuccess);
                }
                else
                {
                    switch (_response.errors[0].extensions.code)
                    {
                        default:
                            GraphQLManager.ErrorCodeHandle(_response.errors[0]);
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
    /// 關閉結帳畫面
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Quit()
    {
        Destroy(this.gameObject);
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
}
