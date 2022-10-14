using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PrivateChatMessageUI : MonoBehaviour
{
    //=================================================================
    #region 參數
    //=================================================================
    // 訊息的Prefab
    [SerializeField] SelfSideChatMessageObject selfSideChatMessageObject_pf;
    [SerializeField] OtherSideChatMessageObject otherSideChatMessageObject_pf;

    //=================================================================
    // 各式外部匯入
    [SerializeField] Transform messageContent_transform;
    [SerializeField] PrivateChatMessageHandler privateChatMessageHandler;
    [SerializeField] Transform chatMessagePoolContent_transform;
    [SerializeField] MoveOut privateChatMessageUI_moveOut;

    //=================================================================
    // UI物件
    [SerializeField] InputField sendMessage_inputField;
    [SerializeField] Text targetName_text;

    //=================================================================
    // 訊息的物件池
    UnityObjectPool<SelfSideChatMessageObject> selfSideChatMessageObjects_pool;
    UnityObjectPool<OtherSideChatMessageObject> otherSideChatMessageObjects_pool;

    //=================================================================
    // 當前已存在畫面上訊息的stack
    Stack<SelfSideChatMessageObject> oldSelfSideChatMessage_stack = new Stack<SelfSideChatMessageObject>();
    Stack<OtherSideChatMessageObject> oldOtherSideChatMessage_stack = new Stack<OtherSideChatMessageObject>();
    Stack<SelfSideChatMessageObject> newSelfSideChatMessage_stack = new Stack<SelfSideChatMessageObject>();
    Stack<OtherSideChatMessageObject> newOtherSideChatMessage_stack = new Stack<OtherSideChatMessageObject>();

    //=================================================================
    // 各式資料
    ResponseStruct.OtherMemberInfo_struct targetMemberInfo;

    //=================================================================
    #endregion
    //=================================================================
    #region Unity Event
    //=================================================================
    void Start()
    {
        // 物件池的建立
        selfSideChatMessageObjects_pool = new UnityObjectPool<SelfSideChatMessageObject>(chatMessagePoolContent_transform, SelfSideChatMessageInstantiate, 10);
        otherSideChatMessageObjects_pool = new UnityObjectPool<OtherSideChatMessageObject>(chatMessagePoolContent_transform, OtherSideChatMessageInstantiate, 10);
    }

    //=================================================================
    void OnEnable()
    {
        //監聽事件
        privateChatMessageHandler.OnClearChatMessage += ClearChatMessage;
        privateChatMessageHandler.OnAddOldSelfSideChatMessage += AddOldSelfSideChatMessage;
        privateChatMessageHandler.OnAddOldOtherSideChatMessage += AddOldOtherSideChatMessage;
        privateChatMessageHandler.OnAddNewSelfSideChatMessage += AddNewSelfSideChatMessage;
        privateChatMessageHandler.OnAddNewOtherSideChatMessage += AddNewOtherSideChatMessage;
        privateChatMessageHandler.OnShowView += ShowView;
        privateChatMessageHandler.OnQuitView += QuitView;
    }

    //=================================================================
    void OnDisable()
    {
        //解除監聽事件
        privateChatMessageHandler.OnClearChatMessage -= ClearChatMessage;
        privateChatMessageHandler.OnAddOldSelfSideChatMessage -= AddOldSelfSideChatMessage;
        privateChatMessageHandler.OnAddOldOtherSideChatMessage -= AddOldOtherSideChatMessage;
        privateChatMessageHandler.OnAddNewSelfSideChatMessage -= AddNewSelfSideChatMessage;
        privateChatMessageHandler.OnAddNewOtherSideChatMessage -= AddNewOtherSideChatMessage;
        privateChatMessageHandler.OnShowView -= ShowView;
        privateChatMessageHandler.OnQuitView -= QuitView;
    }

    //=================================================================
    void OnDestroy()
    {
    }

    //=================================================================
    #endregion
    //=================================================================
    #region 物件的生成、實例化
    //=================================================================
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_content_transform"></param>
    /// <returns></returns>
    SelfSideChatMessageObject SelfSideChatMessageInstantiate(Transform _content_transform)
    {
        var _target = Instantiate(selfSideChatMessageObject_pf, _content_transform);
        return _target;
    }

    //=================================================================
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_content_transform"></param>
    /// <returns></returns>
    OtherSideChatMessageObject OtherSideChatMessageInstantiate(Transform _content_transform)
    {
        var _target = Instantiate(otherSideChatMessageObject_pf, _content_transform);
        return _target;
    }

    //=================================================================
    #endregion
    //=================================================================
    #region 操作相關
    //=================================================================
    /// <summary>
    /// 顯示畫面
    /// </summary>
    public void ShowView(ResponseStruct.OtherMemberInfo_struct _targetMemberInfo)
    {
        AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
        {
            targetName_text.text = _targetMemberInfo.nickname;
        });

        privateChatMessageUI_moveOut.Move(true);
        UnityEngine.Events.UnityEvent _returnEvent = new UnityEngine.Events.UnityEvent();
        _returnEvent.AddListener(QuitView);
        ReturnManager.Instance.PushReturnEvent(_returnEvent);
    }

    //=================================================================
    /// <summary>
    /// 刷新最後一次的訊息
    /// </summary>
    public void RefreshLastChatList()
    {
        privateChatMessageHandler.RefreshLastChatList();
    }

    //=================================================================
    /// <summary>
    /// 關閉畫面
    /// </summary>
    public void QuitView()
    {
        RefreshLastChatList();
        privateChatMessageHandler.QuitView();
        ReturnManager.Instance.PopReturnEvent();
        privateChatMessageUI_moveOut.Move(false);
    }

    //=================================================================
    /// <summary>
    /// 回收所有訊息
    /// </summary>
    public void ClearChatMessage()
    {
        SelfSideChatMessageObject _selfSideTarget;
        lock (oldSelfSideChatMessage_stack)
        {
            while (oldSelfSideChatMessage_stack.TryPop(out _selfSideTarget))
            {
                selfSideChatMessageObjects_pool.BackToPool(_selfSideTarget);
            }
        }
        lock (newSelfSideChatMessage_stack)
        {
            while (newSelfSideChatMessage_stack.TryPop(out _selfSideTarget))
            {
                selfSideChatMessageObjects_pool.BackToPool(_selfSideTarget);
            }
        }
        OtherSideChatMessageObject _otherSideTarget;
        lock (oldOtherSideChatMessage_stack)
        {

            while (oldOtherSideChatMessage_stack.TryPop(out _otherSideTarget))
            {
                otherSideChatMessageObjects_pool.BackToPool(_otherSideTarget);
            }
        }
        lock (newOtherSideChatMessage_stack)
        {

            while (newOtherSideChatMessage_stack.TryPop(out _otherSideTarget))
            {
                otherSideChatMessageObjects_pool.BackToPool(_otherSideTarget);
            }
        }
    }

    //=================================================================
    /// <summary>
    /// 於最底下新增己方的訊息
    /// </summary>
    /// <param name="_chatData">私聊資料</param>
    public void AddNewSelfSideChatMessage(ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct _chatData)
    {
        var _selfMessage = selfSideChatMessageObjects_pool.GetPooledInstance(messageContent_transform);
        _selfMessage.transform.SetAsLastSibling();
        _selfMessage.Set(_chatData);
        newSelfSideChatMessage_stack.Push(_selfMessage);
    }

    //=================================================================
    /// <summary>
    /// 於最上方新增己方的訊息
    /// </summary>
    /// <param name="_chatData">私聊訊息</param>
    public void AddOldSelfSideChatMessage(ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct _chatData)
    {
        var _selfMessage = selfSideChatMessageObjects_pool.GetPooledInstance(messageContent_transform);
        _selfMessage.transform.SetAsFirstSibling();
        _selfMessage.Set(_chatData);
        oldSelfSideChatMessage_stack.Push(_selfMessage);
    }

    //=================================================================
    /// <summary>
    /// 於最底下新增對方的訊息
    /// </summary>
    /// <param name="_chatData">私聊訊息</param>
    public void AddNewOtherSideChatMessage(ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct _chatData)
    {
        var _otherMessage = otherSideChatMessageObjects_pool.GetPooledInstance(messageContent_transform);
        _otherMessage.transform.SetAsLastSibling();
        _otherMessage.Set(_chatData);
        newOtherSideChatMessage_stack.Push(_otherMessage);
    }

    //=================================================================
    /// <summary>
    /// 於最上方新增對方的訊息
    /// </summary>
    /// <param name="_chatData">私聊訊息</param>
    public void AddOldOtherSideChatMessage(ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct _chatData)
    {
        var _otherMessage = otherSideChatMessageObjects_pool.GetPooledInstance(messageContent_transform);
        _otherMessage.transform.SetAsFirstSibling();
        _otherMessage.Set(_chatData);
        oldOtherSideChatMessage_stack.Push(_otherMessage);
    }

    //=================================================================
    /// <summary>
    /// 發送訊息
    /// </summary>
    public void SendChatMessage()
    {
        string _message_str = sendMessage_inputField.text;
        privateChatMessageHandler.SendPrivateMessage(_message_str);
        sendMessage_inputField.text = "";
    }

    //=================================================================
    #endregion
    //=================================================================
}
