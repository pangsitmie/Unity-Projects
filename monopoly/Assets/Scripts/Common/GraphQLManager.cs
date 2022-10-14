using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphQlClient.Core;
/// <summary>
/// GraphQL的管理器
/// </summary>
public class GraphQLManager : MonoBehaviour
{
    private static GraphQLManager instance = null;
    public static GraphQLManager Instance { get => instance; }

    public GraphApi TicketSystemAPIReference;

    public static class GetQueryByName
    {
        public const string getAllCommodityCatedory = "getAllCommodityCategory";
        public const string commodityInformation = "commodityInformation";
        public const string allRecipientInformation = "allRecipientInformation";
        public const string getMemberInfo = "getMemberInfo";
        public const string allOrderInformation = "allOrderInformation";
        public const string gameResult = "gameResult";
        public const string machineTypesInfo = "machineTypesInfo";
        public const string friendList = "friendList";
        public const string searchPlayers = "searchPlayers";
        public const string friendInviteList = "friendInviteList";
        public const string fetchLastPrivateMsg = "fetchLastPrivateMsg";
        public const string fetchPrivateChatRecord = "fetchPrivateChatRecord";

    }

    public static class GetMutationByName
    {
        public const string modifyNickname = "modifyNickname";
        public const string addRecipientInformation = "addRecipientInformation";
        public const string modifyRecipientInformation = "modifyRecipientInformation";
        public const string placeOrder = "placeOrder";
        public const string sendFriendRequest = "sendFriendRequest";
        public const string removeFriend = "removeFriend";
        public const string acceptFriendRequest = "acceptFriendRequest";
        public const string removeFriendRequest = "removeFriendRequest";
        public const string updateRegistrationToken = "updateRegistrationToken";
        public const string sendPrivateMsg = "sendPrivateMsg";


    }
    //====================================================================
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    //====================================================================
    /// <summary>
    /// GraphQL 錯誤訊息處理及顯示
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 處理伺服器回傳的錯誤碼</para>
    /// </summary>
    public static void ErrorCodeHandle(ResponseStruct.GraphQLError_struct _errorCode_str)
    {
        Debug.Log("_errorCode_str.extensions.code: " + _errorCode_str.extensions.code);
        switch (_errorCode_str.extensions.code)
        {
            case "1x001":// token 錯誤
            case "1x002":// 請求token 已過期
            case "1x005":// 無法解析 authorization帶來的資訊（token）
                AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                {
                    NotificationManager.Instance.CreateRemoteLoginView();
                });

                break;
            //---------------------------------------------------------------------------
            case "1x006":// 註冊驗證碼錯誤
                AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                    {
                        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_verificationCodeError);
                    });
                break;
            case "2x001":// 會員不存在
                AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                    {
                        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_memberDontExist);
                    });
                break;
            case "4x001":// 好友已經存在
                AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                    {
                        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.friend_friendAlreadyExists);
                    });
                break;
            case "4x002":// 不能添加自己爲好友
                AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                    {
                        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.friend_cantAddYourselfAsFriend);
                    });
                break;
            case "4x004":// 好友邀請已經存在
                AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                    {
                        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.friend_friendInvitationSent);
                    });
                break;
            case "4x005":// 好友邀請不存在
                AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                    {
                        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.friend_friendInvitationDontExists);
                    });
                break;
            //---------------------------------------------------------------------------
            case "2x002":// 密碼格式錯誤
                AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                        {
                            NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_passwordLengthError);
                        });
                break;
            case "2x003":// 手機號格式錯誤
                AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                        {
                            NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_phoneNumberFormatError);
                        });
                break;
            case "2x004":// 手機號已註冊
                AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                            {
                                NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_phoneNumberRegistered);
                            });
                break;
            case "2x005":// 手機號或密碼錯誤
                AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                            {
                                NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_phoneOrPasswordError);
                            });
                break;
            case "2x006":// 電話號所在地未提供服務
                AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                                        {
                                            NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.login_phoneNumberIsNoAvailableAtTheLocation);
                                        });
                break;
            //---------------------------------------------------------------------------
            case "1x003":// request header not Bearer type
            case "1x004":// 請求token 還未可使用 not before
            case "4x003":// 該id不在好友名單中
            default:
                AsyncUIEventManager.Instance.AddAsyncUIEvent(() =>
                    {
                        Debug.Log("LocalizationManager.KeyTable.common_handleFail");
                        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_handleFail);
                    });
                break;
        }
    }

    //====================================================================
}
