using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// firebaseMessage中好友邀請的處理
/// </summary>
public class FriendInviteFirebaseHandler : MonoBehaviour
{
    //====================================================================
    static FriendInviteFirebaseHandler instance = null;

    public static FriendInviteFirebaseHandler Instance
    {
        get
        {
            return instance;
        }
    }


    //====================================================================
    public System.Action OnFriendInvite;

    //====================================================================
    void Awake()
    {
        instance = this;
    }
    //====================================================================
    void Start()
    {
        FirebaseMessageManager.Instance.OnMessageReceivedEvent += OnMessageReceived;
    }

    //====================================================================
    /// <summary>
    /// firebaseMessage中好友邀請的處理
    /// </summary>
    /// <param name="_firebaseMessage"></param>
    void OnMessageReceived(Firebase.Messaging.MessageReceivedEventArgs _firebaseMessage)
    {
        string _typeDate_str = "";
        if (_firebaseMessage.Message.Data.TryGetValue(FirebaseMessageManager.KeyTable.type, out _typeDate_str))
        {
            if (_typeDate_str == FirebaseMessageManager.TypeTable.friendRequest)
            {
                OnFriendInvite?.Invoke();
            }
        }
    }

    //====================================================================
    void OnDestroy()
    {
        FirebaseMessageManager.Instance.OnMessageReceivedEvent -= OnMessageReceived;
    }

    //====================================================================
}
