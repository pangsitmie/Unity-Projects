using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphQlClient.Core;
public class PlayRecordManager : MonoBehaviour
{
    [SerializeField] LoadingObjectManager loadingObjectManager;
    private List<PlayRecordObject> playRecordObjects_list = new List<PlayRecordObject>();

    [SerializeField] Transform playRecordContent_transform;
    [SerializeField] GameObject playRecordEmptyHint_gObj;

    [SerializeField] PlayRecordObject playRecordObject_pf;
    [SerializeField] PlayRecordDetail playRecordDetail;
    string graphRequset = @"
    query gameResult{
  gameResult(machineType:1){
    ... on PinballGameResult{
      id
      machineInfo{
        id
        details{
          id
          uDId
          managementId
          typeInfo{
            id
          	name
          }
        }
        shopCode
        localCode
      }
      beadResult
      ticket
      playDate
    }
  }
}";

    //====================================================================
    private void Start()
    {
        GetAllPlayRecordInformation();
    }
    public void LogTest()
    {
        Debug.Log("LogTest");
    }
    //====================================================================
    /// <summary>
    /// 取得所有遊玩紀錄
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void GetAllPlayRecordInformation()
    {
        Debug.Log("GetAllPlayRecordInformation");
        loadingObjectManager.LoadingCountAdd();
        GraphApi.Query _machineTypeQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetQueryByName.machineTypesInfo, GraphApi.Query.Type.Query);
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _machineTypeQuery.query, SettingData.Data.token, GetAllmachineTypesInfoCallback));

        loadingObjectManager.LoadingCountAdd();
        //GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetQueryByName.gameResult, GraphApi.Query.Type.Query);
        //_graphQuery.SetArgs(new { machineType = 1 });
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), graphRequset, SettingData.Data.token, GetAllPlayRecordInformationCallback));
    }
    private void GetAllmachineTypesInfoCallback(string _responseData_str)
    {
        if (!string.IsNullOrEmpty(_responseData_str))
        {
            try
            {
                var _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.MachineGameResultResponse_struct>>(_responseData_str);

                if (_responseData.errors == null)
                {

                }
                else
                {
                    switch (_responseData.errors[0].extensions.code)
                    {
                        default:
                            GraphQLManager.ErrorCodeHandle(_responseData.errors[0]);
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex, this);
            }
        }

        loadingObjectManager.LoadingCountSub();
    }
    //====================================================================
    /// <summary>
    /// 取得所有遊玩紀錄的回傳
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void GetAllPlayRecordInformationCallback(string _responseData_str)
    {
        if (!string.IsNullOrEmpty(_responseData_str))
        {
            try
            {
                var _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.GameResultResponse_struct>>(_responseData_str);

                if (_responseData.errors == null)
                {

                    BuildPlayRecordList(_responseData.data.gameResult, playRecordContent_transform, playRecordEmptyHint_gObj, ref playRecordObjects_list);
                }
                else
                {
                    switch (_responseData.errors[0].extensions.code)
                    {
                        default:
                            GraphQLManager.ErrorCodeHandle(_responseData.errors[0]);
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex, this);
            }
        }

        loadingObjectManager.LoadingCountSub();
    }

    //====================================================================
    /// <summary>
    /// 建立遊玩物件列表
    /// <para>編輯者: Dimos</para>
    /// </summary>
    /// <param name="_newPlayRecordData_arry">新的遊玩資料</param>
    /// <param name="_content_transform">遊玩資料的顯示(生成)位置</param>
    /// <param name="_emptyHint_gObj">當新遊玩資料為空時的提示物件</param>
    /// <param name="_playRecordObjects_list">現有的遊玩資料</param>
    void BuildPlayRecordList(ResponseStruct.IPinballGameResult_struct[] _newPlayRecordData_arry, Transform _content_transform, GameObject _emptyHint_gObj, ref List<PlayRecordObject> _playRecordObjects_list)
    {
        if (_newPlayRecordData_arry != null)
        {
            if (_newPlayRecordData_arry.Length <= _playRecordObjects_list.Count)
            {
                int _count = 0;

                for (; _count < _newPlayRecordData_arry.Length; _count++)
                {
                    _playRecordObjects_list[_count].Set(_newPlayRecordData_arry[_count], OpenPlayRecordDetail);
                }

                for (; _count < _playRecordObjects_list.Count;)
                {
                    Destroy(_playRecordObjects_list[_count].gameObject);
                    _playRecordObjects_list.Remove(_playRecordObjects_list[_count]);
                }
            }
            else
            {
                int _count = 0;

                for (; _count < _playRecordObjects_list.Count; _count++)
                {
                    _playRecordObjects_list[_count].Set(_newPlayRecordData_arry[_count], OpenPlayRecordDetail);
                }

                for (; _count < _newPlayRecordData_arry.Length; _count++)
                {
                    var _playRecordObject = Instantiate(playRecordObject_pf, _content_transform);
                    _playRecordObject.transform.SetSiblingIndex(1);
                    _playRecordObject.Set(_newPlayRecordData_arry[_count], OpenPlayRecordDetail);
                    _playRecordObjects_list.Add(_playRecordObject);
                }
            }
        }
        else
        {
            for (int i = 0; i < _playRecordObjects_list.Count; i++)
            {
                Destroy(_playRecordObjects_list[i].gameObject);
            }
            _playRecordObjects_list.Clear();
        }

        _emptyHint_gObj.SetActive(_newPlayRecordData_arry == null || _newPlayRecordData_arry.Length == 0);
    }

    //====================================================================
    /// <summary>
    /// 開啟遊玩詳細的畫面
    /// <para>編輯者: Dimos</para>
    /// <para>此Function會在建立PlayRecordObject時注入PlayRecordObject.cs</para>
    /// </summary>
    public void OpenPlayRecordDetail(ResponseStruct.IPinballGameResult_struct _playRecordData)
    {
        playRecordDetail.Set(_playRecordData);
    }
    //====================================================================
    /// <summary>
    /// 關閉遊玩紀錄畫面
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void QuitPlayRecord()
    {
        SceneController.Instance.UnLoadScene(SceneController.SceneNameEnum.PlayRecordScene.ToString());
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
}
