using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivateMessageFirebaseHandler : MonoBehaviour
{
    //=================================================================
    #region 參數
    //=================================================================
    static PrivateMessageFirebaseHandler instance = null;

    public static PrivateMessageFirebaseHandler Instance
    {
        get
        {
            return instance;
        }
    }

    public System.Action OnPrivateMessageReceived;

    //=================================================================
    #endregion
    //=================================================================
    #region Unity Event
    //=================================================================
    void Awake()
    {
        instance = this;
    }

    //=================================================================
    void Start()
    {
        FirebaseMessageManager.Instance.OnMessageReceivedEvent += OnMessageReceived;
    }

    //=================================================================
    void OnDestroy()
    {
        FirebaseMessageManager.Instance.OnMessageReceivedEvent -= OnMessageReceived;
    }

    //=================================================================
    #endregion
    //=================================================================
    #region 事件處理
    //=================================================================
    /// <summary>
    /// firebaseMessage中私聊通知的處理
    /// </summary>
    /// <param name="_firebaseMessage"></param>
    void OnMessageReceived(Firebase.Messaging.MessageReceivedEventArgs _firebaseMessage)
    {
        string _typeDate_str = "";
        //-------------------------------------------------------------
        //檢查Data是否有type
        if (!_firebaseMessage.Message.Data.TryGetValue(FirebaseMessageManager.KeyTable.type, out _typeDate_str))
        {
            Debug.LogError("firebase Data中未找尋到type");
            return;
        }
        //-------------------------------------------------------------
        //檢查type是否為私聊通知
        if (_typeDate_str == FirebaseMessageManager.TypeTable.privateMessage)
        {
            OnPrivateMessageReceived?.Invoke();
        }
        //-------------------------------------------------------------
    }

    //=================================================================
    #endregion
    //=================================================================
}
