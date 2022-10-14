using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphQlClient.Core;
/// <summary>
/// (存在於 MallScene與OptionScene)
/// </summary>
public class RecipientManager : MonoBehaviour
{
    //====================================================================
    private static RecipientManager instance = null;
    public static RecipientManager Instance { get => instance; }

    //====================================================================
    [SerializeField] LoadingObjectManager loadingObjectManager;
    [SerializeField] Transform canvas_transform;

    //====================================================================
    [SerializeField] RecipientEditView recipientEditView_pf;
    private RecipientEditView recipientEditView;

    //====================================================================
    private ResponseStruct.RecipientInfo_struct[] recipientDatas;

    //====================================================================
    public delegate void RecipientDataUpdate();
    public event RecipientDataUpdate UpdateRecipientInfo;

    //====================================================================
    private void Awake()
    {
        instance = this;
    }

    //====================================================================
    private void Start()
    {
        RequestRecipientInformation();
    }

    //====================================================================
    /// <summary>
    /// 向伺服器請求收件人資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void RequestRecipientInformation()
    {
        loadingObjectManager.LoadingCountAdd();

        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetQueryByName.allRecipientInformation, GraphApi.Query.Type.Query);
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, RequestRecipientInformationCallback));
    }

    //====================================================================
    /// <summary>
    /// 請求收件人資料的回傳
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void RequestRecipientInformationCallback(string _responseData_str)
    {
        if (!string.IsNullOrEmpty(_responseData_str))
        {
            try
            {
                var _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.RecipientInfoResponse_struct>>(_responseData_str);

                if (_responseData.errors == null)
                {
                    recipientDatas = _responseData.data.allRecipientInfo;
                    UpdateRecipientInfo?.Invoke();
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
    /// 取得收件人資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public ResponseStruct.RecipientInfo_struct GetRecipientData(int _index_int)
    {
        if (recipientDatas != null && recipientDatas.Length != 0 && _index_int >= 0)
        {
            return recipientDatas[_index_int];
        }
        else
        {
            return new ResponseStruct.RecipientInfo_struct() { id = -1 };
        }
    }

    //====================================================================
    /// <summary>
    /// 開啟收件人資料編輯畫面
    /// <para>編輯者: Dimos</para>
    /// <para>如無現有的收件人資料，建立假資料以利編輯</para>
    /// </summary>
    public void OpenRecipientEditView()
    {
        if (recipientEditView == null)
        {
            recipientEditView = Instantiate(recipientEditView_pf, canvas_transform);
            if (recipientDatas == null || recipientDatas.Length == 0)
            {
                recipientEditView.Set(new ResponseStruct.RecipientInfo_struct() { id = -1 });
            }
            else
            {
                recipientEditView.Set(recipientDatas[0]);
            }
        }
        else
        {
            Debug.LogError("RecipientManager.OpenRecipientEditView() RecipientEditView has not been destroy. Try to destroy RecipientEditView.");

            Destroy(recipientEditView.gameObject);
        }
    }

    //====================================================================
    /// <summary>
    /// 設定收件人資料
    /// <para>編輯者: Dimos</para>
    /// <para>如 id == -1 代表尚無收件人資料所以新增一筆新的資料，否則修改指定的收件人資料</para>
    /// </summary>
    public void SetRecipientInformation(int _id, RequestStruct.RecipientInformation_struct _recipientInfo)
    {
        if (_id == -1)
        {
            AddRecipientInformation(_recipientInfo);
        }
        else
        {
            ModifyRecipientInformation(_id, _recipientInfo);
        }
    }

    //====================================================================
    /// <summary>
    /// 新增收件人資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void AddRecipientInformation(RequestStruct.RecipientInformation_struct _recipientInfo)
    {
        loadingObjectManager.LoadingCountAdd();

        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetMutationByName.addRecipientInformation, GraphApi.Query.Type.Mutation);
        _graphQuery.SetArgs(new { input = _recipientInfo });
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, AddRecipientInformationCallback));
    }

    //====================================================================
    /// <summary>
    /// 新增收件人資料的回傳
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void AddRecipientInformationCallback(string _responseData_str)
    {
        if (!string.IsNullOrEmpty(_responseData_str))
        {
            try
            {
                var _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.RecipientInfoResponse_struct>>(_responseData_str);

                if (_responseData.errors == null)
                {
                    RequestRecipientInformation();
                    recipientEditView.QuitView();
                }
                else
                {
                    switch (_responseData.errors[0].extensions.code)
                    {
                        default:
                            GraphQLManager.ErrorCodeHandle(_responseData.errors[0]);
                            break;
                    }
                    recipientEditView.UnlockRequest();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex, this);
            }
        }
        else
        {
            recipientEditView.UnlockRequest();
        }

        loadingObjectManager.LoadingCountSub();
    }

    //====================================================================
    /// <summary>
    /// 修改收件人資料
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void ModifyRecipientInformation(int _id, RequestStruct.RecipientInformation_struct _recipientInfo)
    {
        loadingObjectManager.LoadingCountAdd();

        GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetMutationByName.modifyRecipientInformation, GraphApi.Query.Type.Mutation);
        _graphQuery.SetArgs(new { recipientId = 1, info = _recipientInfo });
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphQuery.query, SettingData.Data.token, ModifyRecipientInformationCallback));
    }

    //====================================================================
    /// <summary>
    /// 修改收件人資料的回傳
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void ModifyRecipientInformationCallback(string _responseData_str)
    {
        if (!string.IsNullOrEmpty(_responseData_str))
        {
            try
            {
                var _responseData = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.RecipientInfoResponse_struct>>(_responseData_str);
                if (_responseData.errors == null)
                {
                    RequestRecipientInformation();
                    recipientEditView.QuitView();
                }
                else
                {
                    switch (_responseData.errors[0].extensions.code)
                    {
                        default:
                            GraphQLManager.ErrorCodeHandle(_responseData.errors[0]);
                            break;
                    }
                    recipientEditView.UnlockRequest();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex, this);
            }
        }
        else
        {
            recipientEditView.UnlockRequest();
        }

        loadingObjectManager.LoadingCountSub();
    }

    //====================================================================
}
