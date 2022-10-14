using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using GraphQlClient.Core;
/// <summary>
/// 
/// </summary>
public class NicknameEditView : MonoBehaviour
{
    [SerializeField] InputField nickname_inputField;
    [SerializeField] LoadingObjectManager loadingObjectManager;
    [SerializeField] MoveOut this_moveOut;

    //====================================================================
    /// <summary>
    /// 開啟畫面
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void OpenView()
    {
        this_moveOut.Move(true);

        var _closeEvent = new UnityEvent();
        _closeEvent.AddListener(CloseView);
        ReturnManager.Instance.PushReturnEvent(_closeEvent);
    }

    //====================================================================
    /// <summary>
    /// 關閉畫面
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void CloseView()
    {
        ReturnManager.Instance.PopReturnEvent();

        this_moveOut.Move(false);

        nickname_inputField.text = "";
    }

    //====================================================================

    private bool canRequest_bl = true;

    /// <summary>
    /// 確認修改暱稱
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void EditNickname()
    {
        if (canRequest_bl)
        {
            canRequest_bl = false;

            loadingObjectManager.LoadingCountAdd();

            string _nickname = nickname_inputField.text;

            if (_nickname.Length < 4 || _nickname.Length > 20)
            {
                NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_nicknameLengthError);

                loadingObjectManager.LoadingCountSub();

                canRequest_bl = true;
            }
            else
            {
                GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetMutationByName.modifyNickname, GraphApi.Query.Type.Mutation);
                _graphQuery.SetArgs(new { nickname = _nickname });
                StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, EditNicknameCallback));
            }
        }
    }

    //====================================================================
    /// <summary>
    /// 修改暱稱請求的回傳
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void EditNicknameCallback(string _responseData_str)
    {
        if (!string.IsNullOrEmpty(_responseData_str))
        {
            try
            {
                var _response = JsonUtility.FromJson<ResponseStruct.ResponseMessage<bool>>(_responseData_str);
                if (_response.errors == null)
                {
                    MemberInformationManager.Nickname_str = nickname_inputField.text;
                    NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_requestSuccess);
                    CloseView();
                }
                else
                {
                    switch (_response.errors[0].extensions.code)
                    {
                        default:
                            GraphQLManager.ErrorCodeHandle(_response.errors[0]);
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex, this);
            }
        }

        loadingObjectManager.LoadingCountSub();

        canRequest_bl = true;
    }

    //====================================================================
}
