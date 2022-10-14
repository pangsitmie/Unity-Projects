using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/// <summary>
/// 登入場景的初始化
/// </summary>
public class LoginInitialize : MonoBehaviour
{
    [SerializeField] Text versionCode_text;
    [SerializeField] Dropdown phoneTitle_dropdown;
    [SerializeField] InputField phoneNumber_inputField;
    [SerializeField] Toggle keepLogin_toggle;

    //====================================================================
    private void Start()
    {
        CheckPhoneNumber();
        InitPhoneTitle();
        InitKeepLoginToggle();
        versionCode_text.text = "V" + Application.version;
    }

    //====================================================================
    /// <summary>
    /// 初始化電話號碼(帳號)
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 根據設定檔紀錄填入電話號碼(帳號)</para>
    /// </summary>
    private void CheckPhoneNumber()
    {
        if (!string.IsNullOrEmpty(SettingData.Data.phoneNumber))
        {
            phoneNumber_inputField.text = SettingData.Data.phoneNumber;
        }
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
            Debug.LogWarning("LoginInitialize.InitPhoneTitle() SettingData's phoneTitle can not find");
        }
    }

    //====================================================================
    /// <summary>
    /// 初始化保持登入選項
    /// </summary>
    private void InitKeepLoginToggle()
    {
        keepLogin_toggle.isOn = SettingData.Data.keepLogin;
    }

    //====================================================================
    /// <summary>
    /// 切換是否保持登入
    /// </summary>
    /// <param name="_isOn_bl">true為保持登入，false為關閉保持登入</param>
    public void SwitchKeepLogin(bool _isOn_bl)
    {
        if (SettingData.Data.keepLogin != _isOn_bl)
        {
            SettingData.Data.keepLogin = _isOn_bl;
            SettingData.UpdateData();
        }
    }

    //====================================================================
    /// <summary>
    /// 更改設定檔的國家碼
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void SetCountryCode()
    {
        SettingData.Data.phoneTitle = phoneTitle_dropdown.captionText.text;
        SettingData.UpdateData();
    }

    //====================================================================
    private EventView closeView;

    /// <summary>
    /// 點擊返回鍵後的顯示與處理(此函式由ReturnEventHandler添加)
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void AddReturnEvent()
    {
        closeView = NotificationManager.Instance.CreateEventView(LocalizationManager.KeyTable.common_exitComfirmViewText, CloseViewConfirmCallback, CloseViewCancelCallback);
        UnityEvent _CloseViewReturnEvent = new UnityEvent();
        _CloseViewReturnEvent.AddListener(CloseViewCancelCallback);
        ReturnManager.Instance.PushReturnEvent(_CloseViewReturnEvent);
    }

    //====================================================================
    private void CloseViewConfirmCallback()
    {
        Application.Quit();
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
    private void CloseViewCancelCallback()
    {
        Destroy(closeView.gameObject);
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
}
