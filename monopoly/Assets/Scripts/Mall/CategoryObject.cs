using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 類別物件腳本
/// <para>類別物件的互動處理</para>
/// </summary>
public class CategoryObject : MonoBehaviour
{
    [SerializeField] Material on_material, off_material;

    [SerializeField] Text categoryName_tx;
    [SerializeField] GameObject categoryUnderLine_gObj;
    [SerializeField] ResponseStruct.CategoryDataResponse_struct categoryData;
    [SerializeField] Toggle this_toggle;
    [SerializeField] System.Action<int> FilterByCategory;

    //====================================================================
    /// <summary>
    /// 設定類別資料
    /// <para>編輯者: Dimos</para>
    /// <para>更新類別資料、更新類別名稱、加上toggleGroup、添加商品的類別篩選事件</para>
    /// </summary>
    public void Set(ResponseStruct.CategoryDataResponse_struct _categoryData, ToggleGroup _category_toggleGroup, System.Action<int> _FilterByCategory)
    {
        categoryData = _categoryData;
        this.name = _categoryData.id.ToString();

        if (!string.IsNullOrEmpty(_categoryData.name))
            categoryName_tx.text = "  " + _categoryData.name + "  ";
        else if (_categoryData.id != -1) //id為-1代表是預設的"全部"選項
            categoryName_tx.text = "  Unknow  ";

        this_toggle.group = _category_toggleGroup;
        FilterByCategory = _FilterByCategory;
    }

    //====================================================================
    /// <summary>
    /// 選擇類別時的處理
    /// <para>編輯者: Dimos</para>
    /// <para>傳入的Toggle.isOn值為True時顯示下底線、文字改為主色、觸發商品的類別篩選</para>
    /// <para>傳入的Toggle.isOn值為False時隱藏下底線、文字改為文字主色</para>
    /// </summary>
    public void ValueChange(bool _isSelect_bl)
    {
        if (_isSelect_bl)
        {
            categoryName_tx.material = on_material;
            categoryUnderLine_gObj.SetActive(true);
            FilterByCategory?.Invoke(categoryData.id);
        }
        else
        {
            categoryName_tx.material = off_material;
            categoryUnderLine_gObj.SetActive(false);
        }
    }

    //====================================================================
}
