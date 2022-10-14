using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Tables;
public class HintMessage : MonoBehaviour
{
    [SerializeField] Text message_tx;
    [SerializeField] HintMessageAlphaDown this_hintMessageAlphaDown;
    string[] args = new string[0];
    private string message_key = LocalizationManager.KeyTable.common_messageLoadError;

    //====================================================================
    /// <summary>
    /// 重置Hint的位置
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 重置Hint的Parent，重置Hint的透明度及position</para>
    /// </summary>
    public void Reset(Transform _canvas_tsF)
    {
        this.transform.SetParent(_canvas_tsF);
        this.transform.SetAsLastSibling();
        this_hintMessageAlphaDown.enabled = true;
        this_hintMessageAlphaDown.Init();
    }

    //====================================================================
    /// <summary>
    /// 設定Message顯示的文字
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 設定Message的key</para>
    /// </summary>
    public void Set(string _message_key, params string[] _args)
    {
        message_key = _message_key;
        args = _args;
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
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 更新文字</para>
    /// </summary>
    public void UpdateText(StringTable _stringTable)
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

    //====================================================================
}
