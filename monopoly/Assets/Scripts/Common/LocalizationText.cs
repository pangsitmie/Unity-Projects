using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Tables;
/// <summary>
/// 利用程式碼設定多語言文字(Text)
/// </summary>
public class LocalizationText : MonoBehaviour
{
    [SerializeField] Text message_tx;
    //private string[] args = new string[0];
    private string message_key = LocalizationManager.KeyTable.common_messageLoadError;

    //====================================================================
    /// <summary>
    /// 設定Message要顯示的文字所對應的Key
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Set(string _message_key)
    {
        message_key = _message_key;
        UpdateText(LocalizationManager.localizedStringTable.GetTable());
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
    /// 更新文字
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void UpdateText(StringTable _stringTable)
    {
        message_tx.text = LocalizationManager.GetLocalizedString(_stringTable, message_key);
    }
    /*
    //====================================================================
    /// <summary>
    /// 設定Message要顯示的文字所對應的Key，與內部的參數
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void SetWithArg(string _message_key, params string[] _args)
    {
        message_key = _message_key;
        args = _args;
        UpdateTextWithArg(LocalizationManager.localizedStringTable.GetTable());
    }

    //====================================================================
    /// <summary>
    /// 根據Arg的有無來更新文字
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void UpdateTextWithArg(StringTable _stringTable)
    {
        string _localized_str = LocalizationManager.GetLocalizedString(_stringTable, message_key);
        if (args.Length != 0)
        {
            message_tx.text = string.Format(_localized_str, args);
        }
        else
        {
            message_tx.text = _localized_str;
        }
    }
    */
    //====================================================================
}
