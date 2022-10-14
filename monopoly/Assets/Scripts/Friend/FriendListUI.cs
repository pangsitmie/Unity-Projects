using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FriendListUI : MonoBehaviour
{
    //====================================================================
    [SerializeField] MoveOut friendList_moveout;
    [SerializeField] Transform firendListContent_transform;
    [SerializeField] GameObject friendListEmptyHint_gameObject;
    [SerializeField] Transform firendInviteContent_transform;
    [SerializeField] GameObject friendInviteEmptyHint_gameObject;
    [SerializeField] GameObject inviteHint_gameObject;
    [SerializeField] MoveOut friendListScrollView_moveOut;
    [SerializeField] MoveOut friendInviteScrollView_moveOut;
    [SerializeField] Toggle friendList_toggle;
    [SerializeField] Toggle friendInvite_toggle;


    //====================================================================
    FriendListHandler friendListHandler;


    //====================================================================
    /// <summary>
    /// 載入好友列表資料
    /// </summary>
    /// <param name="_friendListHandler">好友功能處理的引用</param>
    public void LoadAndShowView(FriendListHandler _friendListHandler)
    {
        friendListHandler = _friendListHandler;
        friendListHandler.OnListRefresh += RefreshList;

        if (friendListHandler.FriendInviteInfo_arry != null)
        {
            if (friendListHandler.FriendInviteInfo_arry.Length <= 0)
            {
                friendList_toggle.isOn = true;
            }
            else
            {
                friendInvite_toggle.isOn = true;
            }
        }
        else
        {
            friendList_toggle.isOn = true;
        }


        friendListHandler.GetFriendInviteList(() =>
        {

        });
        friendListHandler.GetFriendList(() =>
        {

        });
        var _returnEvent = new UnityEngine.Events.UnityEvent();
        _returnEvent.AddListener(QuitView);
        ReturnManager.Instance.PushReturnEvent(_returnEvent);
    }

    //====================================================================
    /// <summary>
    /// 刷新列表
    /// </summary>
    void RefreshList()
    {
        Debug.Log("RefreshListUI Display");
        AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
        {
            CheckInviteHint();
            BuildFriendList();
            BuildFriendInviteList();
            friendList_moveout.Move(true);
        });

    }
    //====================================================================
    /// <summary>
    /// 生成好友列表
    /// </summary>
    void BuildFriendList()
    {
        friendListHandler.BuildFriendList(firendListContent_transform, friendListEmptyHint_gameObject);
    }

    //====================================================================
    /// <summary>
    /// 生成好友列表
    /// </summary>
    void BuildFriendInviteList()
    {
        friendListHandler.BuildFriendInviteList(firendInviteContent_transform, friendInviteEmptyHint_gameObject);
    }
    //====================================================================
    /// <summary>
    /// 開啟添加好友用的電話輸入視窗
    /// </summary>
    public void OpenAddFriendInputView()
    {
        friendListHandler.OpenAddFriendInputView(AddFriend);
    }

    //====================================================================
    /// <summary>
    /// 以電話號碼添加好友
    /// </summary>
    /// <param name="_targetPhone"></param>
    public void AddFriend(DataStruct.PhoneFormat_struct _targetPhone)
    {
        if (_targetPhone != null)
        {
            friendListHandler.AddFriendByPhoneNumber(_targetPhone, (_result) =>
            {
                BuildFriendList();
            });
        }
    }
    //====================================================================
    /// <summary>
    /// 關閉此UI
    /// </summary>
    public void QuitView()
    {
        friendListHandler.OnListRefresh -= RefreshList;
        ReturnManager.Instance.PopReturnEvent();
        friendList_moveout.Move(false);

    }

    //====================================================================
    /// <summary>
    /// 顯示好友列表的畫面
    /// </summary>
    public void DisplayFriendList(bool _status_bl)
    {
        if (_status_bl)
        {
            friendListScrollView_moveOut.Move(true);
            friendInviteScrollView_moveOut.Move(false);
        }
    }

    //====================================================================
    /// <summary>
    /// 顯示好友邀請的畫面
    /// </summary>
    public void DisplayFriendInvite(bool _status_bl)
    {
        if (_status_bl)
        {
            friendInviteScrollView_moveOut.Move(true);
            friendListScrollView_moveOut.Move(false);
        }
    }

    //====================================================================
    /// <summary>
    /// 檢查好友邀請以顯示提示點
    /// </summary>
    void CheckInviteHint()
    {
        inviteHint_gameObject.SetActive(friendListHandler.FriendInviteInfo_arry.Length > 0);
    }

    //====================================================================
}
