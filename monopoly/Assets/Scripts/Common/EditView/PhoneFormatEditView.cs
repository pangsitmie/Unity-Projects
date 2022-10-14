using System;
using System.Collections;
using System.Collections.Generic;
using DataStruct;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 需輸入電話號碼的視窗設定(PhoneEditView.prefab)
/// (目前使用在依電話搜尋好友的功能中)
/// </summary>
public class PhoneFormatEditView : MonoBehaviour
{
    //====================================================================
    [SerializeField] LocalizationText message_localText;
    [SerializeField] LocalizationText confirmButton_localText;
    [SerializeField] LocalizationText cancelButton_localText;
    [SerializeField] Dropdown phoneTitle_dropdown;
    [SerializeField] MoveOut phoneFormatEditView_moveOut;
    [SerializeField] InputField phoneNumber_inputField;


    //====================================================================
    private System.Action<DataStruct.PhoneFormat_struct> ConfirmEvent;
    private System.Action CancelEvent;


    //====================================================================
    DataStruct.PhoneFormat_struct phoneFormatedData;

    public PhoneFormat_struct PhoneFormatedData { get => phoneFormatedData; }


    //====================================================================
    /// <summary>
    /// 設定資料、初始化
    /// </summary>
    /// <param name="_title_key">視窗標題的文字</param>
    /// <param name="_ConfirmEvent">點擊UI確認按鍵時回調，會帶入電話資料</param>
    /// <param name="_CancelEvent">點擊UI取消按鍵時回調</param>
    /// <param name="_confirmButtonText_key">確認按鍵的文字，預設為"確認"</param>
    /// <param name="_cancelButtonText_key">取消按鍵的文字，預設為"取消"</param>
    public void Set(string _title_key,
                     System.Action<DataStruct.PhoneFormat_struct> _ConfirmEvent,
                     System.Action _CancelEvent,
                     string _confirmButtonText_key = LocalizationManager.KeyTable.common_confirm,
                     string _cancelButtonText_key = LocalizationManager.KeyTable.common_cancel)
    {
        InitPhoneTitle();
        phoneNumber_inputField.text = "";

        message_localText.Set(_title_key);
        confirmButton_localText.Set(_confirmButtonText_key);
        cancelButton_localText.Set(_cancelButtonText_key);

        ConfirmEvent = _ConfirmEvent;
        CancelEvent = _CancelEvent;

        phoneFormatEditView_moveOut.Move(true);
    }


    //====================================================================
    /// <summary>
    /// 初始化電話國際碼選項
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
            Debug.LogWarning("PhoneFormatEditView.InitPhoneTitle() SettingData's phoneTitle can not find");
        }
    }

    //====================================================================
    /// <summary>
    /// 檢查電話格式並儲存至phoneFormatedData
    /// </summary>
    public void CheckPhoneFormat()
    {
        phoneFormatedData = PhoneFormat();
    }

    //====================================================================
    /// <summary>
    /// 檢查電話格式
    /// </summary>
    /// <returns>如果資料格式錯誤，回傳Null</returns>
    DataStruct.PhoneFormat_struct PhoneFormat()
    {
        if (!string.IsNullOrEmpty(phoneNumber_inputField.text))
        {
            DataStruct.PhoneFormat_struct _phoneFormat = new DataStruct.PhoneFormat_struct()
            {
                location = CountryCode.GetCountryLocationByPhoneTitle(phoneTitle_dropdown.captionText.text),
                number = phoneNumber_inputField.text
            };
            return _phoneFormat;
        }
        return null;
    }

    //====================================================================
    /// <summary>
    /// 確認
    /// <para>詳細內容: 點擊UI確認按鍵時觸發</para>
    /// </summary>
    public void Confirm()
    {
        CheckPhoneFormat();
        if (phoneFormatedData != null)
        {
            if (ConfirmEvent != null)
                ConfirmEvent(phoneFormatedData);
            QuitView();
        }
        else
        {
            NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_phoneNumberIsEmpty);
        }
    }

    //====================================================================
    /// <summary>
    /// 取消
    /// <para>詳細內容: 點擊UI取消按鍵時觸發</para>
    /// </summary>
    public void Cancel()
    {
        if (CancelEvent != null)
            CancelEvent();
        QuitView();
    }

    //====================================================================
    /// <summary>
    /// 關閉視窗
    /// </summary>
    public void QuitView()
    {
        Destroy(this.gameObject);
    }

    //====================================================================
}
