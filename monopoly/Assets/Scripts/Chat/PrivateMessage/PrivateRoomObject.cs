using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrivateRoomObject : MonoBehaviour
{
    //=================================================================
    [SerializeField] Text targetName_text;
    [SerializeField] StripStringWithSuffix lastMessage_stringSuffix;
    [SerializeField] GameObject privateMsgHint_gObj;
    //=================================================================
    ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct privateRoomData;
    ResponseStruct.OtherMemberInfo_struct targetMemberInfo;

    //=================================================================
    PrivateMessageHandler privateMessageHandler;

    //=================================================================
    public void Set(ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct _privateRoomData, PrivateMessageHandler _privateMessageHandler)
    {
        privateRoomData = _privateRoomData;
        privateMessageHandler = _privateMessageHandler;

        // 判斷對方會員資料
        if (_privateRoomData.target.id == MemberInformationManager.Id)
        {
            targetMemberInfo = _privateRoomData.initiator;
        }
        else
        {
            targetMemberInfo = _privateRoomData.target;
        }
        targetName_text.text = targetMemberInfo.nickname;
        lastMessage_stringSuffix.StripString(_privateRoomData.message);
        CheckPrivateMsgHint();
    }
    //=================================================================
    /// <summary>
    /// 檢查是否要顯示私聊提示
    /// </summary>
    public void CheckPrivateMsgHint()
    {
        // 取得已讀資料，false顯示提示
        privateMsgHint_gObj.SetActive(!privateMessageHandler.CheckSingleHaveRead(targetMemberInfo.id, privateRoomData.id));
    }

    //=================================================================
    public void OpenPrivateRoom()
    {
        privateMessageHandler.OpenPrivateChatMessageView(targetMemberInfo);
    }

    //=================================================================
}
