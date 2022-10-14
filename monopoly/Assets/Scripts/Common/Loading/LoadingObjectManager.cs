using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 多載入中動畫的管理器
/// <para>操作方式: 搭配LoadingObjectByActive使用</para>
/// </summary>
public class LoadingObjectManager : MonoBehaviour
{
    [SerializeField] int loadingCount = 0;
    [SerializeField] GameObject loadingObject; //須掛上LoadingObjectByActive

    //====================================================================
    /// <summary>
    /// 添加載入需求
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 載入需求數量+1</para>
    /// </summary>
    public void LoadingCountAdd()
    {
        loadingCount++;
        CheckLoadingCount();
    }

    //====================================================================
    /// <summary>
    /// 結束載入需求
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 載入需求數量-1</para>
    /// </summary>
    public void LoadingCountSub()
    {
        if (loadingCount > 0)
            loadingCount--;
        CheckLoadingCount();
    }

    //====================================================================
    /// <summary>
    /// 結束載入需求
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 載入需求數量-1</para>
    /// </summary>
    public void LoadingCountReset()
    {
        loadingCount = 0;
        CheckLoadingCount();
    }
    //====================================================================
    /// <summary>
    /// 檢查載入中數量
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 當載入需求數量<=0時關閉載入動畫</para>
    /// <para>載入中禁用返回功能</para>
    /// </summary>
    ReturnManager returnManagerInstance;
    public void CheckLoadingCount()
    {
        if (loadingCount > 0)
        {
            loadingObject.SetActive(true);
            if (returnManagerInstance != null)
            {
                returnManagerInstance.returnEnable_bl = true;
            }
            returnManagerInstance = ReturnManager.Instance;
            returnManagerInstance.returnEnable_bl = false;
            loadingObject.transform.SetAsLastSibling();
        }
        else
        {
            ReturnManager.Instance.returnEnable_bl = true;
            loadingObject.SetActive(false);
        }
    }

    //====================================================================
}
