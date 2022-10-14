using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/// <summary>
/// 返回鍵事件處理
/// <para>操作方式: </para>
/// <para>在需要新增返回事件的地方掛上此腳本</para>
/// <para>可設定在Start()、OnEnable()時添加返回事件，也可以自行觸發來添加一串事件或直接將一button上的onClick添加進返回事件</para>
/// <para>通常事件內必須自行加上"PopReturnEvent()"才能結束掉該返回事件 </para>
/// <para>使用enableEvent、startEvent時必須將對應的boolean打勾</para>
/// </summary>
public class ReturnEventHandler : MonoBehaviour
{
    [SerializeField] bool hasStartEvent = false;
    [SerializeField] UnityEvent startEvent;

    [SerializeField] bool hasEnableEvent = false;
    [SerializeField] UnityEvent enableEvent;

    [SerializeField] UnityEvent functionEvent;
    [SerializeField] Button buttonClickEvent_btn;
    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: Start()時將startEvent添加進返回事件列表</para>
    /// </summary>
    private void Start()
    {
        if (hasStartEvent && startEvent != null)
        {
            ReturnManager.Instance.PushReturnEvent(startEvent);
            Debug.Log("ReturnEventHandle.Start(): Push startEvent");
        }
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: OnEnable()時將enableEvent添加進返回事件列表</para>
    /// </summary>
    private void OnEnable()
    {
        if (hasEnableEvent && enableEvent != null)
        {
            ReturnManager.Instance.PushReturnEvent(enableEvent);
            Debug.Log("ReturnEventHandle.OnEnable(): Push enableEvent");
        }
    }

    //====================================================================
    /// <summary>
    /// 添加返回事件
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 將functionEvent添加進返回事件列表(在Button的onClick之類的地方使用)</para>
    /// </summary>
    public void AddFunctionEvent()
    {
        ReturnManager.Instance.PushReturnEvent(functionEvent);
    }

    //====================================================================
    /// <summary>
    /// 添加Button的onClick進返回事件
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 將returnEvent_btn.onClick添加進返回事件列表(在Button的onClick之類的地方使用)</para>
    /// </summary>
    public void AddButtonClickEvent()
    {
        ReturnManager.Instance.PushReturnEvent(buttonClickEvent_btn.onClick);
    }

    //====================================================================
    /// <summary>
    /// Pop返回事件
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 添加在返回事件結束時，以保證返回事件結束</para>
    /// </summary>
    public void PopReturnEvent()
    {
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
    /// <summary>
    /// 添加空事件進返回事件
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 能用來放入PushReturnEvent使接下來暫時無法使用返回建，用在不能返回的情景上</para>
    /// </summary>
    public void PushNullEvent()
    {
        ReturnManager.Instance.PushReturnEvent(new UnityEvent());
    }
    //====================================================================
}
