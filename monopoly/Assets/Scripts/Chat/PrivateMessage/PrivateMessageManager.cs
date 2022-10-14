using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphQlClient.Core;

public class PrivateMessageManager : MonoBehaviour
{
    private static PrivateMessageManager instance = null;
    public static PrivateMessageManager Instance { get => instance; }
    void Awake()
    {
        instance = this;
    }

    //====================================================================
    public void FetchLastPrivateMsg(System.Action<ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct[]> _ResultCallback)
    {
        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetQueryByName.fetchLastPrivateMsg, GraphApi.Query.Type.Query);
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, (_responseData_str) =>
        {
            if (!string.IsNullOrEmpty(_responseData_str))
            {
                try
                {
                    var _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.Chat.PrivateChat.FetchLastPrivateMsgResponce_struct>>(_responseData_str);

                    if (_responseData.errors == null)
                    {
                        _ResultCallback?.Invoke(_responseData.data.fetchLastPrivateMsg);
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
            _ResultCallback?.Invoke(null);
        }));
    }

    //====================================================================
    public void FetchPrivateChatRecord(int _target_id, System.Action<ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct[]> _ResultCallback)
    {
        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetQueryByName.fetchPrivateChatRecord, GraphApi.Query.Type.Query);
        _graphQuery.SetArgs(new { targetId = _target_id });
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, (_responseData_str) =>
        {
            if (!string.IsNullOrEmpty(_responseData_str))
            {
                try
                {
                    var _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.Chat.PrivateChat.FetchPrivateChatRecord_struct>>(_responseData_str);

                    if (_responseData.errors == null)
                    {
                        _ResultCallback?.Invoke(_responseData.data.fetchPrivateChatRecord);
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
            _ResultCallback?.Invoke(null);
        }));
    }

    //====================================================================
    public void SendPrivateMsg(int _target_id, string _message_str, System.Action<ResponseStruct.Chat.PrivateChat.PrivateChatRecord_struct> _ResultCallback)
    {
        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetMutationByName.sendPrivateMsg, GraphApi.Query.Type.Mutation);
        _graphQuery.SetArgs(new { targetId = _target_id, message = _message_str });
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, (_responseData_str) =>
        {
            // 檢查回傳是否為空
            if (string.IsNullOrEmpty(_responseData_str))
            {
                _ResultCallback?.Invoke(null);
                return;
            }

            // 檢查回傳格式是否成功解析
            ResponseStruct.ResponseMessage<ResponseStruct.Chat.PrivateChat.SendPrivateMsgResponce_struct> _responseData = new ResponseStruct.ResponseMessage<ResponseStruct.Chat.PrivateChat.SendPrivateMsgResponce_struct>();
            try
            {
                _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.Chat.PrivateChat.SendPrivateMsgResponce_struct>>(_responseData_str);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex, this);
                _ResultCallback?.Invoke(null);
                return;
            }

            // 檢查是否回傳錯誤
            if (_responseData.errors != null)
            {
                switch (_responseData.errors[0].extensions.code)
                {
                    default:
                        GraphQLManager.ErrorCodeHandle(_responseData.errors[0]);
                        break;
                }
                _ResultCallback?.Invoke(null);
                return;
            }

            // 回傳成功
            _ResultCallback?.Invoke(_responseData.data.sendPrivateMsg);

        }));
    }
}
