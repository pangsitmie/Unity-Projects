using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class CustomUnityEvent : UnityEvent<float> { }
public class MachineInformationUI : MonoBehaviour
{

    [SerializeField] Text status_tx;
    [SerializeField] Text connectTime_tx;
    [SerializeField] MoveOut this_moveOut;
    public event System.Action<System.Action> OnDisconnect;
    [SerializeField] string status_str = "未連接";
    [SerializeField] LoadingObjectManager loadingObjectManager;
    DateTime startConnectTime;
    SocketIOStruct.machineTypeResponse_struct machineData;

    //====================================================================
    /// <summary>
    /// 載入機台資料與注入中斷連線事件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Load(string _status, SocketIOStruct.machineTypeResponse_struct _machineData, DateTime _connectDateTime, System.Action<System.Action> _OnDisconnect, LoadingObjectManager _loadingObjectManager)
    {
        loadingObjectManager = _loadingObjectManager;
        machineData = _machineData;
        OnDisconnect = _OnDisconnect;
        startConnectTime = _connectDateTime;
        status_str = _status;
        StatusDisplay();
        InvokeRepeating("ConnectTimeDisplay", 0, 0.2f);
        MachineInformationManager.Instance.OnStatusChange += StatusChange;
        MachineInformationManager.Instance.OnGameResult += ShowResult;
        MachineInformationManager.Instance.OnLoadingCountAdd += LoadingAdd;
        MachineInformationManager.Instance.OnLoadingCountSub += LoadingSub;
        MachineInformationManager.Instance.OnLoadingCountReset += LoadingReset;
        this_moveOut.Move(true);
    }


    //====================================================================
    /// <summary>
    /// 更新狀態的顯示
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 因非主線程觸發，所以跟UI相關顯示變更應由Update觸發</para>
    /// </summary>
    void StatusDisplay()
    {
        switch (status_str)
        {
            case "未連接":
                Quit();
                break;
            case "等待遊戲":
                status_tx.text = "等待開始";
                break;
            case "遊戲中":
                status_tx.text = "遊戲中";
                break;
        }

    }

    //====================================================================
    /// <summary>
    /// 顯示已連線的時間
    /// <para>編輯者: Dimos</para>
    /// </summary>
    void ConnectTimeDisplay()
    {
        var _time = (DateTime.Now - startConnectTime);
        connectTime_tx.text = _time.Hours.ToString("00") + ":" + _time.Minutes.ToString("00") + ":" + _time.Seconds.ToString("00");
    }

    //====================================================================
    /// <summary>
    /// 顯示已投入的硬幣量
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void CoinChange()
    {

    }

    //====================================================================
    /// <summary>
    /// 連線狀態改變
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 因非主線程觸發，所以跟UI相關顯示變更應由Update觸發</para>
    /// </summary>
    public void StatusChange(string _status_str)
    {
        Debug.Log("StatusChange(UI): " + _status_str);
        status_str = _status_str;

        AsyncUIEventManager.Instance.AddAsyncUIEvent(() => { StatusDisplay(); });
    }

    //====================================================================
    /// <summary>
    /// 開啟查看機台資料詳細
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void OpenDetail()
    {

    }

    //====================================================================
    /// <summary>
    /// 顯示遊玩結果
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void ShowResult(SocketIOClient.SocketIOResponse _response)
    {

    }

    //====================================================================
    /// <summary>
    /// 顯示遊玩結果
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void DisconnectCheck()
    {
        switch (status_str)
        {
            case "遊戲中":
                NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.main_cantDisconnectInGame);
                break;
            default:
                Disconnect();
                break;
        }
    }

    //====================================================================
    /// <summary>
    /// 繼續連線
    /// <para>編輯者: Dimos</para>
    /// </summary>
    void ContinueMatch()
    {
        MachineInformationManager.Instance.ContinueMatch();
    }

    //====================================================================
    /// <summary>
    /// 中斷連線
    /// <para>編輯者: Dimos</para>
    /// </summary>
    void Disconnect()
    {
        OnDisconnect?.Invoke(Quit);
    }

    //====================================================================
    /// <summary>
    /// 關閉視窗
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Quit()
    {
        this_moveOut.Move(false);
        MachineInformationManager.Instance.OnStatusChange -= StatusChange;
        MachineInformationManager.Instance.OnGameResult -= ShowResult;
        MachineInformationManager.Instance.OnLoadingCountAdd -= LoadingAdd;
        MachineInformationManager.Instance.OnLoadingCountSub -= LoadingSub;
        MachineInformationManager.Instance.OnLoadingCountReset -= LoadingReset;
    }
    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        Quit();
    }
    //====================================================================
    /// <summary>
    /// 顯示載入中畫面
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void LoadingAdd()
    {

        AsyncUIEventManager.Instance.AddAsyncUIEvent(() => { loadingObjectManager.LoadingCountAdd(); });
    }
    //====================================================================
    /// <summary>
    /// 關閉載入中畫面
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void LoadingSub()
    {
        AsyncUIEventManager.Instance.AddAsyncUIEvent(() => { loadingObjectManager.LoadingCountSub(); });
    }

    //====================================================================
    /// <summary>
    /// 重置載入中畫面
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void LoadingReset()
    {
        AsyncUIEventManager.Instance.AddAsyncUIEvent(() => { loadingObjectManager.LoadingCountReset(); });
    }
}
