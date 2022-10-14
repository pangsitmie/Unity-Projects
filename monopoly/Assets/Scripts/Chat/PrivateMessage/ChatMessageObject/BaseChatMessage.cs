using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class BaseChatMessage : MonoBehaviour
{
    public ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct chatData;
    public void Set(ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct _chatData)
    {
        chatData = _chatData;
        UpdateDisplay();
    }
    public abstract void UpdateDisplay();
}
