using System;
using System.Collections;
using System.Collections.Generic;
using ResponseStruct.Chat.PrivateChat;
using UnityEngine;

public class PrivateMessageHandler : MonoBehaviour
{
    //=================================================================
    #region 參數
    //=================================================================
    // 訊息的Prefab
    [SerializeField] PrivateRoomObject privateRoomObject_pf;
    [SerializeField] PrivateRoomListUI privateRoomListUI_pf;
    [SerializeField] PrivateChatMessageHandler privateChatMessageHandler_pf;

    //====================================================================
    // 各式外部匯入
    [SerializeField] LoadingObjectManager loadingObjectManager;
    [SerializeField] FriendListHandler friendListHandler;
    [SerializeField] GameObject privateMsgHint_gObj;

    //=================================================================
    //生成物件的暫存
    PrivateRoomListUI privateRoomListUI;
    PrivateChatMessageHandler privateChatMessageHandler;
    List<PrivateRoomObject> privateRoomObjects_list = new List<PrivateRoomObject>();

    //=================================================================
    // 各式資料
    public ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct[] lastChatMessage_arry { get; private set; }
    int requestCount_int;
    public int RequestCount_int
    {
        get => requestCount_int; set
        {
            requestCount_int = value;
            Debug.Log("requestCount_int: " + requestCount_int);
            CheckRequestCount();
        }
    }
    //<聊天室Id,最後已讀訊息Id>
    public Dictionary<int, int> haveReadPrivate_dict = new Dictionary<int, int>();

    //=================================================================
    //可監聽事件
    public System.Action OnListRefresh;
    public System.Action OnRequestCompleted;

    //=================================================================
    #endregion
    //=================================================================
    #region UnityEvent
    //=================================================================
    void Start()
    {
        RefreshLastChatMessageList();
        GetPrivateMsgHaveReadFile();
    }

    //=================================================================
    void OnEnable()
    {
        friendListHandler.OnOpenPrivateChatMessageView += OpenPrivateChatMessageView;
        PrivateMessageFirebaseHandler.Instance.OnPrivateMessageReceived += FirebasePrivateChat;
    }

    //=================================================================
    void OnDisable()
    {
        friendListHandler.OnOpenPrivateChatMessageView -= OpenPrivateChatMessageView;
        PrivateMessageFirebaseHandler.Instance.OnPrivateMessageReceived -= FirebasePrivateChat;
    }

    //=================================================================
    #endregion
    //=================================================================
    #region 操作相關
    //=================================================================
    /// <summary>
    /// 開啟好友列表
    /// </summary>
    public void OpenFriendListView()
    {
        friendListHandler.OpenFriendListView();
    }

    //====================================================================
    /// <summary>
    /// 以Button觸發後開啟私聊列表
    /// </summary>
    public void OpenPrivateRoomListView()
    {
        if (privateRoomListUI == null)
        {
            privateRoomListUI = Instantiate(privateRoomListUI_pf, SceneController.Instance.CurrentSceneCanvas.transform);
            privateRoomListUI.LoadAndShowView(this);
        }
        else
        {
            privateRoomListUI.LoadAndShowView(this);
        }

    }

    //====================================================================
    /// <summary>
    /// 開啟私聊內容介面
    /// </summary>
    public void OpenPrivateChatMessageView(ResponseStruct.OtherMemberInfo_struct _targetMemberInfo)
    {

        if (privateRoomListUI == null)
        {
            privateRoomListUI = Instantiate(privateRoomListUI_pf, SceneController.Instance.CurrentSceneCanvas.transform);
            privateRoomListUI.LoadAndShowView(this);
        }

        if (privateChatMessageHandler == null)
        {
            privateChatMessageHandler = Instantiate(privateChatMessageHandler_pf, SceneController.Instance.CurrentSceneCanvas.transform);
            privateChatMessageHandler.Set(_targetMemberInfo, this);
        }
        else
        {
            privateChatMessageHandler.Set(_targetMemberInfo, this);
        }

    }

    //=================================================================
    /// <summary>
    /// 檢查是否要顯示私聊提示
    /// </summary>
    public void CheckPrivateMsgHint()
    {
        privateMsgHint_gObj.SetActive(!CheckAllHaveRead());
    }

    //=================================================================
    #endregion
    //=================================================================
    #region 資料處理
    //=================================================================
    /// <summary>
    /// 接收到Firebase的私聊通知的處理
    /// </summary>
    void FirebasePrivateChat()
    {
        RefreshLastChatMessageList();
    }

    //=================================================================
    /// <summary>
    /// 刷新最後訊息列表
    /// </summary>
    void RefreshLastChatMessageList()
    {
        PrivateMessageManager.Instance.FetchLastPrivateMsg((_result) =>
        {
            lastChatMessage_arry = _result;
            CheckPrivateMsgHint();
            OnListRefresh?.Invoke();
        });
    }

    //====================================================================
    /// <summary>
    /// 刷新最後訊息列表
    /// </summary>
    public void GetLastChatMessageList(System.Action _CallBack)
    {
        RequestCount_int++;
        PrivateMessageManager.Instance.FetchLastPrivateMsg((System.Action<PrivateChatRecord_struct[]>)((_result) =>
        {
            lastChatMessage_arry = _result;
            CheckPrivateMsgHint();
            OnListRefresh?.Invoke();
            _CallBack?.Invoke();
            RequestCount_int--;
        }));
    }

    //=================================================================
    /// <summary>
    /// 更新已讀紀錄
    /// </summary>
    /// <param name="_privateRoom_id">聊天室Id</param>
    /// <param name="_lastPrivateMsg_id">最後一筆私聊Id</param>
    public void UpdateHaveRead(int _privateRoom_id, int _lastPrivateMsg_id)
    {
        //-------------------------------------------------------------
        // 檢查此聊天室Id是否已被紀錄
        if (!haveReadPrivate_dict.ContainsKey(_privateRoom_id))
        {
            haveReadPrivate_dict.Add(_privateRoom_id, _lastPrivateMsg_id);
            SetPrivateMsgHaveReadFile();
            return;
        }

        // 檢查 新的訊息Id是否比舊的已讀Id還小，出現即為不合理情況
        if (haveReadPrivate_dict[_privateRoom_id] > _lastPrivateMsg_id)
        {
            Debug.LogError("PrivateMessageHandler.UpdateHaveRead() Error: 新的訊息Id比舊的已讀Id還小，不合理!");
            return;
        }

        //-------------------------------------------------------------
        // 更新已讀紀錄
        haveReadPrivate_dict[_privateRoom_id] = _lastPrivateMsg_id;
        SetPrivateMsgHaveReadFile();
        //-------------------------------------------------------------
    }

    //=================================================================
    /// <summary>
    /// 檢查單筆已讀紀錄
    /// </summary>
    /// <param name="_privateRoom_id">聊天室Id</param>
    /// <param name="_lastPrivateMsg_id">最後一筆私聊Id</param>
    /// <returns>true為已讀，false為未讀</returns>
    public bool CheckSingleHaveRead(int _privateRoom_id, int _lastPrivateMsg_id)
    {
        //-------------------------------------------------------------}
        // 如果有聊天室Id不存在，代表有未讀訊息
        if (!haveReadPrivate_dict.ContainsKey(_privateRoom_id))
        {
            return false;
        }

        //-------------------------------------------------------------
        // 檢查 新的訊息Id是否比舊的已讀Id還小，出現即為不合理情況
        if (haveReadPrivate_dict[_privateRoom_id] > _lastPrivateMsg_id)
        {
            Debug.LogError("PrivateMessageHandler.UpdateHaveRead() Error: 新的訊息Id比舊的已讀Id還小，不合理!");
            return false;
        }

        //-------------------------------------------------------------
        // 如果 新的訊息Id等於舊的已讀Id，，代表已讀
        if (haveReadPrivate_dict[_privateRoom_id] == _lastPrivateMsg_id)
        {
            return true;
        }

        //-------------------------------------------------------------
        // 剩下情況皆為未讀
        return false;

        //-------------------------------------------------------------
    }

    //=================================================================
    /// <summary>
    /// 檢查全部已讀紀錄
    /// </summary>
    /// <returns>true為全部訊息皆已讀，false為至少有一筆訊息尚未已讀</returns>
    public bool CheckAllHaveRead()
    {
        //-------------------------------------------------------------
        // 如果遠端訊息陣列為Null，可能表示尚未有訊息產生，所以判定為全部皆已讀
        if (lastChatMessage_arry == null)
        {
            Debug.LogWarning("PrivateMessageHandler.CheckAllHaveRead() 遠端訊息陣列為Null");
            return true;
        }

        //-------------------------------------------------------------
        // 檢查所有最後私聊訊息
        for (int i = 0; i < lastChatMessage_arry.Length; i++)
        {
            var _lastPrivateMsg = lastChatMessage_arry[i];
            int _privateRoom_id = -1;
            // 選擇不等於此帳號會員Id的目標為此聊天室Id
            if (_lastPrivateMsg.target.id == MemberInformationManager.Id)
            {
                _privateRoom_id = _lastPrivateMsg.initiator.id;
            }
            else
            {
                _privateRoom_id = _lastPrivateMsg.target.id;
            }
            // 如果聊天室Id小於1，不合理
            if (_privateRoom_id < 1)
            {
                Debug.LogError("PrivateMessageHandler.CheckSingleHaveRead() Error: 聊天室Id小於1，不合理");
                return false;
            }
            // 如果有一筆結果為未讀，回傳false
            if (!CheckSingleHaveRead(_privateRoom_id, _lastPrivateMsg.id))
            {
                return false;
            }


        }

        //-------------------------------------------------------------
        return true;
    }

    //=================================================================
    /// <summary>
    /// 取得本地已讀紀錄
    /// </summary>
    void GetPrivateMsgHaveReadFile()
    {
        //-------------------------------------------------------------
        // 讀取資料
        string _content_str = FileManager.ReadData(FileManager.FileName.PrivateMsgHaveReadFile);

        // 檢查讀取到的資料
        if (string.IsNullOrEmpty(_content_str))
        {
            Debug.Log("haveReadPrivate_dict為空");
            haveReadPrivate_dict = new Dictionary<int, int>();
            return;
        }

        //-------------------------------------------------------------
        // 解析讀取到的資料
        try
        {
            haveReadPrivate_dict = JsonUtility.FromJson<Serialization<int, int>>(_content_str).ToDictionary();
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }

        //-------------------------------------------------------------
    }

    //=================================================================
    /// <summary>
    /// 寫入本地已讀紀錄
    /// </summary>
    void SetPrivateMsgHaveReadFile()
    {
        //-------------------------------------------------------------
        // 檢查haveReadPrivate_dict是否有資料
        if (haveReadPrivate_dict == null)
        {
            Debug.LogWarning("haveReadPrivate_dict為空");
            return;
        }
        if (haveReadPrivate_dict.Count <= 0)
        {
            Debug.LogWarning("haveReadPrivate_dict內資料數量不達1個");
            return;
        }

        //-------------------------------------------------------------
        // 嘗試寫入檔案
        try
        {
            string _content_str = JsonUtility.ToJson(new Serialization<int, int>(haveReadPrivate_dict));
            if (!FileManager.WriteData(FileManager.FileName.PrivateMsgHaveReadFile, _content_str))
            {
                Debug.LogError("已讀資料寫入失敗");
                return;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
            return;
        }

        // 寫入檔案成功
        Debug.Log("已讀資料寫入成功");
        //-------------------------------------------------------------
    }
    //=================================================================
    #endregion 資料處理
    //=================================================================
    #region 物件生成
    //=================================================================
    /// <summary>
    /// 生成私聊聊天室的物件
    /// </summary>
    /// <param name="_content_transform">私聊聊天室列表的生成區域</param>
    /// <param name="_emptyHint_gObj">如私聊聊天室為空時的提示字</param>
    public void BuildPrivateRoom(Transform _content_transform, GameObject _emptyHint_gObj)
    {
        List<PrivateChatRecord_struct> _sort_list = new List<PrivateChatRecord_struct>();
        _sort_list.AddRange(lastChatMessage_arry);
        _sort_list.Sort((x, y) => -x.id.CompareTo(y.id));

        BuildPrivateRoom(_sort_list.ToArray(), _content_transform, _emptyHint_gObj, ref privateRoomObjects_list);
    }

    //====================================================================
    /// <summary>
    /// 生成私聊聊天室列表的主要函式
    /// </summary>
    /// <param name="_friendInfo_arry">私聊聊天室列表資料的陣列</param>
    /// <param name="_content_transform">私聊聊天室列表的生成區域</param>
    /// <param name="_emptyHint_gObj">如私聊聊天室為空時的提示字</param>
    /// <param name="_friendObjects_list">儲存生成之私聊聊天室物件的List</param>
    public void BuildPrivateRoom(ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct[] _PrivateChat_arry, Transform _content_transform, GameObject _emptyHint_gObj, ref List<PrivateRoomObject> _friendObjects_list)
    {
        if (_PrivateChat_arry != null)
        {
            if (_PrivateChat_arry.Length <= _friendObjects_list.Count)
            {
                int _count = 0;

                for (; _count < _PrivateChat_arry.Length; _count++)
                {
                    _friendObjects_list[_count].Set(_PrivateChat_arry[_count], this);
                }

                for (; _count < _friendObjects_list.Count;)
                {
                    Destroy(_friendObjects_list[_count].gameObject);
                    _friendObjects_list.Remove(_friendObjects_list[_count]);
                }
            }
            else
            {
                int _count = 0;

                for (; _count < _friendObjects_list.Count; _count++)
                {
                    _friendObjects_list[_count].Set(_PrivateChat_arry[_count], this);
                }

                for (; _count < _PrivateChat_arry.Length; _count++)
                {
                    var _orderObject = Instantiate(privateRoomObject_pf, _content_transform);
                    _orderObject.Set(_PrivateChat_arry[_count], this);
                    _friendObjects_list.Add(_orderObject);
                }
            }
        }
        else
        {
            for (int i = 0; i < _friendObjects_list.Count; i++)
            {
                Destroy(_friendObjects_list[i].gameObject);
            }
            _friendObjects_list.Clear();
        }

        _emptyHint_gObj.SetActive(_PrivateChat_arry == null || _PrivateChat_arry.Length == 0);
    }

    //=================================================================
    #endregion
    //=================================================================
    #region 載入畫面處理
    //=================================================================
    /// <summary>
    /// 依目前尚未完成的請求數量決定顯示載入視窗
    /// </summary>
    void CheckRequestCount()
    {
        if (RequestCount_int <= 0)
        {
            requestCount_int = 0;
            OnRequestCompleted?.Invoke();
            loadingObjectManager.LoadingCountSub();
        }
    }

    //====================================================================
    /// <summary>
    /// 提供給隸屬於其功能下需要載入畫面的組件使用
    /// </summary>
    public void LoadingCountSub()
    {
        loadingObjectManager.LoadingCountSub();
    }

    //====================================================================
    /// <summary>
    /// 提供給隸屬於其功能下需要載入畫面的組件使用
    /// </summary>
    public void LoadingCountAdd()
    {
        loadingObjectManager.LoadingCountAdd();
    }

    //=================================================================
    #endregion
    //=================================================================
}
