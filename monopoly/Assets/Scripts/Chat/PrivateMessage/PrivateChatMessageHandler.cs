using System;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivateChatMessageHandler : MonoBehaviour
{
    //=================================================================
    #region 參數
    //=================================================================
    public System.Action OnClearChatMessage;
    public System.Action<ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct> OnAddOldSelfSideChatMessage;
    public System.Action<ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct> OnAddOldOtherSideChatMessage;
    public System.Action<ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct> OnAddNewSelfSideChatMessage;
    public System.Action<ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct> OnAddNewOtherSideChatMessage;
    public System.Action<ResponseStruct.OtherMemberInfo_struct> OnShowView;
    public System.Action OnQuitView;//由其他組件決定關閉此畫面實觸發(尚未使用)

    //=================================================================
    ResponseStruct.OtherMemberInfo_struct targetMemberInfo;

    // 已顯示的訊息(last為最新、first為最舊)
    LinkedList<ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct> lastPrivateChatRecord_linkedList = new LinkedList<ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct>();
    // 尚未顯示的舊訊息
    Stack<ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct> notYetDisplayOldPrivateChatRecord_stack = new Stack<ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct>();
    // 尚未顯示的新訊息
    Stack<ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct> notYetDisplayNewPrivateChatRecord_stack = new Stack<ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct>();

    //=================================================================
    PrivateMessageHandler privateMessageHandler;

    //=================================================================
    #endregion
    //=================================================================
    #region 載入資料
    //=================================================================
    /// <summary>
    /// 設定私聊內容資料
    /// </summary>
    /// <param name="_targetMemberInfo">對方的會員資料</param>
    public void Set(ResponseStruct.OtherMemberInfo_struct _targetMemberInfo, PrivateMessageHandler _privateMessageHandler)
    {
        privateMessageHandler = _privateMessageHandler;
        targetMemberInfo = _targetMemberInfo;

        lock (lastPrivateChatRecord_linkedList)
        {
            lastPrivateChatRecord_linkedList.Clear();
        }
        notYetDisplayOldPrivateChatRecord_stack.Clear();
        notYetDisplayNewPrivateChatRecord_stack.Clear();

        RegisterListener();

        FetchPrivateChatRecord();

    }

    //=================================================================
    // FIXME: 更改一次顯示數量，目前暫時先一次生成50筆且不追加
    /// <summary>
    /// 取得所有舊訊息，並顯示
    /// </summary>
    public void FetchPrivateChatRecord()
    {
        privateMessageHandler.LoadingCountAdd();
        PrivateMessageManager.Instance.FetchPrivateChatRecord(targetMemberInfo.id, (_result) =>
        {
            notYetDisplayOldPrivateChatRecord_stack.Clear();
            ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct[] lastPrivateChatRecord_arry = _result;
            int _lastPrivateMsg_id = -1;
            if (lastPrivateChatRecord_arry != null)
            {
                for (int i = 0; i < lastPrivateChatRecord_arry.Length; i++)
                {
                    ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct _privateChatRecordData = lastPrivateChatRecord_arry[i];
                    _lastPrivateMsg_id = _privateChatRecordData.id;
                    notYetDisplayOldPrivateChatRecord_stack.Push(_privateChatRecordData);
                }
            }

            OnClearChatMessage?.Invoke();
            AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
            {
                DisplayOldPrivateChat(50);
                // 最新的訊息Id需大於零才合理
                if (_lastPrivateMsg_id > 0)
                    privateMessageHandler.UpdateHaveRead(targetMemberInfo.id, _lastPrivateMsg_id);
                else
                    Debug.LogWarning("PrivateChatMessageHandler.FetchPrivateChatRecord() 最新的訊息Id小於等於於零，該聊天室目標Id: " + targetMemberInfo.id);

            });
        });
    }

    //=================================================================
    /// <summary>
    /// 接收到Firebase的私聊通知
    /// </summary>
    public void FirebasePrivateChat()
    {
        // FIXME: 尚未檢查Firebase資料
        Debug.Log("PrivateChatMessageHandler收到Firebase!");
        GetNewPrivateChatRecord();
    }

    //=================================================================
    /// <summary>
    /// 取得新的私聊訊息
    /// </summary>
    public void GetNewPrivateChatRecord()
    {
        PrivateMessageManager.Instance.FetchPrivateChatRecord(targetMemberInfo.id, (_result) =>
        {
            UpdateNewPrivateChatRecord(_result);
        });
    }

    //=================================================================
    // FIXME: 尚未實作過多新訊息的處理
    // FIXME: 等待API修改取得私聊訊息的數量限制
    /// <summary>
    /// 更新私聊訊息
    /// </summary>
    public void UpdateNewPrivateChatRecord(ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct[] _newPrivateChatRecord_arry)
    {
        lock (lastPrivateChatRecord_linkedList)
        {
            //-------------------------------------------------------------
            //檢查遠端私聊紀錄是否為Null
            if (_newPrivateChatRecord_arry == null)
            {
                Debug.LogWarning("遠端私聊紀錄是為Null");
                return;
            }

            //檢查遠端私聊紀錄是否為空
            if (_newPrivateChatRecord_arry.Length <= 0)
            {
                Debug.LogWarning("遠端私聊紀錄是為空");
                return;
            }

            //檢查本地私聊紀錄是否為Null
            if (lastPrivateChatRecord_linkedList == null)
            {
                Debug.LogWarning("本地私聊紀錄是為Null");
                return;
            }

            //檢查本地私聊紀錄是否為空
            if (lastPrivateChatRecord_linkedList.Count <= 0)
            {
                Debug.LogWarning("本地私聊紀錄是為空");
                return;
            }

            //-------------------------------------------------------------
            //取得本地現有最新一筆的ID
            int _lastPrivateMsg_id = lastPrivateChatRecord_linkedList.Last.Value.id;

            int _newPrivateChatLength = _newPrivateChatRecord_arry.Length;

            //-------------------------------------------------------------
            // 若是取得的 遠端最新私聊ID 小於 本地最新私聊ID，表示本地資訊新於遠端資訊，此為不合理狀況 
            if (_newPrivateChatRecord_arry[_newPrivateChatLength - 1].id < _lastPrivateMsg_id)
            {
                Debug.LogError("本地資訊新於遠端資訊，此為不合理狀況");
                AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                {
                    NotificationManager.Instance.CreateConfirmView(LocalizationManager.KeyTable.common_httpRequestError);
                });
                return;
            }

            //-------------------------------------------------------------
            for (int i = _newPrivateChatLength - 1; i > 0; i--)
            {
                var _chatData = _newPrivateChatRecord_arry[i];

                //已找到本地最新私聊資訊，終止迴圈
                if (_chatData.id == _lastPrivateMsg_id)
                {
                    break;
                }
                // 若是取得的遠端私聊ID在未尋找到本地最新私聊ID時小於本地最新私聊ID，表示遠端資訊中缺少本地最新私聊，此為不合理狀況 
                if (_chatData.id < _lastPrivateMsg_id)
                {
                    Debug.LogError("遠端資訊中缺少本地最新私聊，此為不合理狀況 ");
                    AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                    {
                        NotificationManager.Instance.CreateConfirmView(LocalizationManager.KeyTable.common_httpRequestError);
                    });
                    return;
                }
                //此為本地未顯示訊息，添加到未顯示私聊的Stack
                notYetDisplayNewPrivateChatRecord_stack.Push(_chatData);
            }

            //-------------------------------------------------------------
            DisplayNewPrivateChat();

            //-------------------------------------------------------------
        }
    }
    //=================================================================
    #endregion
    //=================================================================
    #region UI驅動
    //=================================================================
    // FIXME: 尚未判斷_target.messageType
    /// <summary>
    /// 顯示尚未顯示的訊息
    /// </summary>
    /// <param name="_displayAmount_int">欲顯示的訊息數量</param>
    /// <returns>回傳最終有生成的數量</returns>
    public int DisplayOldPrivateChat(int _displayAmount_int = 10)
    {
        int _displayedCount_int;
        for (_displayedCount_int = 0; _displayedCount_int < _displayAmount_int - 1; _displayedCount_int++)
        {
            // 在Stack內僅存的資料少於等於1時結束迴圈，為了讓最後一筆資料在下一frame才生成來使UI的適應化正常運作
            if (notYetDisplayOldPrivateChatRecord_stack.Count <= 1)
            {
                break;
            }

            // 如果無法取出資料表示已無未顯示的資料，結束迴圈
            if (!notYetDisplayOldPrivateChatRecord_stack.TryPop(out var _chatData))
            {
                break;
            }
            AddOldChatMessage(_chatData);
        }

        //用異步執行來達成下一frame才生成物件來使UI的適應化正常運作
        AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
        {
            if (notYetDisplayOldPrivateChatRecord_stack.TryPop(out var _chatData))
            {
                AddOldChatMessage(_chatData);
            }
            privateMessageHandler.LoadingCountSub();
            OnShowView?.Invoke(targetMemberInfo);
        });
        Debug.Log("DisplayOldPrivateChat() 本次顯示訊息數量: " + _displayedCount_int);

        // 回傳最終有生成的數量
        return _displayedCount_int;
    }


    //=================================================================
    /// <summary>
    /// 顯示尚未顯示的訊息
    /// </summary>
    /// <param name="_displayAmount_int">欲顯示的訊息數量</param>
    /// <returns>回傳最終有生成的數量</returns>
    public int DisplayNewPrivateChat()
    {
        int _displayAmount_int = notYetDisplayNewPrivateChatRecord_stack.Count;
        int _displayedCount_int;
        for (_displayedCount_int = 0; _displayedCount_int < _displayAmount_int - 1; _displayedCount_int++)
        {
            // 在Stack內僅存的資料少於等於1時結束迴圈，為了讓最後一筆資料在下一frame才生成來使UI的適應化正常運作
            if (notYetDisplayNewPrivateChatRecord_stack.Count <= 1)
            {
                break;
            }

            // 如果無法取出資料表示已無未顯示的資料，結束迴圈
            if (!notYetDisplayNewPrivateChatRecord_stack.TryPop(out var _chatData))
            {
                break;
            }
            AddNewChatMessage(_chatData);
        }

        //用異步執行來達成下一frame才生成物件來使UI的適應化正常運作
        AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
        {
            if (notYetDisplayNewPrivateChatRecord_stack.TryPop(out var _chatData))
            {
                AddNewChatMessage(_chatData);
            }

            privateMessageHandler.UpdateHaveRead(targetMemberInfo.id, _chatData.id);

            privateMessageHandler.LoadingCountSub();
            OnShowView?.Invoke(targetMemberInfo);
        });
        Debug.Log("DisplayNewPrivateChat() 本次顯示訊息數量: " + _displayedCount_int);

        // 回傳最終有生成的數量
        return _displayedCount_int;
    }
    //=================================================================
    public void AddOldChatMessage(ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct _chatData)
    {
        lock (lastPrivateChatRecord_linkedList)
        {
            // 未顯示的資料的發起人ID與個人ID相同則顯示己方訊息，結束本輪
            if (_chatData.initiator.id == MemberInformationManager.Id)
            {
                switch (_chatData.messageType)
                {
                    default:
                        lastPrivateChatRecord_linkedList.AddFirst(_chatData);
                        OnAddOldSelfSideChatMessage?.Invoke(_chatData);
                        break;
                }
                return;
            }

            // 未顯示的資料的發起人ID與個人ID不同則顯示對方訊息
            switch (_chatData.messageType)
            {
                default:
                    lastPrivateChatRecord_linkedList.AddFirst(_chatData);
                    OnAddOldOtherSideChatMessage?.Invoke(_chatData);
                    break;
            }
        }
    }

    //=================================================================
    public void AddNewChatMessage(ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct _chatData)
    {
        lock (lastPrivateChatRecord_linkedList)
        {
            // 未顯示的資料的發起人ID與個人ID相同則顯示己方訊息，結束本輪
            if (_chatData.initiator.id == MemberInformationManager.Id)
            {
                switch (_chatData.messageType)
                {
                    default:
                        lastPrivateChatRecord_linkedList.AddLast(_chatData);
                        OnAddNewSelfSideChatMessage?.Invoke(_chatData);
                        break;
                }
                return;
            }

            // 未顯示的資料的發起人ID與個人ID不同則顯示對方訊息
            switch (_chatData.messageType)
            {
                default:
                    lastPrivateChatRecord_linkedList.AddLast(_chatData);
                    OnAddNewOtherSideChatMessage?.Invoke(_chatData);
                    break;
            }
        }
    }
    //=================================================================
    /// <summary>
    /// 發送私聊訊息
    /// </summary>
    /// <param name="_message_str">訊息文字</param>
    public void SendPrivateMessage(string _message_str)
    {
        lock (lastPrivateChatRecord_linkedList)
        {
            // 檢查輸入訊息是否為空
            if (string.IsNullOrEmpty(_message_str))
            {
                Debug.Log("SendPrivateMessage輸入為空");
                return;
            }
            // FIXME: 之後Server會回傳，就不需要自己創了
            // 送出私聊
            PrivateMessageManager.Instance.SendPrivateMsg(targetMemberInfo.id, _message_str, (_response) =>
            {
                // 回傳接收失敗
                if (_response == null)
                {
                    Debug.LogError("PrivateChatMessageHandler.SendPrivateMessage() 回傳接收失敗");
                    return;
                }

                // 此私聊Id比舊的已讀Id還小，不合理
                if (_response.id < lastPrivateChatRecord_linkedList.Last.Value.id)
                {
                    Debug.LogError("PrivateChatMessageHandler.SendPrivateMessage() 此私聊Id比舊的已讀Id還小，不合理!");
                    return;
                }

                // 更新本地私聊紀錄
                privateMessageHandler.UpdateHaveRead(targetMemberInfo.id, _response.id);

                // 此私聊Id已重複，代表不需再次建立新的訊息
                if (_response.id == lastPrivateChatRecord_linkedList.Last.Value.id)
                {
                    Debug.LogWarning("PrivateChatMessageHandler.SendPrivateMessage() 此私聊Id已重複");
                    return;
                }

                // 建立新的訊息
                lastPrivateChatRecord_linkedList.AddLast(_response);
                OnAddNewSelfSideChatMessage?.Invoke(_response);
            });
        }
    }


    //=================================================================
    /// <summary>
    /// 刷新最後一次的訊息
    /// </summary>
    public void RefreshLastChatList()
    {
        privateMessageHandler.GetLastChatMessageList(() => { });
    }

    //=================================================================
    /// <summary>
    /// 關閉私聊畫面
    /// </summary>
    public void QuitView()
    {
        UnRegisterListener();
    }

    //=================================================================
    #endregion
    //=================================================================
    #region 事件監聽
    //=================================================================
    /// <summary>
    /// 註冊所需監聽事件
    /// </summary>
    void RegisterListener()
    {
        PrivateMessageFirebaseHandler.Instance.OnPrivateMessageReceived += FirebasePrivateChat;
    }
    /// <summary>
    /// 註銷所需監聽事件
    /// </summary>
    void UnRegisterListener()
    {
        PrivateMessageFirebaseHandler.Instance.OnPrivateMessageReceived -= FirebasePrivateChat;
    }
    //=================================================================
    #endregion
    //=================================================================

}
