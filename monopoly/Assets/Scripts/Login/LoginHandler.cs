using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GraphQlClient.Core;
/// <summary>
/// 登入處理腳本
/// </summary>
public class LoginHandler : MonoBehaviour
{
    [SerializeField] LoadingObjectManager loadingObjectManager;
    [SerializeField] Dropdown phoneTitle_dropdown;
    [SerializeField] InputField phoneNumber_inputField;
    [SerializeField] InputField password_inputField;

    private bool canLogin_bl = true;

    //====================================================================
    /// <summary>
    /// 登入資料檢查
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 登入會先呼叫此函式，經由此函式送出請求，等待伺服器回傳後會觸發LoginCallback()</para>
    /// </summary>
    public void Login()
    {
        if (canLogin_bl)
        {
            canLogin_bl = false;
            loadingObjectManager.LoadingCountAdd();

            if (phoneNumber_inputField.text.Length <= 0 ||
                password_inputField.text.Length < 8 || password_inputField.text.Length > 20)
            {
                if (phoneNumber_inputField.text.Length <= 0)
                    NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_phoneNumberIsEmpty);
                else if (password_inputField.text.Length < 8 || password_inputField.text.Length > 20)
                    NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_passwordLengthError);

                canLogin_bl = true;
                loadingObjectManager.LoadingCountSub();
            }
            else
            {
                RequestStruct.LoginRequest_struct _loginRequestData = new RequestStruct.LoginRequest_struct()
                {
                    phone = new DataStruct.PhoneFormat_struct()
                    {
                        location = CountryCode.GetCountryLocationByPhoneTitle(phoneTitle_dropdown.captionText.text),
                        number = phoneNumber_inputField.text,
                    },

                    password = password_inputField.text,
                    registrationToken = SettingData.Data.firebase
                };
                SettingData.Data.phoneNumber = _loginRequestData.phone.number;
                SettingData.UpdateData();

                GraphApi.Query _graphTest = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName("signIn", GraphApi.Query.Type.Mutation);
                _graphTest.SetArgs(_loginRequestData);
                StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphTest.query, null, LoginCallback));
            }
        }
    }

    //====================================================================
    /// <summary>
    /// 登入請求的Callback
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private async void LoginCallback(string _data_str)
    {
        if (!string.IsNullOrEmpty(_data_str))
        {
            try
            {
                var _response = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.LoginResponse_struct>>(_data_str);
                if (_response.errors == null)
                {
                    MemberInformationManager.Id = _response.data.signIn.id;
                    MemberInformationManager.Nickname_str = _response.data.signIn.nickname;
                    MemberInformationManager.Phone = _response.data.signIn.phone;
                    MemberInformationManager.Ticket_int = _response.data.signIn.lotteryTicket;
                    SettingData.Data.token = _response.data.signIn.token;
                    SettingData.UpdateData();
                    await SocketIOManager.Instance.ConnectRetry();
                    SceneController.Instance.ChangeScene(SceneController.SceneNameEnum.LoadingScene.ToString(), null, () =>
                    {
                        SceneController.Instance.ChangeScene(SceneController.SceneNameEnum.MainScene.ToString());
                    });
                }
                else
                {
                    switch (_response.errors[0].extensions.code)
                    {
                        default:
                            GraphQLManager.ErrorCodeHandle(_response.errors[0]);
                            break;
                    }

                    canLogin_bl = true;
                    loadingObjectManager.LoadingCountSub();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex, this);
                canLogin_bl = true;
                loadingObjectManager.LoadingCountSub();
            }
        }
        else
        {
            canLogin_bl = true;
            loadingObjectManager.LoadingCountSub();
        }
    }

    //====================================================================
}
