using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
/// <summary>
/// 編輯收件人資料畫面
/// </summary>
public class RecipientEditView : MonoBehaviour
{
    private string phoneLocation_str; //國家 GEC代碼 e.g. TW, CN, etc.
    private string phoneNumber_str;
    private ResponseStruct.RecipientInfo_struct recipientData = new ResponseStruct.RecipientInfo_struct();

    [SerializeField] InputField name_inputField;
    [SerializeField] Text phone_text, phonePlaceHolder_text;
    [SerializeField] Dropdown country_dropDown; //縣市
    [SerializeField] Dropdown district_dropDown; //行政區
    [SerializeField] Text postalCodeUp_tx; //郵遞區號前三碼 只能由系統在選完行政區後自動填入
    [SerializeField] InputField postalCodeDown_inputField; //郵遞區號後三碼
    [SerializeField] InputField address_inputField;

    [SerializeField] MoveOut phoneEditView_moveOut;
    [SerializeField] Dropdown phoneTitle_dropDown;
    [SerializeField] InputField phoneNumber_inputField;

    //====================================================================
    /// <summary>
    /// 設定收件人資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Set(ResponseStruct.RecipientInfo_struct _recipientData)
    {
        recipientData = _recipientData;

        name_inputField.text = recipientData.name;

        if (recipientData.phone != null)
        {
            if (!string.IsNullOrEmpty(recipientData.phone.location) && !string.IsNullOrEmpty(recipientData.phone.number))
            {
                phoneLocation_str = recipientData.phone.location;
                phoneNumber_str = recipientData.phone.number;

                StringBuilder _fullPhoneNumber = new StringBuilder();
                _fullPhoneNumber
                    .Append("(").Append(CountryCode.GetCountryPhoneTitleByLocation(phoneLocation_str)).Append(") ")
                    .Append(phoneNumber_str);

                phone_text.text = _fullPhoneNumber.ToString();
                phonePlaceHolder_text.gameObject.SetActive(false);
            }
            else
            {
                phone_text.text = string.Empty;
                phonePlaceHolder_text.gameObject.SetActive(true);
            }
        }
        else
        {
            phone_text.text = string.Empty;
            phonePlaceHolder_text.gameObject.SetActive(true);
        }

        SetCountryOptions();

        if (!string.IsNullOrEmpty(recipientData.countyCity))
        {
            string[] _countryCity_arry = recipientData.countyCity.Split(' ');

            if (_countryCity_arry.Length == 2)
            {
                int _countyIndex = country_dropDown.options.FindIndex(0, (x) => x.text == _countryCity_arry[0]);
                if (_countyIndex > -1)
                {
                    country_dropDown.value = _countyIndex;
                }
                else
                {
                    Debug.LogWarning("RecipientEditView.Set() County can not find. County: " + _countryCity_arry[0]);
                }

                int _districtIndex = district_dropDown.options.FindIndex(0, (x) => x.text == _countryCity_arry[1]);
                if (_districtIndex > -1)
                {
                    district_dropDown.value = _districtIndex;
                }
                else
                {
                    Debug.LogWarning("RecipientEditView.Set() District can not find. County: " + _countryCity_arry[1]);
                }
            }
            else
            {
                Debug.LogWarning("RecipientEditView.Set() RecipientData.CountyCity format error. RecipientData.CountyCity: " + recipientData.countyCity);
                SetDistrictOptions(country_dropDown.value);
            }
        }
        else
        {
            SetDistrictOptions(country_dropDown.value);
        }

        if (!string.IsNullOrEmpty(recipientData.postalCode))
        {
            string[] _postalCode_arry = recipientData.postalCode.Split(' ');

            if (_postalCode_arry.Length == 2)
            {
                postalCodeDown_inputField.text = _postalCode_arry[1];
            }
            else
            {
                Debug.LogWarning("RecipientEditView.Set() RecipientData.PostalCode format error. RecipientData.PostalCode: " + recipientData.postalCode);
            }
        }

        address_inputField.text = recipientData.address;
    }

    //====================================================================
    /// <summary>
    /// 設定縣市的下拉選項
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void SetCountryOptions()
    {
        country_dropDown.options = GetDistricts.GetCountryOptions();
    }

    //====================================================================
    /// <summary>
    /// 設定鄉鎮的下拉選項
    /// <para>編輯者: Dimos</para>        
    /// <para>country_dropDown的Value改變時就會觸發</para>
    /// </summary>
    public void SetDistrictOptions(int _country_int)
    {
        var _options = GetDistricts.GetDistrictOptions(country_dropDown.options[_country_int].text);
        if (_options != null)
        {
            district_dropDown.options = _options;
            district_dropDown.value = 0;
            SetPostalCode();
        }
        else
        {
            Debug.LogWarning("RecipientEditView.SetDistrictOptions() Can not find district-options");
        }
    }

    //====================================================================
    /// <summary>
    /// 設定郵遞區號前3碼
    /// <para>編輯者: Dimos</para>
    /// <para>district_dropDown的Value改變時就會觸發</para>
    /// </summary>
    public void SetPostalCode()
    {
        string _postalCode = GetDistricts.GetDistrictZip(country_dropDown.captionText.text, district_dropDown.captionText.text);
        if (!string.IsNullOrEmpty(_postalCode))
        {
            postalCodeUp_tx.text = _postalCode;
        }
        else
        {
            postalCodeUp_tx.text = "Unknown";
            Debug.LogWarning("RecipientEditView.SetPostalCode() Can not find postal code");
        }
    }

    //====================================================================
    /// <summary>
    /// 開啟電話的編輯視窗
    /// <para>編輯者: Dimos</para>
    /// <para>載入電話資料後顯示編輯畫面</para>
    /// <para>添加返回事件</para>
    /// </summary>
    public void OpenPhoneEditView()
    {
        if (phoneTitle_dropDown.options.Count == 0)
        {
            List<Dropdown.OptionData> _phoneTitleOptions = CountryCode.GetAllPhoneTitleOptions();
            phoneTitle_dropDown.AddOptions(_phoneTitleOptions);
        }

        int _index = phoneTitle_dropDown.options.FindIndex((x) => x.text == SettingData.Data.phoneTitle);
        if (_index > -1)
        {
            phoneTitle_dropDown.value = _index;
        }
        else
        {
            Debug.LogWarning("RecipientEditView.EditPhone() SettingData's phoneTitle can not find. phoneTitle: " + SettingData.Data.phoneTitle);
        }

        phoneNumber_inputField.text = string.Empty;
        phoneEditView_moveOut.Move(true);

        UnityEngine.Events.UnityEvent _returnEvent = new UnityEngine.Events.UnityEvent();
        _returnEvent.AddListener(QuitPhoneEditView);
        ReturnManager.Instance.PushReturnEvent(_returnEvent);
    }

    //====================================================================
    /// <summary>
    /// 設定電話資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void SetPhone()
    {
        if (!string.IsNullOrEmpty(phoneNumber_inputField.text))
        {
            phoneLocation_str = CountryCode.GetCountryLocationByPhoneTitle(phoneTitle_dropDown.captionText.text);
            phoneNumber_str = phoneNumber_inputField.text;

            StringBuilder _fullPhoneNumber = new StringBuilder();
            _fullPhoneNumber
                .Append("(").Append(CountryCode.GetCountryPhoneTitleByLocation(phoneLocation_str)).Append(") ")
                .Append(phoneNumber_str);

            phone_text.text = _fullPhoneNumber.ToString();
            phonePlaceHolder_text.gameObject.SetActive(false);
            QuitPhoneEditView();
        }
        else
        {
            NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_phoneNumberIsEmpty);
        }
    }

    //====================================================================
    /// <summary>
    /// 關閉電話的編輯畫面
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void QuitPhoneEditView()
    {
        phoneEditView_moveOut.Move(false);
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
    private bool canSendRequest_bl = true;

    /// <summary>
    /// 初步檢查資料是否正確然後向RecipientManager設定收件人資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void SendSetRecipientRequest()
    {
        if (canSendRequest_bl)
        {
            canSendRequest_bl = false;

            if (string.IsNullOrEmpty(name_inputField.text))
            {
                NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.mall_recipientNameEmptyError);
                canSendRequest_bl = true;
            }
            else if (string.IsNullOrEmpty(phoneNumber_str))
            {
                NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_phoneNumberIsEmpty);
                canSendRequest_bl = true;
            }
            else if (string.IsNullOrEmpty(address_inputField.text))
            { 
                NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.mall_recipientAddressEmptyError);
                canSendRequest_bl = true;
            }
            else
            {
                RequestStruct.RecipientInformation_struct _recipientInfo = new RequestStruct.RecipientInformation_struct()
                {
                    name = name_inputField.text,
                    phone = new DataStruct.PhoneFormat_struct()
                    {
                        location = phoneLocation_str,
                        number = phoneNumber_str
                    },
                    countyCity = country_dropDown.captionText.text + " " + district_dropDown.captionText.text,
                    address = address_inputField.text,
                };

                if (string.IsNullOrEmpty(postalCodeDown_inputField.text))
                {
                    _recipientInfo.postalCode = postalCodeUp_tx.text;
                }
                else
                {
                    _recipientInfo.postalCode = postalCodeUp_tx.text + " " + postalCodeDown_inputField.text;
                }

                RecipientManager.Instance.SetRecipientInformation(recipientData.id, _recipientInfo);
            }
        }
    }

    //====================================================================
    /// <summary>
    /// 解除送出資料的鎖定
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void UnlockRequest()
    {
        canSendRequest_bl = true;
    }

    //====================================================================
    /// <summary>
    /// 關閉畫面
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void QuitView()
    {
        Destroy(this.gameObject);
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
}
