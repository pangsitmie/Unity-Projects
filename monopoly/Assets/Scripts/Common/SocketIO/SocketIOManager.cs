using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using SocketIOClient.Transport;
using Newtonsoft.Json;
public class SocketIOManager : MonoBehaviour
{

    public static class SocketIOSetting
    {
        public static string socketIOUrl = "https://nfctest.cloudprogrammingonline.com/machine";
        //public static string socketIOUrl = "http://192.168.0.107:3000/machine";
        public static int EIO = 4;  // this is SocketIO's component version
        public static string Path = "/socket.io";
        public static bool Reconnection = true;
        public static string Error = null;
    }
    static SocketIOManager instance = null;

    public static SocketIOManager Instance { get => instance; }
    public bool Connected { get => mainSocketIO.Connected; }
    [SerializeField] List<string> socketOnEvent_list = new List<string>();
    SocketIO mainSocketIO = new SocketIO(
        SocketIOManager.SocketIOSetting.socketIOUrl,
        new SocketIOOptions
        {
            Path = SocketIOManager.SocketIOSetting.Path,
            EIO = SocketIOManager.SocketIOSetting.EIO,
            Reconnection = SocketIOManager.SocketIOSetting.Reconnection,
            ExtraHeaders = new Dictionary<string, string>()
        }
    );
    public event EventHandler OnConnected;
    public event EventHandler<string> OnDisconnected;
    public event EventHandler OnReconnectFailed;
    public event EventHandler<int> OnReconnected;
    public event EventHandler<int> OnReconnectAttempt;
    public event EventHandler<Exception> OnReconnectError;
    //public event EventHandler<SocketIOClient.EventArguments.ReceivedEventArgs> OnReceivedEvent;
    public event EventHandler<string> OnSystemError;
    public delegate void SocketIOResponseEventHandler(SocketIOResponse _socketIOResponse);
    public event SocketIOResponseEventHandler OnError;
    public event EventHandler OnPing;
    void Awake()
    {
        instance = this;
        var jsonSerializer = new NewtonsoftJsonSerializer();
        mainSocketIO.JsonSerializer = jsonSerializer;
    }
    private async void Start()
    {
    }
    public async Task ConnectRetry()
    {

        try
        {
            await Connect();
        }
        catch (System.Exception a)
        {
            Debug.Log("SocketIO Connect() failed: " + a.Message);
            ConnectRetry();
        }
    }
    //==================================================================
    /// <summary>
    /// SocketIO 連線
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public async Task Connect()
    {
        if (mainSocketIO.Options.ExtraHeaders.ContainsKey("authorization"))
        {
            mainSocketIO.Options.ExtraHeaders["authorization"] = "Bearer " + SettingData.Data.token;
        }
        else
        {
            mainSocketIO.Options.ExtraHeaders.Add("authorization", "Bearer " + SettingData.Data.token);
        }
        Initial();
        await mainSocketIO.ConnectAsync();
    }

    //==================================================================
    /// <summary>
    /// SocketIO 斷線
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public async Task DisConnect()
    {
        await mainSocketIO.DisconnectAsync();
    }

    //==================================================================
    /// <summary>
    /// SocketIO 初始化
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Initial()
    {
        socketOnEvent_list.Clear();
        SetSocketOnEvent("error", true, (_response) =>
        {
            Debug.LogError("SocketIO Error: " + _response.ToString());
            OnError?.Invoke(_response);
        });
        mainSocketIO.OnPing += OnPingCheck;
        mainSocketIO.OnPong += OnPongCheck;
        mainSocketIO.OnConnected += OnConnectedCheck;
        mainSocketIO.OnDisconnected += OnDisconnectedCheck;
        mainSocketIO.OnReconnectFailed += OnReconnectFailedCheck;
        mainSocketIO.OnReconnected += OnReconnectedCheck;
        mainSocketIO.OnReconnectAttempt += OnReconnectAttemptCheck;
        mainSocketIO.OnReconnectError += OnReconnectErrorCheck;
        mainSocketIO.OnError += OnErrorCheck;
    }

    //==================================================================
    /// <summary>
    /// 設置SocketIO的監聽事件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void SetSocketOnEvent(string _eventName_str, bool _status_bl, System.Action<SocketIOClient.SocketIOResponse> _Response)
    {
        if (_status_bl == socketOnEvent_list.Contains(_eventName_str))
        {
            mainSocketIO.Off(_eventName_str);
            socketOnEvent_list.Remove(_eventName_str);
            Debug.Log(" SetSocketOnEvent: On eventName: " + _eventName_str);
            mainSocketIO.On(_eventName_str, _Response);
            socketOnEvent_list.Add(_eventName_str);
        }
        else
        {
            if (_status_bl == false)
            {
                Debug.Log(" SetSocketOnEvent: Off eventName: " + _eventName_str);
                mainSocketIO.Off(_eventName_str);
                socketOnEvent_list.Remove(_eventName_str);
            }
            else
            {
                Debug.Log(" SetSocketOnEvent: On eventName: " + _eventName_str);
                mainSocketIO.On(_eventName_str, _Response);
                socketOnEvent_list.Add(_eventName_str);
            }
        }
    }

    //==================================================================
    /// <summary>
    /// 設置SocketIO的發送事件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public async void SetSocketEmitEventAsync(string _eventName_str, System.Action<SocketIOClient.SocketIOResponse> _Callback, params object[] _args_obj)
    {
        Debug.Log(" SetSocketEmitEventAsync: eventName: " + _eventName_str + " args: " + JsonConvert.SerializeObject(_args_obj));

        if (_Callback != null)
        {
            await mainSocketIO.EmitAsync(_eventName_str,
            (_response) =>
            {
                Debug.Log("SocketEmit Response: " + _response.ToString());
                _Callback.Invoke(_response);
            }, _args_obj);
        }
        else
        {
            await mainSocketIO.EmitAsync(_eventName_str, _args_obj);
        }
    }

    //==================================================================
    /// <summary>
    /// SocketIO連線到的觸發事件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    void OnConnectedCheck(object sender, EventArgs arg)
    {
        Debug.Log("SocketIO Connected");
        //DisConnect();
        OnConnected?.Invoke(sender, arg);
    }

    //==================================================================
    /// <summary>
    /// SocketIO中斷連線的觸發事件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    void OnDisconnectedCheck(object sender, string arg)
    {
        Debug.LogWarning("SocketIO Disconnected: " + arg);
        OnDisconnected?.Invoke(sender, arg);
    }

    //==================================================================
    /// <summary>
    /// SocketIO重連失敗的觸發事件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    void OnReconnectFailedCheck(object sender, EventArgs arg)
    {
        Debug.Log("SocketIO ReconnectFailed");
        OnReconnectFailed?.Invoke(sender, arg);
    }

    //==================================================================
    /// <summary>
    /// SocketIO重連錯誤的觸發事件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    void OnReconnectErrorCheck(object sender, Exception arg)
    {
        Debug.Log("SocketIO ReconnectFailed");
        OnReconnectError?.Invoke(sender, arg);
    }

    //==================================================================
    /// <summary>
    /// SocketIO重連的觸發事件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    void OnReconnectedCheck(object sender, int arg)
    {
        Debug.Log("SocketIO Reconnecting: " + arg.ToString());
        OnReconnected?.Invoke(sender, arg);
    }
    //==================================================================
    /// <summary>
    /// SocketIO重連的觸發事件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    void OnReconnectAttemptCheck(object sender, int arg)
    {
        Debug.Log("OnReconnect AttemptCheck: " + arg.ToString());
        OnReconnected?.Invoke(sender, arg);
    }
    //==================================================================
    /// <summary>
    /// SocketIO所有訊息的的觸發事件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    //void OnReceivedEventCheck(object sender, SocketIOClient.EventArguments.ReceivedEventArgs args)
    //{
    //    Debug.Log("SocketIO ReceivedEvent: " + args.Response.ToString());
    //    OnReceivedEvent?.Invoke(sender, args);
    //}

    //==================================================================
    /// <summary>
    /// SocketIO錯誤的觸發事件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    void OnErrorCheck(object sender, string arg)
    {
        Debug.Log("SocketIO System Error: " + arg);
        OnSystemError?.Invoke(sender, arg);
    }

    //==================================================================
    /// <summary>
    /// SocketIO ping的觸發事件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    void OnPingCheck(object sender, EventArgs arg)
    {
        Debug.Log("SocketIO Ping");
        OnPing?.Invoke(sender, arg);
    }
    //==================================================================
    /// <summary>
    /// SocketIO ping的觸發事件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    void OnPongCheck(object sender, TimeSpan arg)
    {
        Debug.Log("SocketIO Pong");
    }
}
