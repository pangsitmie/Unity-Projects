using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionNotYetNotification : MonoBehaviour
{
    //====================================================================
    /// <summary>
    /// 顯示暫時尚未開放的訊息
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void FunctionNotYet()
    {
        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_functionNotOpenYet);
    }
}
