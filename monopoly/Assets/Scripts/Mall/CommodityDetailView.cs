using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CommodityDetailView : MonoBehaviour
{
    [SerializeField] Text commodityName_tx;
    [SerializeField] Text commodityDiscription_tx;
    [SerializeField] Text commodityQuantity_tx;
    [SerializeField] Text commodityPrice_tx;
    [SerializeField] Text selectQuantity_tx;

    [SerializeField] ResponseStruct.CommodityDataResponse_struct commodityData;
    [SerializeField] int selectQuantity_int;
    [SerializeField] Button select_button;

    [SerializeField] MoveOut this_moveOut;

    //====================================================================
    /// <summary>
    /// 載入商品資料
    /// <para>編輯者: Dimos</para>
    /// <para>更新商品資料、初始selectAmount設為1</para>
    /// </summary>
    public void Set(ResponseStruct.CommodityDataResponse_struct _commodityData)
    {
        selectQuantity_int = 1;
        selectQuantity_tx.text = "1";

        commodityData = _commodityData;
        commodityName_tx.text = _commodityData.name;
        commodityDiscription_tx.text = _commodityData.description;
        commodityQuantity_tx.text = _commodityData.quantity.ToString();
        commodityPrice_tx.text = _commodityData.price.ToString();

        select_button.interactable = (_commodityData.quantity > 0);

        UnityEngine.Events.UnityEvent _QuitViewReturnEvent = new UnityEngine.Events.UnityEvent();
        _QuitViewReturnEvent.AddListener(QuitView);
        ReturnManager.Instance.PushReturnEvent(_QuitViewReturnEvent);

        this_moveOut.Move(true);
    }

    //====================================================================
    /// <summary>
    /// 增加要兌換的商品數量
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void AddSelectAmount()
    {
        if (commodityData.quantity > selectQuantity_int)
        {
            selectQuantity_int++;
        }

        selectQuantity_tx.text = selectQuantity_int.ToString();
    }

    //====================================================================
    /// <summary>
    /// 減少要兌換的商品數量
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void SubSeletAmount()
    {
        if (selectQuantity_int > 1)
        {
            selectQuantity_int--;
        }

        selectQuantity_tx.text = selectQuantity_int.ToString();
    }

    //====================================================================
    /// <summary>
    /// 選擇指定數量加入購物車
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Select()
    {
        ShoppingCart.Instance.AddCommodityData(commodityData, selectQuantity_int);
        QuitView();
    }

    //====================================================================
    /// <summary>
    /// 關閉商品詳細視窗
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void QuitView()
    {
        this_moveOut.Move(false);
        selectQuantity_int = 1;
        selectQuantity_tx.text = "1";
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
}
