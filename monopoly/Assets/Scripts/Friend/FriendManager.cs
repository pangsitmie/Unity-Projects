using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphQlClient.Core;
using System;
public class FriendManager : MonoBehaviour
{
    //====================================================================
    private static FriendManager instance = null;
    public static FriendManager Instance { get => instance; }

    //====================================================================
    void Awake()
    {
        instance = this;
    }

    //====================================================================
    /// <summary>
    /// 取得好友列表
    /// </summary>
    /// <param name="_ResultCallback">取得好友列表的回調</param>
    public void GetFriendList(Action<ResponseStruct.OtherMemberInfo_struct[]> _ResultCallback)
    {
        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetQueryByName.friendList, GraphApi.Query.Type.Query);
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, (_responseData_str) =>
        {
            if (!string.IsNullOrEmpty(_responseData_str))
            {
                try
                {
                    var _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.FriendListResponce_struct>>(_responseData_str);

                    if (_responseData.errors == null)
                    {
                        _ResultCallback?.Invoke(_responseData.data.friendList);
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
    /// <summary>
    /// 以電話搜尋用戶資料
    /// </summary>
    /// <param name="_phone">目標之電話號碼</param>
    /// <param name="_ResultCallback">搜尋結果的回調</param>
    public void PlayerSearch(DataStruct.PhoneFormat_struct _phone, Action<ResponseStruct.OtherMemberInfo_struct> _ResultCallback)
    {
        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetQueryByName.searchPlayers, GraphApi.Query.Type.Query);
        _graphQuery.SetArgs(new { input = new { phone = _phone } });
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, (_responseData_str) =>
        {
            if (!string.IsNullOrEmpty(_responseData_str))
            {
                try
                {
                    var _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.PlayerSearchResponce_struct>>(_responseData_str);

                    if (_responseData.errors == null)
                    {
                        _ResultCallback?.Invoke(_responseData.data.searchPlayers[0]);
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
            return;
        }));
    }

    //====================================================================
    /// <summary>
    /// 添加好友
    /// </summary>
    /// <param name="_id">欲添加好友的MemberId</param>
    /// <param name="_ResultCallback">添加好友是否成功的回調</param>
    public void AddFriend(int _id, Action<bool> _ResultCallback)
    {
        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetMutationByName.sendFriendRequest, GraphApi.Query.Type.Mutation);
        _graphQuery.SetArgs(new { targetId = _id });
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, (_responseData_str) =>
        {
            if (!string.IsNullOrEmpty(_responseData_str))
            {
                try
                {
                    var _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<bool>>(_responseData_str);

                    if (_responseData.errors == null)
                    {
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
            return;
        }));
    }

    //====================================================================
    /// <summary>
    /// 移除好友
    /// </summary>
    /// <param name="_id">欲移除好友的MemberId</param>
    /// <param name="_ResultCallback">移除好友是否成功的回調</param>
    public void RemoveFriend(int _id, Action<bool> _ResultCallback)
    {
        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetMutationByName.removeFriend, GraphApi.Query.Type.Mutation);
        _graphQuery.SetArgs(new { targetId = _id });
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, (_responseData_str) =>
        {
            if (!string.IsNullOrEmpty(_responseData_str))
            {
                try
                {
                    var _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<bool>>(_responseData_str);

                    if (_responseData.errors == null)
                    {
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
            return;
        }));
    }


    //====================================================================
    /// <summary>
    /// 取得好友邀請列表
    /// </summary>
    /// <param name="_ResultCallback">取得好友邀請列表的回調</param>
    public void GetFriendInviteList(Action<ResponseStruct.OtherMemberInfo_struct[]> _ResultCallback)
    {
        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetQueryByName.friendInviteList, GraphApi.Query.Type.Query);
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, (_responseData_str) =>
        {
            if (!string.IsNullOrEmpty(_responseData_str))
            {
                try
                {
                    var _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.FriendInviteListResponce_struct>>(_responseData_str);

                    if (_responseData.errors == null)
                    {
                        _ResultCallback?.Invoke(_responseData.data.friendInviteList);
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
    /// <summary>
    /// 接受好友邀請
    /// </summary>
    /// <param name="_id">欲接受好友邀請的Id</param>
    /// <param name="_ResultCallback">接受好友邀請是否成功的回調</param>
    public void AcceptFriendInvite(int _id, Action<bool> _ResultCallback)
    {
        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetMutationByName.acceptFriendRequest, GraphApi.Query.Type.Mutation);
        _graphQuery.SetArgs(new { initiatorId = _id });
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, (_responseData_str) =>
        {
            if (!string.IsNullOrEmpty(_responseData_str))
            {
                try
                {
                    var _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<bool>>(_responseData_str);

                    if (_responseData.errors == null)
                    {
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
            return;
        }));
    }

    //====================================================================
    /// <summary>
    /// 拒絕好友邀請
    /// </summary>
    /// <param name="_id">欲拒絕好友邀請的Id</param>
    /// <param name="_ResultCallback">拒絕好友邀請是否成功的回調</param>
    public void RefuseFriendInvite(int _id, Action<bool> _ResultCallback)
    {
        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetMutationByName.removeFriendRequest, GraphApi.Query.Type.Mutation);
        _graphQuery.SetArgs(new { initiatorId = _id });
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, (_responseData_str) =>
        {
            if (!string.IsNullOrEmpty(_responseData_str))
            {
                try
                {
                    var _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<bool>>(_responseData_str);

                    if (_responseData.errors == null)
                    {
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
            return;
        }));
    }

    //====================================================================
}
