using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Tables;
public class EndObject : MonoBehaviour
{
    [SerializeField] Text message_tx;
    private string message_key = LocalizationManager.KeyTable.common_messageLoadError;



    //====================================================================
    /// <summary>
    /// 設定顯示的文字
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 設定文字的key</para>
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
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 更新文字</para>
    /// </summary>
    public void UpdateText(StringTable _stringTable)
    {
        message_tx.text = LocalizationManager.GetLocalizedString(_stringTable, message_key);
    }

    //====================================================================
}
