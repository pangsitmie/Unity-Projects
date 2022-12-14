using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendListHandler : MonoBehaviour
{
    //====================================================================
    [SerializeField] FriendObject friendObject_pf;
    [SerializeField] FriendInviteObject friendInviteObject_pf;
    [SerializeField] MemberInfoUI memberInfoUI_pf;
    [SerializeField] FriendListUI friendListUI_pf;
    [SerializeField] PhoneFormatEditView phoneFormatEditView_pf;


    //====================================================================
    public System.Action OnListRefresh;
    public System.Action OnRequestCompleted;
    public System.Action<ResponseStruct.OtherMemberInfo_struct> OnOpenPrivateChatMessageView;

    //====================================================================
    [SerializeField] LoadingObjectManager loadingObjectManager;
    [SerializeField] GameObject inviteHint_gameObject;

    //====================================================================
    ResponseStruct.OtherMemberInfo_struct[] friendInfo_arry;
    public ResponseStruct.OtherMemberInfo_struct[] FriendInfo_arry { get => friendInfo_arry; }
    ResponseStruct.OtherMemberInfo_struct[] friendInviteInfo_arry;
    public ResponseStruct.OtherMemberInfo_struct[] FriendInviteInfo_arry { get => friendInviteInfo_arry; }


    List<FriendObject> friendObjects_list = new List<FriendObject>();
    List<FriendInviteObject> friendInviteObjects_list = new List<FriendInviteObject>();


    //====================================================================
    MemberInfoUI memberInfoUI;
    FriendListUI friendListUI;
    PhoneFormatEditView phoneFormatEditView;


    //====================================================================
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

    //====================================================================
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        RefreshFriendInviteList();
        FriendInviteFirebaseHandler.Instance.OnFriendInvite += FirebaseRefreshFriendInvite;
    }

    //====================================================================
    /// <summary>
    /// ???Firebase??????????????????????????????
    /// </summary>
    void FirebaseRefreshFriendInvite()
    {
        RefreshFriendInviteList();
    }

    //====================================================================
    /// <summary>
    /// ????????????????????????
    /// </summary>
    void RefreshFriendInviteList()
    {
        FriendManager.Instance.GetFriendInviteList((_result) =>
        {
            friendInviteInfo_arry = _result;
            CheckInviteHint();
        });
    }
    //====================================================================
    /// <summary>
    /// ???Button???????????????????????????
    /// </summary>
    public void OpenFriendListView()
    {
        loadingObjectManager.LoadingCountAdd();
        if (friendListUI == null)
        {
            friendListUI = Instantiate(friendListUI_pf, SceneController.Instance.CurrentSceneCanvas.transform);
            friendListUI.LoadAndShowView(this);
        }
        else
        {
            friendListUI.LoadAndShowView(this);
        }

    }

    //====================================================================
    /// <summary>
    /// ??????????????????????????????
    /// </summary>
    /// <param name="_Callback">????????????????????????</param>
    public void GetFriendList(System.Action _Callback)
    {
        RequestCount_int++;
        FriendManager.Instance.GetFriendList((_result) =>
        {
            RequestCount_int--;
            friendInfo_arry = _result;
            _Callback?.Invoke();
        });
    }

    //====================================================================
    /// <summary>
    /// ????????????????????????????????????
    /// </summary>
    /// <param name="_Callback">????????????????????????</param>
    public void GetFriendInviteList(System.Action _Callback)
    {
        RequestCount_int++;
        FriendManager.Instance.GetFriendInviteList((_result) =>
        {
            RequestCount_int--;
            friendInviteInfo_arry = _result;
            CheckInviteHint();
            _Callback?.Invoke();
        });
    }

    //====================================================================
    /// <summary>
    /// ?????????????????????
    /// </summary>
    /// <param name="_content_transform">???????????????????????????</param>
    /// <param name="_emptyHint_gObj">??????????????????????????????</param>
    public void BuildFriendList(Transform _content_transform, GameObject _emptyHint_gObj)
    {
        BuildFriendList(friendInfo_arry, _content_transform, _emptyHint_gObj, ref friendObjects_list);
    }
    //====================================================================
    /// <summary>
    /// ???????????????????????????
    /// </summary>
    /// <param name="_content_transform">?????????????????????????????????</param>
    /// <param name="_emptyHint_gObj">????????????????????????????????????</param>
    public void BuildFriendInviteList(Transform _content_transform, GameObject _emptyHint_gObj)
    {
        BuildFriendInviteList(friendInviteInfo_arry, _content_transform, _emptyHint_gObj, ref friendInviteObjects_list);
    }

    //====================================================================
    /// <summary>
    /// ???????????????????????????????????????????????????
    /// </summary>
    /// <param name="_friendObject">????????????????????????</param>
    public void ShowMemberInfo(FriendObject _friendObject)
    {
        if (memberInfoUI == null)
        {
            memberInfoUI = Instantiate(memberInfoUI_pf, SceneController.Instance.CurrentSceneCanvas.transform);
            memberInfoUI.LoadAndShowView(_friendObject, this);
        }
        else
        {
            memberInfoUI.LoadAndShowView(_friendObject, this);
        }
    }
    //====================================================================
    /// <summary>
    /// ??????????????????????????????????????????
    /// </summary>
    /// <param name="_ResultCallBack">?????????????????????????????????????????????????????????????????????????????????Null</param>
    public void OpenAddFriendInputView(System.Action<DataStruct.PhoneFormat_struct> _ResultCallBack)
    {
        if (phoneFormatEditView == null)
        {
            phoneFormatEditView = Instantiate(phoneFormatEditView_pf, SceneController.Instance.CurrentSceneCanvas.transform);
            phoneFormatEditView.Set(LocalizationManager.KeyTable.login_phoneNumberPlaceholder, _ResultCallBack, () => { _ResultCallBack(null); });
        }
        else
        {
            phoneFormatEditView.Set(LocalizationManager.KeyTable.login_phoneNumberPlaceholder, _ResultCallBack, () => { _ResultCallBack(null); });
        }
    }
    //====================================================================
    /// <summary>
    /// ???????????????????????????
    /// </summary>
    /// <param name="_targetPhoneData">??????????????????????????????</param>
    /// <param name="_ResultCallBack">??????????????????????????????????????????Boolean???true????????????????????????????????????</param>
    public void AddFriendByPhoneNumber(DataStruct.PhoneFormat_struct _targetPhoneData, Action<bool> _ResultCallBack)
    {
        loadingObjectManager.LoadingCountAdd();
        FriendManager.Instance.PlayerSearch(_targetPhoneData, (_result) =>
        {
            if (_result == null)
            {
                NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_handleFail);
                _ResultCallBack?.Invoke(false);
                loadingObjectManager.LoadingCountSub();
            }
            else
            {
                FriendManager.Instance.AddFriend(_result.id, (_result) =>
                {
                    if (_result)
                    {
                        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_requestSuccess);
                        _ResultCallBack?.Invoke(true);
                        loadingObjectManager.LoadingCountSub();
                    }
                    else
                    {
                        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_handleFail);
                        _ResultCallBack?.Invoke(false);
                        loadingObjectManager.LoadingCountSub();
                    }

                });
            }
        });
    }

    //=================================================================
    public void OpenPrivateChatMessageView(ResponseStruct.OtherMemberInfo_struct _targetMemberInfo)
    {
        OnOpenPrivateChatMessageView?.Invoke(_targetMemberInfo);
        if (friendListUI != null)
            friendListUI.QuitView();
    }
    //====================================================================
    /// <summary>
    /// ????????????
    /// </summary>
    /// <param name="_friend_id">????????????????????????ID</param>
    /// <param name="_ResultCallBack">??????????????????????????????????????????Boolean???true????????????????????????????????????</param>
    public void RemoveFriend(int _friend_id, Action<bool> _ResultCallBack)
    {
        loadingObjectManager.LoadingCountAdd();
        FriendManager.Instance.RemoveFriend(_friend_id, (_result) =>
        {
            if (_result)
            {
                NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_requestSuccess);
                OnRequestCompleted += () =>
                {
                    _ResultCallBack?.Invoke(true);
                    loadingObjectManager.LoadingCountSub();
                    OnRequestCompleted = null;
                };
                RequestCount_int++;
                GetFriendList(() =>
                {
                    RequestCount_int--;
                });
            }
            else
            {
                NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_handleFail);
                _ResultCallBack?.Invoke(false);
                loadingObjectManager.LoadingCountSub();
            }
        });
    }

    //====================================================================
    /// <summary>
    /// ??????????????????
    /// </summary>
    /// <param name="_friend_id">????????????????????????ID</param>
    /// <param name="_ResultCallBack">????????????????????????????????????????????????Boolean???true??????????????????????????????????????????</param>
    public void AcceptFriendInvite(int _friend_id, Action<bool> _ResultCallBack)
    {
        loadingObjectManager.LoadingCountAdd();
        FriendManager.Instance.AcceptFriendInvite(_friend_id, (_result) =>
        {
            if (_result)
            {
                NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_requestSuccess);
                OnRequestCompleted += () =>
                {
                    _ResultCallBack?.Invoke(true);
                    loadingObjectManager.LoadingCountSub();
                    OnRequestCompleted = null;
                };

                RequestCount_int++;
                GetFriendInviteList(() =>
                {
                    RequestCount_int--;
                });
                RequestCount_int++;
                GetFriendList(() =>
                {
                    RequestCount_int--;
                });
            }
            else
            {
                NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_handleFail);
                _ResultCallBack?.Invoke(false);
                loadingObjectManager.LoadingCountSub();
            }
        });
    }

    //====================================================================
    /// <summary>
    /// ??????????????????
    /// </summary>
    /// <param name="_friend_id">????????????????????????ID</param>
    /// <param name="_ResultCallBack">????????????????????????????????????????????????Boolean???true??????????????????????????????????????????</param>
    public void RefuseFriendInvite(int _friend_id, Action<bool> _ResultCallBack)
    {
        loadingObjectManager.LoadingCountAdd();
        FriendManager.Instance.RefuseFriendInvite(_friend_id, (_result) =>
        {
            if (_result)
            {
                NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_requestSuccess);
                OnRequestCompleted += () =>
                {
                    _ResultCallBack?.Invoke(true);
                    loadingObjectManager.LoadingCountSub();
                    OnRequestCompleted = null;
                };

                RequestCount_int++;
                GetFriendInviteList(() =>
                {
                    RequestCount_int--;
                });
                RequestCount_int++;
                GetFriendList(() =>
                {
                    RequestCount_int--;
                });
            }
            else
            {
                NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_handleFail);
                _ResultCallBack?.Invoke(false);
                loadingObjectManager.LoadingCountSub();
            }
        });
    }

    //====================================================================
    /// <summary>
    /// ?????????????????????????????????
    /// </summary>
    /// <param name="_friendInfo_arry">???????????????????????????</param>
    /// <param name="_content_transform">???????????????????????????</param>
    /// <param name="_emptyHint_gObj">??????????????????????????????</param>
    /// <param name="_friendObjects_list">??????????????????????????????List</param>
    public void BuildFriendList(ResponseStruct.OtherMemberInfo_struct[] _friendInfo_arry, Transform _content_transform, GameObject _emptyHint_gObj, ref List<FriendObject> _friendObjects_list)
    {
        if (_friendInfo_arry != null)
        {
            if (_friendInfo_arry.Length <= _friendObjects_list.Count)
            {
                int _count = 0;

                for (; _count < _friendInfo_arry.Length; _count++)
                {
                    _friendObjects_list[_count].Set(_friendInfo_arry[_count], this);
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
                    _friendObjects_list[_count].Set(_friendInfo_arry[_count], this);
                }

                for (; _count < _friendInfo_arry.Length; _count++)
                {
                    var _orderObject = Instantiate(friendObject_pf, _content_transform);
                    _orderObject.Set(_friendInfo_arry[_count], this);
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

        _emptyHint_gObj.SetActive(_friendInfo_arry == null || _friendInfo_arry.Length == 0);
    }

    //====================================================================
    /// <summary>
    /// ?????????????????????????????????
    /// </summary>
    /// <param name="_friendInviteInfo_arry">???????????????????????????</param>
    /// <param name="_content_transform">???????????????????????????</param>
    /// <param name="_emptyHint_gObj">??????????????????????????????</param>
    /// <param name="_friendInviteObjects_list">??????????????????????????????List</param>
    public void BuildFriendInviteList(ResponseStruct.OtherMemberInfo_struct[] _friendInviteInfo_arry, Transform _content_transform, GameObject _emptyHint_gObj, ref List<FriendInviteObject> _friendInviteObjects_list)
    {
        if (_friendInviteInfo_arry != null)
        {
            if (_friendInviteInfo_arry.Length <= _friendInviteObjects_list.Count)
            {
                int _count = 0;

                for (; _count < _friendInviteInfo_arry.Length; _count++)
                {
                    _friendInviteObjects_list[_count].Set(_friendInviteInfo_arry[_count], this);
                }

                for (; _count < _friendInviteObjects_list.Count;)
                {
                    Destroy(_friendInviteObjects_list[_count].gameObject);
                    _friendInviteObjects_list.Remove(_friendInviteObjects_list[_count]);
                }
            }
            else
            {
                int _count = 0;

                for (; _count < _friendInviteObjects_list.Count; _count++)
                {
                    _friendInviteObjects_list[_count].Set(_friendInviteInfo_arry[_count], this);
                }

                for (; _count < _friendInviteInfo_arry.Length; _count++)
                {
                    var _orderObject = Instantiate(friendInviteObject_pf, _content_transform);
                    _orderObject.Set(_friendInviteInfo_arry[_count], this);
                    _friendInviteObjects_list.Add(_orderObject);
                }
            }
        }
        else
        {
            for (int i = 0; i < _friendInviteObjects_list.Count; i++)
            {
                Destroy(_friendInviteObjects_list[i].gameObject);
            }
            _friendInviteObjects_list.Clear();
        }

        _emptyHint_gObj.SetActive(_friendInviteInfo_arry == null || _friendInviteInfo_arry.Length == 0);
    }

    //====================================================================
    /// <summary>
    /// ????????????????????????????????????????????????????????????
    /// </summary>
    void CheckRequestCount()
    {
        if (requestCount_int <= 0)
        {
            requestCount_int = 0;
            OnListRefresh?.Invoke();
            OnRequestCompleted?.Invoke();
            loadingObjectManager.LoadingCountSub();
        }
    }

    //====================================================================
    /// <summary>
    /// ????????????????????????????????????
    /// </summary>
    void CheckInviteHint()
    {
        inviteHint_gameObject.SetActive(friendInviteInfo_arry.Length > 0);
    }

    //====================================================================
    void OnDestroy()
    {
        FriendInviteFirebaseHandler.Instance.OnFriendInvite -= FirebaseRefreshFriendInvite;
    }

    //====================================================================
}
