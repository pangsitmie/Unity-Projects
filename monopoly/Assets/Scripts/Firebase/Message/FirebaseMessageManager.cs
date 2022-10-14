using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using GraphQlClient.Core;
/// <summary>
/// Firebase的Message相關功能管理
/// </summary>
public class FirebaseMessageManager : MonoBehaviour
{
    //====================================================================
    static FirebaseMessageManager instance = null;
    public static FirebaseMessageManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FirebaseMessageManager>();
                if (instance == null)
                {
                    var obj = GameObject.Find("FirebaseMessageManager");
                    if (obj == null) obj = Instantiate<GameObject>(new GameObject());
                    obj.name = "FirebaseMessageManager";
                    obj.AddComponent<DontDestroy>();
                    instance = obj.AddComponent<FirebaseMessageManager>();
                    Debug.Log("FirebaseMessageManager Created");
                }
            }
            return instance;
        }
    }



    //====================================================================
    public System.Action<Firebase.Messaging.MessageReceivedEventArgs> OnMessageReceivedEvent;
    public System.Action<Firebase.Messaging.TokenReceivedEventArgs> OnTokenReceivedEvent;

    //====================================================================
    public class TypeTable
    {
        public const string friendRequest = "friendRequest";
        public const string privateMessage = "privateMessage";
    }
    public class KeyTable
    {
        public const string type = "type";
    }
    //====================================================================
    public async void Start()
    {

        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        this.gameObject.AddComponent<FriendInviteFirebaseHandler>();
        this.gameObject.AddComponent<PrivateMessageFirebaseHandler>();
        UnityEngine.Debug.Log("Firebase.Start() Received Event Add Complete");
        try
        {
            var _firebaseToken = await Firebase.Messaging.FirebaseMessaging.GetTokenAsync();
            if (SettingData.Data.firebase != _firebaseToken)
            {
                SettingData.Data.firebase = _firebaseToken;
                SettingData.UpdateData();

                UpdateRegistrationToken(SettingData.Data.firebase, (_response) =>
                {
                    if (_response)
                    {
                        Debug.Log("SettingData Firebase Token: " + SettingData.Data.firebase);
                    }
                    else
                    {
                        Debug.Log("firebaseToken UpdateFail");
                    }
                });

            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }



    }
    //====================================================================
    /// <summary>
    /// 取得好友列表
    /// </summary>
    /// <param name="_ResultCallback">取得好友列表的回調</param>
    public void UpdateRegistrationToken(string _firebaseToken_str, System.Action<bool> _ResultCallback)
    {
        //如果token為空，則代表尚未登入，不需更新FirebaseToken;
        if (string.IsNullOrEmpty(SettingData.Data.token))
        {
            _ResultCallback?.Invoke(false);
            return;
        }

        //請求Server更新Firebase Token;
        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetMutationByName.updateRegistrationToken, GraphApi.Query.Type.Mutation);
        _graphQuery.SetArgs(new { registrationToken = _firebaseToken_str });
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, (_responseData_str) =>
        {
            if (!string.IsNullOrEmpty(_responseData_str))
            {
                try
                {
                    var _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.UpdateFirebaseTokenResponce_struct>>(_responseData_str);

                    if (_responseData.errors == null)
                    {
                        string _registrationToken = _responseData.data.updateRegistrationToken.registrationToken;
                        if (!string.IsNullOrEmpty(_registrationToken))
                        {
                            SettingData.Data.token = _registrationToken;
                            SettingData.UpdateData();
                        }
                        _ResultCallback?.Invoke(true);
                        return;
                    }
                    else
                    {
                        switch (_responseData.errors[0].extensions.code)
                        {
                            default:
                                GraphQLManager.ErrorCodeHandle(_responseData.errors[0]);
                                break;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex, this);
                }
            }
            _ResultCallback?.Invoke(false);
        }));
    }

    //====================================================================
    /// <summary>
    /// Token更新的觸發事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="token"></param>
    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Firebase.OnTokenReceived() Received Registration Token: " + token.Token);
        SettingData.Data.firebase = token.Token;
        UpdateRegistrationToken(SettingData.Data.firebase, (_response) =>
        {
            if (_response)
            {
                UnityEngine.Debug.Log("SettingData Firebase Token: " + SettingData.Data.firebase);
            }
            else
            {
                Debug.Log("firebaseToken UpdateFail");
            }
        });
        SettingData.UpdateData();
        OnTokenReceivedEvent?.Invoke(token);

    }

    //====================================================================
    /// <summary>
    /// 接收到Firebase訊息的事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        StringBuilder stringBuilder_strbd = new StringBuilder();
        stringBuilder_strbd.Clear();
        stringBuilder_strbd.Append("Firebase.OnMessageReceived() Received a new message \n")
                           .Append("\t\tfrom: ").Append(e.Message.From).Append("\n");
        stringBuilder_strbd.Append("\n");
        try
        {
            stringBuilder_strbd.Append("\t\tData: ").Append(JsonConvert.SerializeObject(e.Message.Data)).Append("\n");
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex, this);
            stringBuilder_strbd.Append("\t\tData: ").Append("Parse Error\n");
        }
        stringBuilder_strbd.Append("\n");
        try
        {
            stringBuilder_strbd.Append("\t\tNotification: \n");
            stringBuilder_strbd.Append("\t\t\tTitle: ").Append(e.Message.Notification.Title).Append("\n");
            stringBuilder_strbd.Append("\t\t\tBody: ").Append(e.Message.Notification.Body).Append("\n");
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex, this);
            stringBuilder_strbd.Append("\t\tNotification: ").Append("Parse Error\n");
        }

        UnityEngine.Debug.Log(stringBuilder_strbd.ToString());
        OnMessageReceivedEvent?.Invoke(e);
    }

    //====================================================================
    void OnDestroy()
    {
        Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
    }

    //====================================================================
}
