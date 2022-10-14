using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class LogoutHandler : MonoBehaviour
{


    //====================================================================
    private EventView logoutView;
    /// <summary>
    /// 彈出登出確認視窗
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Logout()
    {
        logoutView = NotificationManager.Instance.CreateEventView(LocalizationManager.KeyTable.common_logoutCheck, LogoutViewConfirmCallback, LogoutViewReturnEvent);
        UnityEvent _LogoutViewReturnEvent = new UnityEvent();
        _LogoutViewReturnEvent.AddListener(LogoutViewReturnEvent);
        ReturnManager.Instance.PushReturnEvent(_LogoutViewReturnEvent);
    }
    //====================================================================


    //====================================================================
    /// <summary>
    /// 登出確認視窗返回鍵行為
    /// <para>編輯者: Dimos</para>
    /// </summary>

    private void LogoutViewReturnEvent()
    {
        Destroy(logoutView.gameObject);
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
    /// <summary>
    /// 登出確認視窗確認行為
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void LogoutViewConfirmCallback()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/ShoppingCart.json"))
        {
            System.IO.File.Delete(Application.persistentDataPath + "/ShoppingCart.json");
        }

        SettingData.Data.token = null;
        SettingData.UpdateData();
        SocketIOManager.Instance.DisConnect();
        SceneController.Instance.ChangeScene(SceneController.SceneNameEnum.LoginScene.ToString());

        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
}
