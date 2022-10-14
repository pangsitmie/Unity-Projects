using System.Collections;
using System.Collections.Generic;
using ResponseStruct;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 購物車內的商品物件，與一般商品列表內的商品物件有所差異
/// </summary>
public class ShoppingCartCommodityObject : MonoBehaviour
{
    [SerializeField] CommodityDataResponse_struct commodityData;
    public CommodityDataResponse_struct CommodityData { get => commodityData; }

    [SerializeField] StripStringWithSuffix commodityName_tx;
    [SerializeField] int selectQuantity_int;
    [SerializeField] Text selectQuantity_tx;
    [SerializeField] Text commodityPrice_tx;
    [SerializeField] Text currentQuantity_tx;

    public ClickToggle this_clickToggle;

    //====================================================================
    /// <summary>
    /// 設定商品資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Set(CommodityDataResponse_struct _commodityData, int _selectQuantity_int)
    {
        commodityData = _commodityData;

        commodityName_tx.StripString(_commodityData.name);
        selectQuantity_int = _selectQuantity_int;
        selectQuantity_tx.text = _selectQuantity_int.ToString();
        currentQuantity_tx.text = _commodityData.quantity.ToString();
        commodityPrice_tx.text = _commodityData.price.ToString();
    }

    //====================================================================
    /// <summary>
    /// 設定要兌換的商品數量
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void SetSelectQuantity(int _selectQuantity_int)
    {
        selectQuantity_int = _selectQuantity_int;
        selectQuantity_tx.text = selectQuantity_int.ToString();
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

        ShoppingCart.Instance.CommodityQuantityChange(commodityData.id, selectQuantity_int);
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
        else
        {
            ShoppingCart.Instance.RemoveCommodityData(commodityData.id);
        }
        selectQuantity_tx.text = selectQuantity_int.ToString();

        ShoppingCart.Instance.CommodityQuantityChange(commodityData.id, selectQuantity_int);
    }

    //====================================================================
    /// <summary>
    /// 從購物車的等待結帳列表添加或移除
    /// <para>編輯者: Dimos</para>
    /// <para>位於 => Prefab/Mall/ProductInShoppingCartObject: ProductInShoppingCartObject/Toggle</para>
    /// </summary>
    public void Select(bool _status_bl)
    {
        if (_status_bl)
        {
            ShoppingCart.Instance.AddCommodityToWaitList(commodityData.id);
        }
        else
        {
            ShoppingCart.Instance.RemoveCommodityToWaitList(commodityData.id);
        }
    }

    //====================================================================
}
