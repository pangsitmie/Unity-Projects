using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Newtonsoft.Json;
public class MachineInformationManager : MonoBehaviour
{
    static MachineInformationManager instance = null;
    public static MachineInformationManager Instance { get => instance; }
    public delegate void StatusChangeEvent(string _status_str);
    public UnityEvent OnMachineMatch;
    public StatusChangeEvent OnStatusChange;
    public System.Action OnLoadingCountAdd;
    public System.Action OnLoadingCountSub;
    public System.Action OnLoadingCountReset;
    public event SocketIOManager.SocketIOResponseEventHandler OnGameResult;
    public string status;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    public SocketIOStruct.machineTypeResponse_struct machineData { get; private set; }
    public DateTime connectDateTime;
    [SerializeField] MachineGameResultObject machineGameResultObject_pf;
    [SerializeField] TicketAnimationHandler ticketAnimation_pf;
    MachineGameResultObject machineGameResultObject;
    TicketAnimationHandler ticketAnimation;
    ResponseStruct.MachineGameResultResponse_struct machineGameResultData;
    void Awake()
    {
        instance = this;
    }
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        status = "未連接";
    }
    //====================================================================
    /// <summary>
    /// 設定配對到的機台資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void SetMachineData(SocketIOStruct.machineTypeResponse_struct _machineData)
    {
        machineData = _machineData;
        status = "等待遊戲";
        connectDateTime = DateTime.Now;
        SocketIOManager.Instance.SetSocketOnEvent("gameStart", true, GameStart);
        SocketIOManager.Instance.SetSocketOnEvent("gameResult", true, GameResult);
        SocketIOManager.Instance.SetSocketOnEvent("matchTimeout", true, MatchTimeout);
        SocketIOManager.Instance.SetSocketOnEvent("cancelMatch", true, CancelMatch);
        SocketIOManager.Instance.OnError += SoceketError;
        SocketIOManager.Instance.OnReconnected += OnReconnected;
        SocketIOManager.Instance.OnDisconnected += OnDisconnected;
        OnMachineMatch?.Invoke();
    }

    //====================================================================
    /// <summary>
    /// 遊戲開始訊號
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void GameStart(SocketIOClient.SocketIOResponse _response)
    {
        status = "遊戲中";
        Debug.Log("MachineInformationManager.GameStart() 遊戲開始: " + _response.ToString());
        OnStatusChange?.Invoke(status);
    }

    //====================================================================
    /// <summary>
    /// 遊玩結果訊號
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void GameResult(SocketIOClient.SocketIOResponse _response)
    {
        Debug.Log("MachineInformationManager.GameResult() 遊戲結果: " + _response.ToString());
        try
        {
            ContinueMatch();
            AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
            {
                if (machineGameResultObject == null)
                {
                    machineGameResultObject = Instantiate(machineGameResultObject_pf, SceneController.Instance.CurrentSceneCanvas.transform);
                }
                machineGameResultObject.Set(machineData, machineGameResultData, null, () => { ClientDisconnect(null); });
                if (!SettingData.Data.skipTicketAnimation && machineGameResultData.ticket > 0)
                {
                    if (ticketAnimation != null)
                    {
                        Destroy(ticketAnimation.gameObject);
                    }
                    ticketAnimation = Instantiate(ticketAnimation_pf, SceneController.Instance.CurrentSceneCanvas.transform);
                    ticketAnimation.Set(machineGameResultData.ticket);
                }

                MemberInformationManager.Instance.RequestMemberInformation();
            });
            machineGameResultData = JsonConvert.DeserializeObject<List<ResponseStruct.MachineGameResultResponse_struct>>(_response.ToString())[0];
            OnGameResult?.Invoke(_response);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex, this);
        }
    }

    //====================================================================
    /// <summary>
    /// 與機台配對逾時訊號
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void MatchTimeout(SocketIOClient.SocketIOResponse _response)
    {
        status = "未連接";
        AsyncUIEventManager.Instance.AddAsyncUIEvent(() => { NotificationManager.Instance.CreateConfirmView(LocalizationManager.KeyTable.common_timeout); });
        Debug.Log("MachineInformationManager.MatchTimeout() 連線逾時: " + _response.ToString());
        OnStatusChange?.Invoke(status);
        OnUnPair();
        OnLoadingCountReset?.Invoke();
    }

    //====================================================================
    /// <summary>
    /// Server端主動中斷連線訊號
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void CancelMatch(SocketIOClient.SocketIOResponse _response)
    {
        status = "未連接";
        AsyncUIEventManager.Instance.AddAsyncUIEvent(() => { NotificationManager.Instance.CreateConfirmView(LocalizationManager.KeyTable.common_timeout); });
        Debug.Log("MachineInformationManager.CancelMatch() 取消配對: " + _response.ToString());
        OnStatusChange?.Invoke(status);
        OnUnPair();
        OnLoadingCountReset?.Invoke();
    }

    //====================================================================
    /// <summary>
    /// Server端主動中斷連線訊號
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void SoceketError(SocketIOClient.SocketIOResponse _response)
    {
        status = "未連接";
        AsyncUIEventManager.Instance.AddAsyncUIEvent(() => { NotificationManager.Instance.CreateConfirmView(LocalizationManager.KeyTable.common_timeout); });
        Debug.Log("MachineInformationManager.SoceketError() 連線錯誤: " + _response.ToString());
        OnStatusChange?.Invoke(status);
        OnUnPair();
        OnLoadingCountReset?.Invoke();
    }

    //====================================================================
    /// <summary>
    /// App端主動中斷連線訊號
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void ClientDisconnect(System.Action _DisconnectSuccess)
    {
        OnLoadingCountAdd?.Invoke();
        SocketIOManager.Instance.SetSocketEmitEventAsync("clientCancelMatch", (_response) =>
        {
            Debug.Log("ClientDisconnectCallback(): " + _response.ToString());
            SocketIOStruct.SocketIOResponseMessage<bool> _disconnectResponse = new SocketIOStruct.SocketIOResponseMessage<bool>();
            try
            {
                _disconnectResponse = JsonConvert.DeserializeObject<List<SocketIOStruct.SocketIOResponseMessage<bool>>>(_response.ToString())[0];
            }
            catch (Exception ex)
            {
                Debug.LogError("MatchMachineCallback parse error: " + ex);
                AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                {
                    NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_handleFail);
                });
                OnLoadingCountSub?.Invoke();
                return;
            }

            switch (_disconnectResponse.status)
            {
                case "0x000":
                    AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                    {
                        OnLoadingCountSub?.Invoke();
                        _DisconnectSuccess?.Invoke();
                        OnUnPair();
                        status = "未連接";
                        OnStatusChange?.Invoke(status);

                    });
                    break;
                default:
                    AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                    {
                        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_handleFail);
                        OnLoadingCountSub?.Invoke();
                    });
                    break;
            }

        }, new { });


    }

    //====================================================================
    /// <summary>
    /// r解除綁定時停止監聽下列事件訊號
    /// <para>編輯者: Dimos</para>
    /// </summary>
    void OnUnPair()
    {
        SocketIOManager.Instance.SetSocketOnEvent("gameStart", false, GameStart);
        SocketIOManager.Instance.SetSocketOnEvent("gameResult", false, GameResult);
        SocketIOManager.Instance.SetSocketOnEvent("matchTimeout", false, MatchTimeout);
        SocketIOManager.Instance.SetSocketOnEvent("cancelMatch", false, CancelMatch);
        SocketIOManager.Instance.OnError -= SoceketError;
        SocketIOManager.Instance.OnReconnected -= OnReconnected;
        SocketIOManager.Instance.OnDisconnected -= OnDisconnected;
    }

    //====================================================================
    /// <summary>
    /// 繼續遊戲
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void ContinueMatch()
    {
        OnLoadingCountAdd?.Invoke();
        SocketIOManager.Instance.SetSocketEmitEventAsync("rematch", (_response) =>
        {
            Debug.Log("ContinueMatchCallback(): " + _response.ToString());
            SocketIOStruct.SocketIOResponseMessage<bool> _disconnectResponse = new SocketIOStruct.SocketIOResponseMessage<bool>();
            try
            {
                _disconnectResponse = JsonConvert.DeserializeObject<List<SocketIOStruct.SocketIOResponseMessage<bool>>>(_response.ToString())[0];
            }
            catch (Exception ex)
            {
                Debug.LogError("ContinueMatch parse error: " + ex);
                AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                {
                    NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_handleFail);
                });
                OnLoadingCountSub?.Invoke();
                return;
            }

            switch (_disconnectResponse.status)
            {
                case "0x000":
                    if (status == "遊戲中")
                    {
                        status = "等待遊戲";
                        OnStatusChange?.Invoke(status);
                    }
                    else
                    {
                        Debug.LogError("於錯誤狀態下接收到繼續遊戲的回傳: status=" + status);
                    }
                    break;
                default:
                    AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                    {
                        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_handleFail);
                        OnLoadingCountSub?.Invoke();
                    });
                    break;
            }
            OnLoadingCountSub?.Invoke();
        }, new { });
    }

    //==================================================================
    /// <summary>
    /// SocketIO中斷連線的觸發事件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    void OnDisconnected(object sender, string arg)
    {
        Debug.Log("Machine Disconnected");
        status = "未連接";
        //NotificationManager.Instance.CreateConfirmView(LocalizationManager.KeyTable.common_socketReconnect);
        OnStatusChange?.Invoke(status);
        OnUnPair();
    }

    //==================================================================
    /// <summary>
    /// SocketIO重連線的觸發事件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    void OnReconnected(object sender, int arg)
    {

    }
    //==================================================================
}
