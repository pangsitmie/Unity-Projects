using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 結帳畫面的商品物件(此物件的數量設定為預計要下訂的數量)
/// </summary>
public class CheckoutCommodityObject : MonoBehaviour
{
    [SerializeField] Text commodityName_tx;
    [SerializeField] Text commodityQuantity_tx;
    [SerializeField] Text commodityPrice_tx;

    //====================================================================
    /// <summary>
    /// 設定商品資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Set(ResponseStruct.CommodityDataResponse_struct _commodityData, int _amount_int)
    {
        commodityName_tx.text = _commodityData.name;
        commodityQuantity_tx.text = "x" + _amount_int.ToString();
        commodityPrice_tx.text = _commodityData.price.ToString();
    }
    //====================================================================
    /// <summary>
    /// 設定商品資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Set(ResponseStruct.IOrderCommodityInfo_struct _commodityData, int _amount_int)
    {
        commodityName_tx.text = _commodityData.name;
        commodityQuantity_tx.text = "x" + _amount_int.ToString();
        commodityPrice_tx.text = _commodityData.unitPrice.ToString();
    }
    //====================================================================
}
