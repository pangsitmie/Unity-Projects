using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Tables;
/// <summary>
/// 異地登入訊息
/// <para>詳細內容: 當請求回傳JWT錯誤時，跳出此視窗，按下按鍵回到登入畫面，使使用者重新登入</para>
/// </summary>
public class RemoteLoginView : MonoBehaviour
{
    [SerializeField] Text message_tx;
    private string message_key = LocalizationManager.KeyTable.common_messageLoadError;

    //====================================================================
    /// <summary>
    /// 設定Message顯示的文字
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 設定Message的key</para>
    /// </summary>
    public void Set(string _message_key = "common_remoteLoginText")
    {
        message_key = _message_key;
        UpdateText(LocalizationManager.localizedStringTable.GetTable());
    }

    //====================================================================
    /// <summary>
    /// 跳轉處理(未執行相應的登出處理)
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 跳轉回登入畫面，並執行相應的登出處理</para>
    /// </summary>
    public void Confirm()
    {
        SceneController.Instance.ChangeScene(SceneController.SceneNameEnum.LoginScene.ToString());
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 登記更新文字事件，使更換語言時能改變文字</para>
    /// </summary>
    private void OnEnable()
    {
        LocalizationManager.localizedStringTable.TableChanged += UpdateText;
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 解除更新文字事件</para>
    /// </summary>
    private void OnDisable()
    {
        LocalizationManager.localizedStringTable.TableChanged -= UpdateText;
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 更新文字</para>
    /// </summary>
    public void UpdateText(StringTable _stringTable)
    {
        message_tx.text = LocalizationManager.GetLocalizedString(_stringTable, message_key);
    }

    //====================================================================
}
