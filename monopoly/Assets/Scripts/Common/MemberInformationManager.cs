using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphQlClient.Core;
using DataStruct;
/// <summary>
/// 取得個人資料的主要腳本
/// </summary>
public delegate void UpdataMemberInformation();

public class MemberInformationManager : MonoBehaviour
{
    public static event UpdataMemberInformation UserInfoUpdate;

    private static MemberInformationManager instance = null;
    public static MemberInformationManager Instance { get => instance; }

    private static int id;
    private static PhoneFormat_struct phone;
    private static string nickname_str;
    private static int ticket_int;

    public static int Id { get => id; set { if (SetValue.SetStructValue(ref id, value)) UserInfoUpdate?.Invoke(); } }
    public static PhoneFormat_struct Phone { get => phone; set { if (SetValue.SetClassValue(ref phone, value)) UserInfoUpdate?.Invoke(); } }
    public static string Nickname_str { get => nickname_str; set { if (SetValue.SetClassValue(ref nickname_str, value)) UserInfoUpdate?.Invoke(); } }
    public static int Ticket_int { get => ticket_int; set { if (SetValue.SetStructValue(ref ticket_int, value)) UserInfoUpdate?.Invoke(); } }

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
    /// 請求個人資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void RequestMemberInformation()
    {
        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetQueryByName.getMemberInfo, GraphApi.Query.Type.Query);
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, RequestMemberInformationCallback));
    }

    //====================================================================
    /// <summary>
    /// 請求個人資料的回傳
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void RequestMemberInformationCallback(string _responseData_str)
    {
        if (!string.IsNullOrEmpty(_responseData_str))
        {
            try
            {
                var _reponseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.GetMemberInfoResponse_struct>>(_responseData_str);
                if (_reponseData.errors == null)
                {
                    Nickname_str = _reponseData.data.memberInfo.nickname;
                    Phone = _reponseData.data.memberInfo.phone;
                    Ticket_int = _reponseData.data.memberInfo.lotteryTicket;
                }
                else
                {
                    switch (_reponseData.errors[0].extensions.code)
                    {
                        default:
                            GraphQLManager.ErrorCodeHandle(_reponseData.errors[0]);
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex, this);
            }
        }
    }

    //====================================================================
}
