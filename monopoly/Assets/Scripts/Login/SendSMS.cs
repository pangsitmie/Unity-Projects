using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GraphQlClient.Core;
/// <summary>
/// 發送簡訊驗證碼
/// <para>僅用來發送簡訊驗證碼請求</para>
/// </summary>
public class SendSMS : MonoBehaviour
{
    private bool canSMS_bl = true;

    //====================================================================
    /// <summary>
    /// 發送簡訊驗證碼
    /// <para>編輯者: Dimos</para>
    /// <para>送入簡訊的請求資料，可帶 辨識碼的Text、LoadingObjectManager、Callback</para>
    /// </summary>
    public void SendSMSRequest(RequestStruct.SMSRequest_struct _SMSRequestData, LoadingObjectManager _loadingObjectManager = null, System.Action<string> _Callback = null)
    {
        if (canSMS_bl)
        {
            canSMS_bl = false;

            if (_loadingObjectManager != null)
                _loadingObjectManager.LoadingCountAdd();

            GraphApi.Query _graphTest = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName("sendSMS", GraphApi.Query.Type.Mutation);
            _graphTest.SetArgs(new { input = _SMSRequestData });
            StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphTest.query, null, (_responseData_str) =>
            {
                if (!string.IsNullOrEmpty(_responseData_str))
                {
                    try
                    {
                        var _response = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.SMSResponse_struct>>(_responseData_str);
                        if (_response.errors == null)
                        {

                            NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_verificationCodeSendSuccess);
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

                canSMS_bl = true;
                if (_loadingObjectManager != null)
                    _loadingObjectManager.LoadingCountSub();

                if (_Callback != null)
                    _Callback(_responseData_str);
            }));
        }
    }

    //====================================================================
}
