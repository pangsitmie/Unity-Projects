using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FriendInviteObject : MonoBehaviour
{
    //====================================================================
    [SerializeField] Text memberName_text;


    //====================================================================
    ResponseStruct.OtherMemberInfo_struct memberInfo;
    public ResponseStruct.OtherMemberInfo_struct MemberInfo { get => memberInfo; }


    //====================================================================
    FriendListHandler friendListHandler;


    //====================================================================
    /// <summary>
    /// 設定好友資料
    /// </summary>
    /// <param name="_memberInfo">欲顯示的會員資料</param>
    /// <param name="_friendListHandler">好友功能處理的引用</param>
    public void Set(ResponseStruct.OtherMemberInfo_struct _memberInfo, FriendListHandler _friendListHandler)
    {
        memberInfo = _memberInfo;
        memberName_text.text = memberInfo.nickname;
        friendListHandler = _friendListHandler;
    }

    //====================================================================
    /// <summary>
    /// 接受好友邀請
    /// </summary>
    public void Accept()
    {
        friendListHandler.AcceptFriendInvite(memberInfo.id, (_result) =>
        {

        });
    }

    //====================================================================
    /// <summary>
    /// 拒絕好友邀請
    /// </summary>
    public void Refuse()
    {
        friendListHandler.RefuseFriendInvite(memberInfo.id, (_result) =>
        {

        });
    }

    //====================================================================
}
