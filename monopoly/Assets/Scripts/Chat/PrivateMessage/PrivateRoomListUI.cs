using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivateRoomListUI : MonoBehaviour
{
    //====================================================================
    [SerializeField] MoveOut privateRoomList_moveout;
    [SerializeField] Transform privateRoomListContent_transform;
    [SerializeField] GameObject privateRoomListEmptyHint_gameObject;


    //====================================================================
    PrivateMessageHandler privateRoomListHandler;


    //====================================================================
    /// <summary>
    /// 載入好友列表資料
    /// </summary>
    /// <param name="_privateRoomListHandler">好友功能處理的引用</param>
    public void LoadAndShowView(PrivateMessageHandler _privateRoomListHandler)
    {
        Debug.Log("PrivateRoomListUI LoadAndShowView");
        privateRoomListHandler = _privateRoomListHandler;
        privateRoomListHandler.OnListRefresh += RefreshList;

        privateRoomListHandler.GetLastChatMessageList(() =>
        {
            Debug.Log("PrivateRoomListUI GetLastChatMessageList Complete");
        });

        UnityEngine.Events.UnityEvent _returnEvent = new UnityEngine.Events.UnityEvent();
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
            privateRoomList_moveout.Move(true);
        });

    }
    //====================================================================
    /// <summary>
    /// 生成好友列表
    /// </summary>
    void BuildFriendList()
    {
        privateRoomListHandler.BuildPrivateRoom(privateRoomListContent_transform, privateRoomListEmptyHint_gameObject);
    }

    //====================================================================
    public void AddPrivateRoom()
    {
        privateRoomListHandler.OpenFriendListView();
    }
    //====================================================================
    /// <summary>
    /// 顯示此UI
    /// </summary>
    public void ShowView()
    {
        privateRoomList_moveout.Move(true);

    }

    //=================================================================
    /// <summary>
    /// 關閉此UI
    /// </summary>
    public void QuitView()
    {
        privateRoomListHandler.OnListRefresh -= RefreshList;
        ReturnManager.Instance.PopReturnEvent();
        privateRoomList_moveout.Move(false);

    }

    //====================================================================
    /// <summary>
    /// 檢查好友邀請以顯示提示點
    /// </summary>
    void CheckInviteHint()
    {

    }

    //====================================================================
}
