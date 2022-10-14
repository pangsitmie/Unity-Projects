using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Tables;
using GraphQlClient.Core;
/// <summary>
/// 用於開啟帳號管理(註冊帳號與重設密碼)相關畫面
/// </summary>
public class AccountOption : MonoBehaviour
{
    [SerializeField] LoadingObjectManager loadingObjectManager;
    [SerializeField] SignUpObject signUpObject_pf;

    //====================================================================
    /// <summary>
    /// 初始化註冊帳號畫面
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void OpenSignUp()
    {
        SignUpObject _signUpObject = Instantiate(signUpObject_pf, SceneController.Instance.CurrentSceneCanvas.transform);
        _signUpObject.Init(loadingObjectManager);
    }

    //====================================================================
    /// <summary>
    /// 初始化重設密碼畫面
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void OpenResetPassword()
    {
        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_functionNotOpenYet);
    }

    //====================================================================
}
