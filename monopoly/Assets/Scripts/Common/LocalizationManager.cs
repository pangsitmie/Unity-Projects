using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Metadata;
using UnityEngine.Localization.Settings;
/// <summary>
/// 多國語言相關功能
/// <para>編輯者: Dimos</para>
/// <para>詳細內容: 有KeyTable可參考Key</para>
/// <para>內含腳本使用Localization的範例，使用時須using UnityEngine.Localization.Tables</para>
/// <para>主動呼叫Update時在改完key後呼叫UpdateText(LocalizationManager.localizedStringTable.GetTable()); </para>
/// </summary>
public class LocalizationManager : MonoBehaviour
{
    //====================================================================
    /// <summary>
    /// Localization的key(entry)對應表
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 包含的常數皆為Localization的Key，變數名與該Key相同</para>
    /// </summary>
    static public Dictionary<string, string> localizationNameToDisplayString_dict = new Dictionary<string, string>() { { "Chinese (Traditional) (zh-TW)", "繁體中文" }, { "English (en)", "English" } };
    static public class KeyTable
    {
        public const string common_test = "common_test";
        public const string common_confirm = "common_confirm";
        public const string common_cancel = "common_cancel";
        public const string common_remoteLoginConfirm = "common_remoteLoginConfirm";
        public const string common_remoteLoginText = "common_remoteLoginText";
        public const string common_updateViewText = "common_updateViewText";
        public const string common_updateViewGoShop = "common_updateViewGoShop";
        public const string common_httpRequestError = "common_httpRequestError";
        public const string common_loading = "common_loading";
        public const string login_passwordLengthError = "login_passwordLengthError";
        public const string common_functionNotOpenYet = "common_functionNotOpenYet";
        public const string login_signUp = "login_signUp";
        public const string login_resetPassword = "login_resetPassword";
        public const string common_exitComfirmViewText = "common_exitComfirmViewText";
        public const string common_messageLoadError = "common_messageLoadError";
        public const string login_passwordNotSameError = "login_passwordNotSameError";
        public const string login_phoneNumberIsEmpty = "login_phoneNumberIsEmpty";
        public const string login_verificationCodeLengthError = "login_verificationCodeLengthError";
        public const string login_nicknameLengthError = "login_nicknameLengthError";
        public const string login_signUpSuccess = "login_signUpSuccess";
        public const string common_handleFail = "common_handleFail";
        public const string login_verificationCodeSendSuccess = "login_verificationCodeSendSuccess";
        public const string mall_noMoreCommodity = "mall_noMoreCommodity";
        public const string common_requestSuccess = "common_requestSuccess";
        public const string mall_recipientNameEmptyError = "mall_recipientNameEmptyError";
        public const string mall_recipientAddressEmptyError = "mall_recipientAddressEmptyError";
        public const string mall_ticketNotEnoughError = "mall_ticketNotEnoughError";
        public const string mall_commodityInventoryNotEnoughError = "mall_commodityInventoryNotEnoughError";
        public const string mall_placeOrder = "mall_placeOrder";
        public const string mall_orderSuccess = "mall_orderSuccess";
        public const string mall_checkoutWarnning = "mall_checkoutWarnning";
        public const string order_statusPreparing = "order_statusPreparing";
        public const string common_unknown = "common_unknown";
        public const string nfc_scanning = "nfc_scanning";
        public const string nfc_detected = "nfc_detected";
        public const string nfc_dataReading = "nfc_dataReading";
        public const string nfc_requestingPairing = "nfc_requestingPairing";
        public const string common_timeout = "common_timeout";
        public const string main_cantDisconnectInGame = "main_cantDisconnectInGame";
        public const string order_statusOK = "order_statusOK";
        public const string common_socketReconnect = "common_socketReconnect";
        public const string nfc_nfcError = "nfc_nfcError";
        public const string common_logoutCheck = "common_logoutCheck";
        public const string login_phoneNumberPlaceholder = "login_phoneNumberPlaceholder";
        public const string login_verificationCodeError = "login_verificationCodeError";
        public const string common_memberDontExist = "common_memberDontExist";
        public const string friend_friendAlreadyExists = "friend_friendAlreadyExists";
        public const string friend_cantAddYourselfAsFriend = "friend_cantAddYourselfAsFriend";
        public const string friend_friendInvitationSent = "friend_friendInvitationSent";
        public const string friend_friendInvitationDontExists = "friend_friendInvitationDontExists";
        public const string login_phoneNumberFormatError = "login_phoneNumberFormatError";
        public const string login_phoneNumberRegistered = "login_phoneNumberRegistered";
        public const string login_phoneOrPasswordError = "login_phoneOrPasswordError";
        public const string login_phoneNumberIsNoAvailableAtTheLocation = "login_phoneNumberIsNoAvailableAtTheLocation";





    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
    }
    //====================================================================
    /// <summary>
    /// 使用的Localization的table
    /// <para>編輯者: Dimos</para>
    /// </summary>
    static public LocalizedStringTable localizedStringTable = new LocalizedStringTable { TableReference = "StringTable" };

    //====================================================================
    /// <summary>
    /// 取得Localization翻譯String
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 輸入StringTable與Key(entry name)回傳翻譯的文字</para>
    /// </summary>
    static public string GetLocalizedString(StringTable table, string entryName)
    {
        var entry = table.GetEntry(entryName);

        if (entry == null)
        {
            entry = table.GetEntry(LocalizationManager.KeyTable.common_messageLoadError);
        }

        // We can also extract Metadata here
        var comment = entry.GetMetadata<Comment>();
        if (comment != null)
        {
            Debug.Log($"Found metadata comment for {entryName} - {comment.CommentText}");
        }

        return entry.GetLocalizedString(); // We can pass in optional arguments for Smart Format or String.Format here.
    }

    //====================================================================
    /* 範例
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
    public void UpdateText(StringTable _StringTable)
    {

    }

    //====================================================================
    */
}
