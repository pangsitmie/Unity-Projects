using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/// <summary>
/// 
/// </summary>
public class OptionManager : MonoBehaviour
{
    [SerializeField] Toggle keepLogin_toggle;
    [SerializeField] Toggle skipTicketAnimation_toggle;

    //====================================================================
    private void Start()
    {
        keepLogin_toggle.isOn = SettingData.Data.keepLogin;
        skipTicketAnimation_toggle.isOn = SettingData.Data.skipTicketAnimation;
    }

    //====================================================================
    public void SwitchKeepLogin(bool _isOn_bl)
    {
        if (SettingData.Data.keepLogin != _isOn_bl)
        {
            SettingData.Data.keepLogin = _isOn_bl;
            SettingData.UpdateData();
        }
    }

    //====================================================================
    public void SwitchSkipTicketAnimation(bool _isOn_bl)
    {
        if (SettingData.Data.skipTicketAnimation != _isOn_bl)
        {
            SettingData.Data.skipTicketAnimation = _isOn_bl;
            SettingData.UpdateData();
        }
    }

    //====================================================================
    public void CustomerService()
    {
        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_functionNotOpenYet);
    }

    //====================================================================
    public void QAndA()
    {
        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_functionNotOpenYet);
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
