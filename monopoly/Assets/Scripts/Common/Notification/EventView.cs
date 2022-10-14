using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
public class EventView : MonoBehaviour
{
    [SerializeField] Text message_tx, confirmButton_tx, cancelButton_tx;
    private string message_key = LocalizationManager.KeyTable.common_messageLoadError;
    private string confirmButtonText_key = LocalizationManager.KeyTable.common_confirm;
    private string cancelButtonText_key = LocalizationManager.KeyTable.common_cancel;
    private System.Action ConfirmEvent, CancelEvent;

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 載入事件視窗的資料</para>
    /// <para>_message_key、_confirmButtonText_key、_cancelButtonText_key須為Localization的Key</para>
    /// <para>_ConfirmEvent、_CancelEvent可為Null</para>
    /// </summary>
    public void Set(string _message_key,
                     System.Action _ConfirmEvent,
                     System.Action _CancelEvent,
                     string _confirmButtonText_key = LocalizationManager.KeyTable.common_confirm,
                     string _cancelButtonText_key = LocalizationManager.KeyTable.common_cancel)
    {
        message_key = _message_key;
        confirmButtonText_key = _confirmButtonText_key;
        cancelButtonText_key = _cancelButtonText_key;
        ConfirmEvent = _ConfirmEvent;
        CancelEvent = _CancelEvent;
        UpdateText(LocalizationManager.localizedStringTable.GetTable());
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 按下確認按鍵時觸發</para>
    /// </summary>
    public void Confirm()
    {
        if (ConfirmEvent != null)
            ConfirmEvent();
        Destroy(this.gameObject);
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 按下取消按鍵時觸發</para>
    /// </summary>
    public void Cancel()
    {
        if (CancelEvent != null)
            CancelEvent();
        Destroy(this.gameObject);
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
        confirmButton_tx.text = LocalizationManager.GetLocalizedString(_stringTable, confirmButtonText_key);
        cancelButton_tx.text = LocalizationManager.GetLocalizedString(_stringTable, cancelButtonText_key);
    }

    //====================================================================
}
