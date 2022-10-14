using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GraphQlClient.Core;
/// <summary>
/// 商城總管理
/// <para>編輯者: Dimos</para>
/// <para>2021_09_07-獲取商品列表過程之補充: </para>
/// <para>首次請求時，startId固定為0，且嘗試請求數量固定為20</para>
/// <para>接下來的請求(非首次時)，startId為上一次請求的最後一項商品Id，且嘗試請求數量將更改為21</para>
/// <para>故從第二次請求開始，將會有重複資料</para>
/// <para>(程式碼內由startId是否為0來判斷是不是首次請求)</para>
/// </summary>
public class MallManager : MonoBehaviour
{
    private static MallManager instance = null;
    public static MallManager Instance { get => instance; }

    [SerializeField] LoadingObjectManager loadingObjectManager;

    //====================================================================
    /// <summary>
    /// 負責於商品列表最下方顯示"載入中"或"沒有更多商品"的文字
    /// </summary>
    [SerializeField] LocalizationText commodityListStatus;

    [SerializeField] Text userTicketAmount;

    //====================================================================
    [SerializeField] CategoryObject allCategoryObject;
    [SerializeField] CategoryObject categoryObject_pf;
    [SerializeField] Transform categoryContent;
    [SerializeField] ToggleGroup category_toggleGroup;

    //====================================================================
    [SerializeField] CommodityObject commodityObject_pf;
    [SerializeField] Transform commodityContent;
    [SerializeField] Scrollbar commodity_scrollbar;

    //====================================================================
    private RequestStruct.CommodityFilterRequest_struct commodityFilter = new RequestStruct.CommodityFilterRequest_struct()
    {
        categoryId = -1,
        startId = 0,
        quantity = 20,
        sort = new RequestStruct.SortedMethod_struct()
        {
            order = "ASC",
            field = "id"
        }
    };

    private List<CommodityObject> commodityObjects_list = new List<CommodityObject>();
    private bool isCommodityEnd_bl = false;
    private bool canRequestCommodity_bl = true;

    //====================================================================
    private Coroutine commodity_coroutine;
    private Coroutine moreCommodity_coroutine;

    //====================================================================
    private void Awake()
    {
        instance = this;
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>初始化彩票量</para>
    /// <para>初始化類別的"全部"選項</para>
    /// <para>發出取得所有類別請求</para>
    /// <para>發出取得所有商品請求</para>
    /// </summary>
    private void Start()
    {
        UpdateUserInfo();

        commodityListStatus.Set(LocalizationManager.KeyTable.common_loading);
        allCategoryObject.Set(new ResponseStruct.CategoryDataResponse_struct() { id = -1, }, category_toggleGroup, GetCommodityListByCategory);
        commodityObjects_list.Clear();

        loadingObjectManager.LoadingCountAdd();
        GraphApi.Query _graphTest = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetQueryByName.getAllCommodityCatedory, GraphApi.Query.Type.Query);
        StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphTest.query, SettingData.Data.token, CategoryListCallback));

        GetCommodityListByCategory(-1);
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 登記更新文字事件</para>
    /// </summary>
    private void OnEnable()
    {
        MemberInformationManager.UserInfoUpdate += UpdateUserInfo;
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 解除更新文字事件</para>
    /// </summary>
    private void OnDisable()
    {
        MemberInformationManager.UserInfoUpdate -= UpdateUserInfo;
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 更新文字</para>
    /// </summary>
    private void UpdateUserInfo()
    {
        userTicketAmount.text = MemberInformationManager.Ticket_int.ToString();
    }

    //====================================================================
    /// <summary>
    /// 請求類別列表的回傳
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void CategoryListCallback(string _responseData_str)
    {
        if (!string.IsNullOrEmpty(_responseData_str))
        {
            try
            {
                var _response = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.CategoriesResponse_struct>>(_responseData_str);
                if (_response.errors == null)
                {
                    BuildCategoryList(_response.data.allCommodityCategoryName);
                }
                else
                {
                    switch (_response.errors[0].extensions.code)
                    {
                        default:
                            GraphQLManager.ErrorCodeHandle(_response.errors[0]);
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
    /// 建立類別物件列表
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void BuildCategoryList(ResponseStruct.CategoryDataResponse_struct[] _categoryData_arry)
    {
        if (_categoryData_arry != null)
        {
            for (int i = 0; i < _categoryData_arry.Length; i++)
            {
                CategoryObject _categoryObject = Instantiate(categoryObject_pf, categoryContent);
                _categoryObject.Set(_categoryData_arry[i], category_toggleGroup, GetCommodityListByCategory);
            }
        }
    }

    //====================================================================
    /// <summary>
    /// 透過類別取得商品列表(若類別ID為-1，代表不篩選類別，取得全部商品)
    /// <para>編輯者: Dimos</para>
    /// <para>如有進行中的取得更多商品請求，將其停止</para>
    /// <para>如有進行中的取得商品列表請求，將其停止並減少loading計數</para>
    /// <para>將下拉列表調至頂端，避免觸發取得更多商品請求</para>
    /// </summary>
    private void GetCommodityListByCategory(int _categoryId)
    {
        if (moreCommodity_coroutine != null)
        {
            StopCoroutine(moreCommodity_coroutine);
            moreCommodity_coroutine = null;
        }

        if (commodity_coroutine != null)
        {
            StopCoroutine(commodity_coroutine);
            commodity_coroutine = null;
            loadingObjectManager.LoadingCountSub();
        }

        commodity_scrollbar.value = 1;

        isCommodityEnd_bl = false;
        canRequestCommodity_bl = false;
        loadingObjectManager.LoadingCountAdd();

        commodityFilter.startId = 0;
        commodityFilter.categoryId = _categoryId;
        commodityFilter.quantity = 20;

        GraphApi.Query _graphTest = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetQueryByName.commodityInformation, GraphApi.Query.Type.Query);
        _graphTest.SetArgs(commodityFilter);
        commodity_coroutine = StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphTest.query, SettingData.Data.token, CommodityListCallback));
    }

    //====================================================================
    /// <summary>
    /// 請求商品列表的回傳
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void CommodityListCallback(string _responseData_str)
    {
        commodity_coroutine = null;

        if (!string.IsNullOrEmpty(_responseData_str))
        {
            try
            {
                var _response = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.commodityInfoResponse_struct>>(_responseData_str);
                if (_response.errors == null)
                {
                    BuildCommodityList(_response.data.commodityInfo);
                }
                else
                {
                    switch (_response.errors[0].extensions.code)
                    {
                        default:
                            GraphQLManager.ErrorCodeHandle(_response.errors[0]);
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
        canRequestCommodity_bl = true;
    }

    //====================================================================
    /// <summary>
    /// 建立商品物件列表
    /// <para>編輯者: zxft</para>
    /// <para>建立的物件會存入commodityObjects_list</para>
    /// <para>如果建立物件時startId不為0或是建立物件的序列大於commodityObjects_list時創建新的物件</para>
    /// <para>如果建立物件時startId為0且建立物件的序列在commodityObjects_list的範圍內時修改舊物件的資料</para>
    /// <para>如果建立物件時startId為0且commodityObjects_list的大小大於_commodityData_arry 銷毀多餘物件</para>
    /// </summary>
    public void BuildCommodityList(ResponseStruct.CommodityDataResponse_struct[] _commodityData_arry)
    {
        if (_commodityData_arry != null)
        {
            if (commodityFilter.startId == 0)
            {
                if (_commodityData_arry.Length <= commodityObjects_list.Count)
                {
                    int _count = 0;

                    for (; _count < _commodityData_arry.Length; _count++)
                    {
                        commodityObjects_list[_count].Set(_commodityData_arry[_count]);
                    }

                    for (; _count < commodityObjects_list.Count;)
                    {
                        Destroy(commodityObjects_list[_count].gameObject);
                        commodityObjects_list.Remove(commodityObjects_list[_count]);
                    }
                }
                else
                {
                    int _count = 0;

                    for (; _count < commodityObjects_list.Count; _count++)
                    {
                        commodityObjects_list[_count].Set(_commodityData_arry[_count]);
                    }

                    for (; _count < _commodityData_arry.Length; _count++)
                    {
                        CommodityObject _commodityObject = Instantiate(commodityObject_pf, commodityContent);
                        _commodityObject.Set(_commodityData_arry[_count]);
                        commodityObjects_list.Add(_commodityObject);
                    }
                }
            }
            else
            {
                //int i = 1，跳過首個資料重複的商品
                for (int i = 1; i < _commodityData_arry.Length; i++)
                {
                    CommodityObject _commodityObject = Instantiate(commodityObject_pf, commodityContent);
                    _commodityObject.Set(_commodityData_arry[i]);
                    commodityObjects_list.Add(_commodityObject);
                }
            }

            //第一次請求會請求20筆資料，因為請求更多時會包含重複的startId的資料，所以會改請求21筆
            if (_commodityData_arry.Length < (commodityFilter.startId == 0 ? 20 : 21))
            {
                isCommodityEnd_bl = true;
                commodityListStatus.Set(LocalizationManager.KeyTable.mall_noMoreCommodity);
            }
            else
            {
                commodityFilter.startId = _commodityData_arry[_commodityData_arry.Length - 1].id;
            }
        }
        else
        {
            for (int i = 0; i < commodityObjects_list.Count; i++)
            {
                Destroy(commodityObjects_list[i].gameObject);
            }
            commodityObjects_list.Clear();
        }
    }

    //====================================================================
    /// <summary>
    /// 取得更多商品(ScrollBar.OnValueChange(Single)觸發)
    /// <para>編輯者: Dimos</para>
    /// <para>如果使用者將ScrollView拉至最後10筆時，發出請求</para>
    /// </summary>
    public void ScrollViewValueChange(float _value_f)
    {
        if (commodityObjects_list.Count > 0)
        {
            if (_value_f < (10.0f / commodityObjects_list.Count))
            {
                GetMoreCommodityList();
            }
        }
    }

    //====================================================================
    /// <summary>
    /// 取得更多商品
    /// <para>編輯者: Dimos</para>
    /// <para>用canRequestCommodity_bl來確保一次只會有一筆請求在運行</para>
    /// <para>isCommodityEnd_bl不為true時才會請求</para>
    /// </summary>
    private void GetMoreCommodityList()
    {
        if (canRequestCommodity_bl)
        {
            if (!isCommodityEnd_bl)
            {
                canRequestCommodity_bl = false;
                commodityListStatus.Set(LocalizationManager.KeyTable.common_loading);

                commodityFilter.quantity = 21;
                GraphApi.Query _graphTest = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetQueryByName.commodityInformation, GraphApi.Query.Type.Query);
                _graphTest.SetArgs(commodityFilter);
                moreCommodity_coroutine = StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), _graphTest.query, SettingData.Data.token, MoreCommodityListCallback));
            }
        }
    }

    //====================================================================
    /// <summary>
    /// 請求更多商品列表的回傳
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void MoreCommodityListCallback(string _responseData_str)
    {
        moreCommodity_coroutine = null;

        if (!string.IsNullOrEmpty(_responseData_str))
        {
            try
            {
                var _response = JsonUtility.FromJson<ResponseStruct.ResponseMessage<ResponseStruct.commodityInfoResponse_struct>>(_responseData_str);
                if (_response.errors == null)
                {
                    BuildCommodityList(_response.data.commodityInfo);
                }
                else
                {
                    switch (_response.errors[0].extensions.code)
                    {
                        default:
                            GraphQLManager.ErrorCodeHandle(_response.errors[0]);
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex, this);
            }
        }

        canRequestCommodity_bl = true;
    }

    //====================================================================
    /// <summary>
    /// 更改排序Order後刷新商品
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void SortOrderChange(string _input_str)
    {
        switch (_input_str)
        {
            case "ASC":
                commodityFilter.sort.order = "ASC";
                break;
            case "DESC":
                commodityFilter.sort.order = "DESC";
                break;
            default:
                Debug.LogError("MallManager.SortOrderChange() Unknown sort type");
                break;
        }

        GetCommodityListByCategory(commodityFilter.categoryId);
    }

    //====================================================================
    /// <summary>
    /// 更改排序Field後刷新商品
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void SortFieldChange(string _input_str)
    {
        switch (_input_str)
        {
            case "id":
                commodityFilter.sort.field = "id";
                break;
            case "price":
                commodityFilter.sort.field = "price";
                break;
            case "quantity":
                commodityFilter.sort.field = "quantity";
                break;
            case "name":
                commodityFilter.sort.field = "name";
                break;
            default:
                Debug.LogError("MallManager.SortFieldChange() Unknown sort type");
                break;
        }

        GetCommodityListByCategory(commodityFilter.categoryId);
    }

    //====================================================================
    /// <summary>
    /// 回到頂端
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void CommodityToTop()
    {
        commodity_scrollbar.value = 1;
    }

    //====================================================================
    /// <summary>
    /// 關閉商城畫面
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void QuitMall()
    {
        SceneController.Instance.UnLoadScene(SceneController.SceneNameEnum.MallScene.ToString());
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
    /// <summary>
    /// 刷新商品
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void RefreshCommodity()
    {
        GetCommodityListByCategory(commodityFilter.categoryId);
    }

    //====================================================================

}
