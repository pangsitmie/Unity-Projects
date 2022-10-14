using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 返回鍵管理器
/// <para>操作方式: </para>
/// <para>此為管理返回鍵功能的腳本 </para>
/// <para>按下返回鍵會觸發當前stack裡peek出來的事件 </para>
/// <para>而該事件的函式內必須加上"ReturnManager.Instance.PopReturnEvent();"才能pop掉該事件 </para>
/// <para>可與ReturnEventHandler一同使用 方便設計 </para>
/// <para>可同時有多個存在 但僅只有最新的一套能觸發功能 </para>
/// <para>當最新的被銷毀後 就會設定第二新的為instance </para>
/// </summary>
public class ReturnManager : MonoBehaviour
{
    private static ReturnManager instance = null; //當前場景使用的返回功能
    public static ReturnManager Instance { get => instance; }

    private Stack<UnityEvent> returnEvent_stack = new Stack<UnityEvent>(); //返回功能的所要觸發之事件的stack
    private static Stack<ReturnManager> instance_stack = new Stack<ReturnManager>(); //複數場景存在時將前面的對象儲存起來
    public bool returnEnable_bl = true;
    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 開出新的ReturnManager時會將上一個ReturnManager儲存起來並將instance設置為當前的</para>
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("ReturnManager.Awake() instance now is: " + instance.gameObject.name);
        }
        else
        {
            instance_stack.Push(instance);
            Debug.Log("ReturnManager.Awake() Push: " + instance.gameObject.name);

            instance = this;
            Debug.Log("ReturnManager.Awake() instance now is: " + instance.gameObject.name);
        }
        returnEvent_stack.Clear();
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 當當前的ReturnManager被銷毀時 將instance設為上一個ReturnManager</para>
    /// </summary>
    private void OnDestroy()
    {
        if (instance == this)
        {
            if (instance_stack.Count > 0)
            {
                instance = instance_stack.Pop();
                Debug.Log("ReturnManager.OnDestroy() Pop: " + instance.gameObject.name);
                Debug.Log("ReturnManager.OnDestroy() instance now is: " + instance.gameObject.name);
            }
            else
            {
                instance = null;
                Debug.Log("ReturnManager.OnDestroy() instance_stack is empty");
                Debug.Log("ReturnManager.OnDestroy() instance now is: null");
            }
        }
        else
        {
            Debug.Log("ReturnManager.OnDestroy() is going to Drop: " + instance_stack.Peek().gameObject.name);
            instance_stack.Pop();
            Debug.Log("ReturnManager.OnDestroy() instance now is: " + instance.gameObject.name);
        }
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 按下返回鍵觸發返回事件(僅觸發當前instance)</para>
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && returnEnable_bl)
        {
            if (instance == this)
            {
                if (returnEvent_stack.Count > 0)
                {
                    Debug.Log("ReturnManager.Update() Return button click");
                    returnEvent_stack.Peek().Invoke();
                }
                else
                {
                    Debug.LogWarning("ReturnManager.Update() Return button click but Return-Event-Stack is empty");
                }
            }
        }
    }

    //====================================================================
    /// <summary>
    /// 添加返回事件
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 將下一個按返回鍵要觸發的事件放入Stack</para>
    /// </summary>
    public void PushReturnEvent(UnityEvent _event)
    {
        returnEvent_stack.Push(_event);
    }

    //====================================================================
    /// <summary>
    /// 去除返回事件
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 在需要觸發的返回事件裡調用，其代表該事件已完成</para>
    /// </summary>
    public void PopReturnEvent()
    {
        returnEvent_stack.Pop();
    }

    //====================================================================
}
