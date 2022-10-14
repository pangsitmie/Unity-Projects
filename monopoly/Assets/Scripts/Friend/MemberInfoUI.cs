using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MemberInfoUI : MonoBehaviour
{
    //====================================================================
    [SerializeField] Text memberName_text;
    [SerializeField] MoveOut memberInfoUI_moveout;
    FriendListHandler friendListHandler;


    //====================================================================
    ResponseStruct.OtherMemberInfo_struct memberInfo;
    public ResponseStruct.OtherMemberInfo_struct MemberInfo { get => memberInfo; }


    //====================================================================
    /// <summary>
    /// 載入會員資料
    /// </summary>
    /// <param name="_friendObject">欲顯示的好友物件</param>
    /// <param name="_friendListHandler">好友功能處理的引用</param>
    public void LoadAndShowView(FriendObject _friendObject, FriendListHandler _friendListHandler)
    {
        memberInfo = _friendObject.MemberInfo;
        memberName_text.text = memberInfo.nickname;
        friendListHandler = _friendListHandler;
        memberInfoUI_moveout.Move(true);
        var _returnEvent = new UnityEngine.Events.UnityEvent();
        _returnEvent.AddListener(QuitView);
        ReturnManager.Instance.PushReturnEvent(_returnEvent);
    }

    //=================================================================
    /// <summary>
    /// 開啟私聊
    /// </summary>
    public void OpenPrivateChatMessageView()
    {
        friendListHandler.OpenPrivateChatMessageView(this.memberInfo);
        QuitView();
    }
    //====================================================================
    /// <summary>
    /// 移除該好友
    /// </summary>
    public void RemoveFriend()
    {
        friendListHandler.RemoveFriend(memberInfo.id, (_result) =>
        {
            if (_result)
            {
                QuitView();
            }
        });
    }

    //====================================================================
    /// <summary>
    /// 關閉此畫面
    /// </summary>
    public void QuitView()
    {
        ReturnManager.Instance.PopReturnEvent();
        memberInfoUI_moveout.Move(false);
    }

    //====================================================================
}
