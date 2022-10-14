using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphQlClient.Core;
/// <summary>
/// 檢查JWTToken
/// </summary>
public class JWTTokenCheck : MonoBehaviour
{
    //=======================================================================
    /// <summary>
    /// 檢查設定檔是否存在JWTToken，並檢查是否可用
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: token可用的話會呼叫_Available_callBack，反之會呼叫_Fail_callBack</para>
    /// </summary>
    public void CheckJWTToken(System.Action _Available_callBack, System.Action _Fail_callBack)
    {
        if (!string.IsNullOrEmpty(SettingData.Data.token))
        {
            GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetQueryByName.getMemberInfo, GraphApi.Query.Type.Query);
            StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, (_responseData_str) =>
            {
                if (!string.IsNullOrEmpty(_responseData_str))
                {
                    var _response = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.GetMemberInfoResponse_struct>>(_responseData_str);
                    if (_response.errors == null)
                    {
                        MemberInformationManager.Id = _response.data.memberInfo.id;
                        MemberInformationManager.Nickname_str = _response.data.memberInfo.nickname;
                        MemberInformationManager.Phone = _response.data.memberInfo.phone;
                        MemberInformationManager.Ticket_int = _response.data.memberInfo.lotteryTicket;
                        if (_Available_callBack != null)
                        {
                            _Available_callBack();
                        }
                    }
                    else
                    {
                        switch (_response.errors[0].extensions.code)
                        {
                            default:
                                GraphQLManager.ErrorCodeHandle(_response.errors[0]);
                                break;
                        }

                        if (_Fail_callBack != null)
                        {
                            _Fail_callBack();
                        }
                    }
                }
                else
                {
                    if (_Fail_callBack != null)
                    {
                        _Fail_callBack();
                    }
                }
            }));
        }
        else
        {
            if (_Fail_callBack != null)
            {
                _Fail_callBack();
            }
        }
    }

    //=======================================================================
}
