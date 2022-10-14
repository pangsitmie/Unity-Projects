using System.Collections;
using System.Collections.Generic;
using ResponseStruct;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 商品物件腳本
/// <para>商品列表所使用的物件</para>
/// </summary>
public class CommodityObject : MonoBehaviour
{
    [SerializeField] CommodityDataResponse_struct commodityData;
    public CommodityDataResponse_struct CommodityData { get => commodityData; }

    [SerializeField] CommodityDetailView commodityDetailView_pf;
    private static CommodityDetailView CommodityDetailView;

    [SerializeField] Text commodityName_tx;
    [SerializeField] Text commodityQuantity_tx;
    [SerializeField] Text commodityPrice_tx;

    //====================================================================
    /// <summary>
    /// 載入商品資料
    /// <para>編輯者: Dimos</para>
    /// <para>更新商品資料</para>
    /// </summary>
    public void Set(CommodityDataResponse_struct _commodityData)
    {
        commodityData = _commodityData;
        commodityName_tx.text = _commodityData.name;
        commodityQuantity_tx.text = _commodityData.quantity.ToString();
        commodityPrice_tx.text = _commodityData.price.ToString();
    }

    //====================================================================
    /// <summary>
    /// 開啟顯示商品詳細視窗
    /// <para>編輯者: Dimos</para>
    /// <para>當未創建過商品視窗時才會Instantiate</para>
    /// <para>當創建過商品視窗後，僅執行載入商品資料並顯示</para>
    /// </summary>
    public void OpenDetailView()
    {
        if (CommodityDetailView == null)
        {
            CommodityDetailView = Instantiate(commodityDetailView_pf, SceneController.Instance.CurrentSceneCanvas.transform);
            CommodityDetailView.Set(commodityData);
        }
        else
        {
            CommodityDetailView.Set(commodityData);
        }
    }
    //====================================================================
}
