using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
/// <summary>
/// 用於ScrollView的下拉刷新
/// </summary>
public class DragDownRefresh : MonoBehaviour, IEndDragHandler
{
    [SerializeField] UnityEvent OnRefreshEvent;

    [SerializeField] RectTransform baseObject_RectTransform; //判斷刷新是否觸發的基準物件(目前大多使用ScrollView本身)
    [SerializeField] RectTransform target_RectTransform; //讀取圖示(預設狀態需高於ScrollView)

    private float triggerPositionY_f; //當'target的Y'低於此高度便會觸發刷新
    private float targetHalfHeight_f; //用以計算讀取圖示當前頂端的Y

    //====================================================================
    private void Start()
    {
        triggerPositionY_f = baseObject_RectTransform.position.y + baseObject_RectTransform.rect.height * 0.5f;
        //Debug.Log(triggerPositionY_f);

        targetHalfHeight_f = target_RectTransform.rect.height * 0.5f;
        //Debug.Log(targetHalfHeight_f);
    }

    //====================================================================
    /// <summary>
    /// 下拉時使讀取圖示旋轉
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void RotateTarget()
    {
        target_RectTransform.localRotation = Quaternion.Euler(0, 0, (target_RectTransform.position.y) * 2.25f);
    }

    //====================================================================
    /// <summary>
    /// 下拉結束時當讀取圖示低於ScrollView時觸發OnRefreshEvent
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void OnEndDrag(PointerEventData data)
    {
        //Debug.Log("DragDownRefresh.OnEndDrag() " + (currentTarget_tsF.position.y + targetHalfHeight_f) + " " + triggerPositionY_f);
        if (target_RectTransform.position.y + targetHalfHeight_f < triggerPositionY_f)
        {
            Debug.Log("DragDownRefresh.OnEndDrag() OnRefreshEvent Invoke");
            if (OnRefreshEvent == null)
            {
                Debug.Log("null");
            }
            OnRefreshEvent?.Invoke();
        }
    }

    //====================================================================
}

