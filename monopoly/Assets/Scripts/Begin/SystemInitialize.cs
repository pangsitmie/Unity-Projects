using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using System;
/// <summary>
/// 使用者剛開啟App時最先處理的各項事務(初始化)
/// </summary>
public class SystemInitialize : MonoBehaviour
{
    [SerializeField] JWTTokenCheck this_JWTTokenCheck;

    //=======================================================================
    private void Awake()
    {
        Debug.unityLogger.logEnabled = true;
        QualitySettings.vSyncCount = 0;   // 把垂直同步關掉
        Application.targetFrameRate = 30;
        Screen.sleepTimeout = SleepTimeout.NeverSleep; //防止螢幕休眠
        SettingData.ReadData();
    }

    //=======================================================================

    IEnumerator Start()
    {
        // Wait for the localization system to initialize
        yield return LocalizationSettings.InitializationOperation;

        // Generate list of available Locales
        int selected = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (SettingData.Data.language == locale.name)
                selected = i;
        }
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[selected];
        CheckAppVersion();
    }
    //=======================================================================
    /// <summary>
    /// 檢查版本號
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 檢查版本號後，回傳給AppVersionHandle()</para>
    /// </summary>
    private void CheckAppVersion()
    {
        AppVersionCheck.CheckAppVersion(AppVersionHandle);
    }

    //====================================================================
    /// <summary>
    /// 檢查版本號的回傳處理
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 根據回傳的狀態決定再來執行的處理</para>
    /// </summary>
    private void AppVersionHandle(int _versionStatus_int)
    {
        /*
        switch (_versionStatus_int)
        {
            default:
                break;
        }
        */
        AutoLogin();
    }

    //====================================================================
    /// <summary>
    /// 自動登入處理
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 檢查設定檔的keepLogin來決定</para>
    /// </summary>
    private void AutoLogin()
    {
        if (SettingData.Data.keepLogin)
        {
            this_JWTTokenCheck.CheckJWTToken(JWTTokenAvailable, ChangeToLoginScene);
        }
        else
        {
            ChangeToLoginScene();
        }
    }

    //====================================================================
    /// <summary>
    /// JWTToken可用時的處理
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 經由LoadingScene進入MainScene</para>
    /// </summary>
    private async void JWTTokenAvailable()
    {
        await SocketIOManager.Instance.ConnectRetry();
        SceneController.Instance.ChangeScene(SceneController.SceneNameEnum.LoadingScene.ToString(), null, () =>
        {
            SceneController.Instance.ChangeScene(SceneController.SceneNameEnum.MainScene.ToString());
        });
    }

    //====================================================================
    /// <summary>
    /// 切換至Login場景
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: </para>
    /// </summary>
    private void ChangeToLoginScene()
    {
        SceneController.Instance.ChangeScene(SceneController.SceneNameEnum.LoginScene.ToString());
    }

    //====================================================================
}
