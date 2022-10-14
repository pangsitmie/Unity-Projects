using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GraphQlClient.Core;
/// <summary>
/// 負責註冊帳號畫面的各種處理
/// </summary>
public class SignUpObject : MonoBehaviour
{
    [SerializeField] Dropdown phoneTitle_dropdown;
    [SerializeField] InputField phoneNumber_inputField;
    [SerializeField] InputField verificationCode_inputField;
    [SerializeField] InputField nickname_inputField;
    [SerializeField] InputField password_inputField, passwordAgain_inputField;

    [SerializeField] SendSMS this_SendSMS;
    private LoadingObjectManager loadingObjectManager;

    private bool canSignUp_bl = true;

    //====================================================================
    /// <summary>
    /// 初始化
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Init(LoadingObjectManager _loadingObjectManager)
    {
        loadingObjectManager = _loadingObjectManager;
        InitPhoneTitle();
    }

    //====================================================================
    /// <summary>
    /// 初始化電話國際碼選項
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void InitPhoneTitle()
    {
        List<Dropdown.OptionData> _phoneTitleOptions = CountryCode.GetAllPhoneTitleOptions();
        phoneTitle_dropdown.AddOptions(_phoneTitleOptions);

        int _index = _phoneTitleOptions.FindIndex((x) => x.text == SettingData.Data.phoneTitle);
        if (_index > -1)
        {
            phoneTitle_dropdown.value = _index;
        }
        else
        {
            Debug.LogWarning("SignUpObject.InitPhoneTitle() SettingData's phoneTitle can not find");
        }
    }

    //====================================================================
    /// <summary>
    /// 註冊帳號
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void SignUpRequest()
    {
        if (canSignUp_bl)
        {
            canSignUp_bl = false;
            loadingObjectManager.LoadingCountAdd();

            if (phoneNumber_inputField.text.Length <= 0 ||                                      //檢查電話號碼是否為空
                verificationCode_inputField.text.Length != 6 ||                                 //檢查簡訊驗證碼長度是否不為6
                nickname_inputField.text.Length < 4 || nickname_inputField.text.Length > 20 ||  //檢查暱稱長度是否不為1~20
                password_inputField.text != passwordAgain_inputField.text ||                    //檢查兩次輸入的密碼是否不同
                password_inputField.text.Length < 8 || password_inputField.text.Length > 20     //檢查密碼長度是否不為8~20
            )
            {
                if (phoneNumber_inputField.text.Length <= 0)
                    NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_phoneNumberIsEmpty);
                else if (verificationCode_inputField.text.Length != 6)
                    NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_verificationCodeLengthError);
                else if (nickname_inputField.text.Length < 4 || nickname_inputField.text.Length > 20)
                    NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_nicknameLengthError);
                else if (password_inputField.text != passwordAgain_inputField.text)
                    NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_passwordNotSameError);
                else if (password_inputField.text.Length < 8 || password_inputField.text.Length > 20)
                    NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_passwordLengthError);

                canSignUp_bl = true;
                loadingObjectManager.LoadingCountSub();
            }
            else
            {
                RequestStruct.SignUpRequest_struct _signUpRequestData = new RequestStruct.SignUpRequest_struct()
                {
                    phone = new DataStruct.PhoneFormat_struct()
                    {
                        location = CountryCode.GetCountryLocationByPhoneTitle(phoneTitle_dropdown.captionText.text),
                        number = phoneNumber_inputField.text,
                    },

                    password = password_inputField.text,
                    verificationCode = verificationCode_inputField.text,
                    nickname = nickname_inputField.text
                };
                GraphApi.Query _graphTest = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName("signUp", GraphApi.Query.Type.Mutation);
                _graphTest.SetArgs(_signUpRequestData);
                StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphTest.query, null, SignUpRequestCallback));
            }
        }
    }

    //====================================================================
    /// <summary>
    /// 註冊帳號請求的Callback
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void SignUpRequestCallback(string _data_str)
    {
        if (!string.IsNullOrEmpty(_data_str))
        {
            try
            {
                var _response = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.SMSResponse_struct>>(_data_str);
                if (_response.errors == null)
                {
                    NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_signUpSuccess);
                    CloseSignUpView();
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

        canSignUp_bl = true;
        loadingObjectManager.LoadingCountSub();
    }

    //====================================================================
    /// <summary>
    /// 發送簡訊驗證碼的請求
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void SendSMSRequest()
    {
        if (phoneNumber_inputField.text.Length <= 0) //檢查電話號碼是否為空
        {
            NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_phoneNumberIsEmpty);
        }
        else
        {
            RequestStruct.SMSRequest_struct _SMSRequestData = new RequestStruct.SMSRequest_struct()
            {
                phone = new DataStruct.PhoneFormat_struct()
                {
                    location = CountryCode.GetCountryLocationByPhoneTitle(phoneTitle_dropdown.captionText.text),
                    number = phoneNumber_inputField.text,
                },

                type = 0
            };
            this_SendSMS.SendSMSRequest(_SMSRequestData, loadingObjectManager, (_response) =>
            {
                try
                {
                    var _SendSMSResponse = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.SMSResponse_struct>>(_response);
                    verificationCode_inputField.text = _SendSMSResponse.data.sendSMS;
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.LogException(ex, this);
                }

            });
        }
    }

    //====================================================================
    /// <summary>
    /// 關閉註冊頁面
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void CloseSignUpView()
    {
        Destroy(this.gameObject);

        //Pop掉由Back_btn上的ReturnEventHandler.cs所Push的StartEvent
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
}
