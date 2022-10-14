using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using GraphQlClient.Core;
using Newtonsoft.Json;
/// <summary>
/// 購物車功能的主要腳本
/// <para>位於 => MallScene: MallCanvas/ShoppingCartBackground </para>
/// </summary>
public class ShoppingCart : MonoBehaviour
{
    private static ShoppingCart instance = null;
    public static ShoppingCart Instance { get { return instance; } }

    /// <summary>
    /// Key為商品ID, Value為下訂數量
    /// </summary>
    private Dictionary<int, int> baseData_dict = new Dictionary<int, int>();

    /// <summary>
    /// Key為商品ID, Value為購物車商品物件
    /// </summary>
    private Dictionary<int, ShoppingCartCommodityObject> shoppingCartCommodityObjects_dict = new Dictionary<int, ShoppingCartCommodityObject>();

    private List<int> selectedCommodityId_list = new List<int>(); //已被選擇的商品Id列表(等待結帳的商品Id列表)

    [SerializeField] LoadingObjectManager loadingObjectManager;
    [SerializeField] MoveOut this_moveOut;

    [SerializeField] ShoppingCartCommodityObject shoppingCartCommodityObject_pf;
    [SerializeField] Transform shoppingCartCommodityContent_transform; //購物車商品物件的容器
    [SerializeField] ClickToggle selectAll_clickToggle;
    [SerializeField] Text subTotal_tx;
    [SerializeField] Button checkout_btn;
    [SerializeField] GameObject shoppingCartHintIcon_gObj;
    [SerializeField] CheckoutView checkoutView_pf;
    private CheckoutView checkoutView;

    //==============================================================================================
    private void Awake()
    {
        instance = this;
    }

    //==============================================================================================
    /// <summary>
    /// 開場讀取購物車
    /// </summary>
    private void Start()
    {
        ReadLocalData();
    }

    //==============================================================================================
    /// <summary>
    /// 讀取儲存於本地的購物車資料
    /// <para>編輯者: Dimos</para>
    /// <para>如存在購物車資料(ShoppingCart.json)，讀取其資料</para>
    /// <para>如不存在購物車資料(ShoppingCart.json)，生成新檔案</para>
    /// <para>購物車資料以Json格式儲存</para>
    /// <para>讀取完資料後向伺服器請求完整商品資料</para>
    /// <para>請求時會將複數取得商品資料的GraphQL藉由MergeQuery()合併為一次請求送出</para>
    /// </summary>
    private void ReadLocalData()
    {
        if (File.Exists(Application.persistentDataPath + "/ShoppingCart.json"))
        {
            loadingObjectManager.LoadingCountAdd();

            using (StreamReader _file = new StreamReader(Application.persistentDataPath + "/ShoppingCart.json"))
            {
                try
                {
                    baseData_dict = JsonUtility.FromJson<Serialization<int, int>>(_file.ReadToEnd()).ToDictionary();
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            /*2021_09_07改寫為上方using
            StreamReader _file = new StreamReader(Application.persistentDataPath + "/ShoppingCart.json");
            baseData_dict = JsonUtility.FromJson<Serialization<int, int>>(_file.ReadToEnd()).ToDictionary();
            _file.Close();
            */

            Debug.Log(JsonUtility.ToJson(new Serialization<int, int>(baseData_dict)));

            if (baseData_dict.Count > 0)
            {
                Dictionary<int, int>.Enumerator enumerator = baseData_dict.GetEnumerator();
                List<string> _queries_list = new List<string>();
                while (enumerator.MoveNext())
                {
                    RequestStruct.CommodityFilterRequest_struct _filter = new RequestStruct.CommodityFilterRequest_struct()
                    {
                        categoryId = -1,
                        startId = enumerator.Current.Key,
                        quantity = 1,
                        sort = new RequestStruct.SortedMethod_struct()
                        {
                            order = "ASC",
                            field = "id"
                        }
                    };
                    GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(GraphQLManager.GetQueryByName.commodityInformation, GraphApi.Query.Type.Query);
                    _graphQuery.SetArgs(_filter);
                    _queries_list.Add(_graphQuery.query);
                }
                StartCoroutine(HttpManager.Instance.HttpPostGraphQLHandler(HttpManager.UrlPath.GraphQLPath(), MergeQuery(_queries_list.ToArray()), SettingData.Data.token, CommodityDetailCallback));
            }
            else
            {
                loadingObjectManager.LoadingCountSub();
            }
        }
        else
        {
            //baseData_dict.Clear();
            //baseData_dict = new Dictionary<int, int>();
            UpdateLocalData();
        }
    }

    //==============================================================================================
    /// <summary>
    /// 將購物車資料記錄於本地
    /// <para>編輯者: zxft</para>
    /// <para>購物車資料以Json格式儲存</para>
    /// </summary>
    private void UpdateLocalData()
    {
        using (StreamWriter _file = new StreamWriter(Application.persistentDataPath + "/ShoppingCart.json"))
        {
            try
            {
                _file.Write(JsonUtility.ToJson(new Serialization<int, int>(baseData_dict)));
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /*2021_09_07改寫為上方using
        StreamWriter _file = new StreamWriter(Application.persistentDataPath + "/ShoppingCart.json");
        _file.Write(JsonUtility.ToJson(new Serialization<int, int>(baseData_dict)));
        _file.Close();
        */
    }

    //==============================================================================================
    /// <summary>
    /// 取得商品資料的回傳
    /// <para>編輯者: Dimos</para>
    /// <para>_response僅用來檢查回傳是否成功</para>
    /// <para>如果回傳成功 呼叫GetCommodityDetailData()以進行更詳細的資料處理</para>
    /// </summary>
    public void CommodityDetailCallback(string _responseData_str)
    {
        if (!string.IsNullOrEmpty(_responseData_str))
        {
            try
            {
                var _response = JsonUtility.FromJson<ResponseStruct.ResponseMessage<string>>(_responseData_str);
                if (_response.errors == null)
                {
                    GetCommodityDetailData(_responseData_str);
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

    //==============================================================================================
    /// <summary>
    /// 處理取得的商品資料並創建商品物件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void GetCommodityDetailData(string _responseData_str)
    {
        Debug.Log("ShoppingCart.GetCommodityDetailData() _responseData_str: " + _responseData_str);

        //移除資料開頭的{"data":{ 及結尾的 }-------------------------------------------------
        int _firstColonIndex = _responseData_str.IndexOf(":"); //找出{"data":{的位址
        string _processedResponseData_str = string.Empty;
        if (_responseData_str.Length - _firstColonIndex - 3 >= 0)
        {
            _processedResponseData_str = _responseData_str.Substring(_firstColonIndex + 1, _responseData_str.Length - _firstColonIndex - 3);
        }
        //---------------------------------------------------------------------------------

        //因讀取到的資料會是長度為1的array，去除[]以方便後續處理-------------------------------
        _processedResponseData_str = _processedResponseData_str.Replace('[', ' ');
        _processedResponseData_str = _processedResponseData_str.Replace(']', ' ');
        //---------------------------------------------------------------------------------

        Debug.Log("ShoppingCart.GetCommodityDetailData() _processedResponseData_str: " + _processedResponseData_str);

        try
        {
            //使用Newtonsoft.Json將資料以Dict的方式讀出------------------------------------------
            var _responseData_dict = JsonConvert.DeserializeObject<Dictionary<string, ResponseStruct.CommodityDataResponse_struct>>(_processedResponseData_str);

            Debug.Log(JsonUtility.ToJson(new Serialization<string, ResponseStruct.CommodityDataResponse_struct>(_responseData_dict)));
            //---------------------------------------------------------------------------------

            //使用迭代器遍歷_responseData_dict以降低GC產生---------------------------------------
            Dictionary<string, ResponseStruct.CommodityDataResponse_struct>.Enumerator _enumerator = _responseData_dict.GetEnumerator();
            while (_enumerator.MoveNext())
            {
                Debug.Log(_enumerator.Current.Key);
                Debug.Log(JsonUtility.ToJson(_enumerator.Current.Value));

                var _commodityData = _enumerator.Current.Value;
                var _shoppingCartCommodityObject = Instantiate(shoppingCartCommodityObject_pf, shoppingCartCommodityContent_transform); //創建購物車商品物件
                _shoppingCartCommodityObject.Set(_commodityData, baseData_dict[_enumerator.Current.Value.id]); //載入商品資料及從baseData_dict提取的下訂數量
                shoppingCartCommodityObjects_dict.Add(_commodityData.id, _shoppingCartCommodityObject); //添加此購物車商品物件至commodityObjects_dict
            }
            //---------------------------------------------------------------------------------
            UpdateDisplay();
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex, this);
        }
    }

    //==============================================================================================
    /// <summary>
    /// 添加商品資料到購物車
    /// <para>編輯者: Dimos</para>
    /// <para>如購物車內已經存在相同ID的商品，將需求數量疊加</para>
    /// <para>如不存在，創建購物車商品物件並載入商品資料</para>
    /// <para>商品資料會在baseData_dict儲存商品Id與下訂數量 並更新本地資料</para>
    /// <para>將創建出的購物車商品物件儲存至shoppingCartCommodityObjects_dict方便之後提取資料或對商品處理</para>
    /// </summary>
    public void AddCommodityData(ResponseStruct.CommodityDataResponse_struct _commodityData, int _selectQuantity_int)
    {
        try
        {
            if (baseData_dict.ContainsKey(_commodityData.id))
            {
                baseData_dict[_commodityData.id] += _selectQuantity_int;
                shoppingCartCommodityObjects_dict[_commodityData.id].SetSelectQuantity(baseData_dict[_commodityData.id]);
            }
            else
            {
                baseData_dict.Add(_commodityData.id, _selectQuantity_int);
                var _shoppingCartCommodityObject = Instantiate(shoppingCartCommodityObject_pf, shoppingCartCommodityContent_transform);
                _shoppingCartCommodityObject.Set(_commodityData, _selectQuantity_int);
                shoppingCartCommodityObjects_dict.Add(_commodityData.id, _shoppingCartCommodityObject);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex, this);
        }

        UpdateLocalData();
        UpdateDisplay();
    }

    //==============================================================================================
    /// <summary>
    /// 從購物車移除商品資料
    /// <para>編輯者: Dimos</para>
    /// <para>檢查baseData_dict是否有符合的商品Id並移除</para>
    /// <para>檢查shoppingCartCommodityObjects_dict是否有符合的商品Id並在銷毀該物件後移除此筆資料</para>
    /// <para>檢查selectedCommodityId_list是否有符合的商品Id並移除</para>
    /// </summary>
    public void RemoveCommodityData(int _commodity_id)
    {
        if (baseData_dict.ContainsKey(_commodity_id))
            baseData_dict.Remove(_commodity_id);

        if (shoppingCartCommodityObjects_dict.TryGetValue(_commodity_id, out ShoppingCartCommodityObject _shoppingCartCommodityObject))
        {
            if (_shoppingCartCommodityObject != null)
            {
                Destroy(_shoppingCartCommodityObject.gameObject);
            }
            shoppingCartCommodityObjects_dict.Remove(_commodity_id);
        }

        if (selectedCommodityId_list.Contains(_commodity_id))
        {
            selectedCommodityId_list.Remove(_commodity_id);
        }

        UpdateLocalData();
        UpdateDisplay();
    }

    //==============================================================================================
    /// <summary>
    /// 移除所有選擇的商品資料於購物車
    /// <para>編輯者: Dimos</para>
    /// <para>移除所有selectedCommodityId_list內商品Id於baseData_dict、shoppingCartCommodityObjects_dict的資料</para>
    /// <para>檢查baseData_dict是否有符合的商品Id並移除</para>
    /// <para>檢查shoppingCartCommodityObjects_dict是否有符合的商品Id並在銷毀該物件後移除此筆資料</para>
    /// <para>清空selectedCommodityId_list</para>
    /// </summary>
    public void RemoveSelect()
    {
        for (int i = 0; i < selectedCommodityId_list.Count; i++)
        {
            var _commodity_id = selectedCommodityId_list[i];

            if (baseData_dict.ContainsKey(_commodity_id))
                baseData_dict.Remove(_commodity_id);

            if (shoppingCartCommodityObjects_dict.TryGetValue(_commodity_id, out ShoppingCartCommodityObject _shoppingCartCommodityObject))
            {
                if (_shoppingCartCommodityObject != null)
                {
                    Destroy(_shoppingCartCommodityObject.gameObject);
                }
                shoppingCartCommodityObjects_dict.Remove(_commodity_id);
            }
        }
        selectedCommodityId_list.Clear();

        UpdateLocalData();
        UpdateDisplay();
    }

    //==============================================================================================
    /// <summary>
    /// 合併複數取得商品資料的GraphQL
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private string MergeQuery(string[] _queries_str)
    {
        System.Text.StringBuilder _stringBuilder = new System.Text.StringBuilder();
        //先建立GraphQL開頭的 {"query":"query commodityInformation{\n 字串------------------
        int _index = _queries_str[0].IndexOf('{');
        if (_index + 2 < _queries_str[0].Length)
        {
            _stringBuilder.Append(_queries_str[0].Substring(0, _index + 2));
        }
        //---------------------------------------------------------------------------------

        //去除所有GraphQL開頭的 {"query":"query commodityInformation{\n 與結尾的 } 字串並添加到合併字串上
        for (int i = 0; i < _queries_str.Length; i++)
        {
            int _firstUpperBraceIndex = _queries_str[i].IndexOf('{');
            if (_queries_str[i].Length - _firstUpperBraceIndex - 7 >= 0)
            {
                _stringBuilder.Append("    commodity").Append(i).Append(":")
                             .Append(_queries_str[i].Substring(_firstUpperBraceIndex + 6, _queries_str[i].Length - _firstUpperBraceIndex - 7));
            }
        }
        //---------------------------------------------------------------------------------
        _stringBuilder.Append("\n}");
        Debug.Log("ShoppingCart.MergeQuery() _stringBuilder: " + _stringBuilder.ToString());
        return _stringBuilder.ToString();
    }

    //==============================================================================================
    /// <summary>
    /// 改變商品下訂數量
    /// <para>編輯者: Dimos</para>
    /// <para>檢查baseData_dict是否有符合的商品Id並改變其下訂數量</para>
    /// <para>更新本地資料及顯示</para>
    /// </summary>
    public void CommodityQuantityChange(int _commodity_id, int _quantity_int)
    {
        if (baseData_dict.ContainsKey(_commodity_id))
            baseData_dict[_commodity_id] = _quantity_int;

        UpdateLocalData();
        UpdateDisplay();
    }

    //==============================================================================================
    /// <summary>
    /// 添加到等待結帳的商品
    /// <para>編輯者: Dimos</para>
    /// <para>檢查waitForOrder_list是否沒有重複的商品Id後添加</para>
    /// <para>更新畫面的顯示</para>
    /// </summary>
    public void AddCommodityToWaitList(int _commodity_id)
    {
        if (!selectedCommodityId_list.Contains(_commodity_id))
            selectedCommodityId_list.Add(_commodity_id);

        UpdateDisplay();
    }

    //==============================================================================================
    /// <summary>
    /// 移除等待結帳的商品
    /// <para>編輯者: Dimos</para>
    /// <para>檢查waitForOrder_list是否有符合的商品Id後移除</para>
    /// <para>更新畫面的顯示</para>
    /// </summary>
    public void RemoveCommodityToWaitList(int _commodity_id)
    {
        if (selectedCommodityId_list.Contains(_commodity_id))
            selectedCommodityId_list.Remove(_commodity_id);

        UpdateDisplay();
    }

    //==============================================================================================
    /// <summary>
    /// 選擇全部商品/取消選擇全部商品
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void SelectAll(bool _status_bl)
    {
        //使用迭代器遍歷commodityObjects_dict以降低GC產生---------------------------------------
        var _enumerator = shoppingCartCommodityObjects_dict.GetEnumerator();
        while (_enumerator.MoveNext())
        {
            if (_enumerator.Current.Value.this_clickToggle.IsOn != _status_bl) //當選擇狀態不同時才做處理
            {
                _enumerator.Current.Value.this_clickToggle.IsOn = _status_bl;
                if (_status_bl) //檢查waitForOrder_list是否沒有重複的商品Id後添加
                {
                    if (!selectedCommodityId_list.Contains(_enumerator.Current.Key))
                        selectedCommodityId_list.Add(_enumerator.Current.Key);
                }
                else           //檢查waitForOrder_list是否有符合的商品Id後移除
                {
                    if (selectedCommodityId_list.Contains(_enumerator.Current.Key))
                        selectedCommodityId_list.Remove(_enumerator.Current.Key);
                }
            }
        }
        //---------------------------------------------------------------------------------
        UpdateDisplay();
    }

    //==============================================================================================
    /// <summary>
    /// 更新畫面的顯示
    /// <para>編輯者: Dimos</para>
    /// <para>根據商品的選擇狀態改變 全選Toggle的狀態</para>
    /// <para>計算所選商品的小計</para>
    /// </summary>
    private void UpdateDisplay()
    {
        if (selectedCommodityId_list.Count != baseData_dict.Count || baseData_dict.Count == 0)
        {
            selectAll_clickToggle.IsOn = false;
        }
        else
        {
            selectAll_clickToggle.IsOn = true;
        }

        int _subtotal_int = 0;

        try
        {
            for (int i = 0; i < selectedCommodityId_list.Count; i++)
            {
                int _commodityId_int = selectedCommodityId_list[i];
                _subtotal_int += baseData_dict[_commodityId_int] * shoppingCartCommodityObjects_dict[_commodityId_int].CommodityData.price;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex, this);
        }

        shoppingCartHintIcon_gObj.SetActive(baseData_dict.Count > 0);

        subTotal_tx.text = _subtotal_int.ToString();
        checkout_btn.interactable = selectedCommodityId_list.Count > 0;
    }

    //==============================================================================================
    /// <summary>
    /// 進入結帳畫面
    /// </summary>
    public void Checkout()
    {
        if (CheckInventoryEnough())
        {
            if (checkoutView == null)
            {
                List<DataStruct.CommodityCheckoutData_struct> _commodityCheckoutDatas_list = new List<DataStruct.CommodityCheckoutData_struct>();

                try
                {
                    for (int i = 0; i < selectedCommodityId_list.Count; i++)
                    {
                        int _commodityId_int = selectedCommodityId_list[i];
                        _commodityCheckoutDatas_list.Add(new DataStruct.CommodityCheckoutData_struct()
                        {
                            commodityData = shoppingCartCommodityObjects_dict[_commodityId_int].CommodityData,
                            amount = baseData_dict[_commodityId_int]
                        });
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex, this);
                }

                checkoutView = Instantiate(checkoutView_pf, SceneController.Instance.CurrentSceneCanvas.transform);
                checkoutView.Set(_commodityCheckoutDatas_list, loadingObjectManager);
            }
            else
            {
                Debug.LogError("ShoppingCart.Checkout() CheckoutView has not been destroy");

                Destroy(checkoutView.gameObject);
            }
        }
        else
        {
            NotificationManager.Instance.CreateConfirmView(LocalizationManager.KeyTable.mall_commodityInventoryNotEnoughError);
        }
    }

    //==============================================================================================
    /// <summary>
    /// 進入結帳畫面前，再次確認所有準備結帳的商品庫存是否充足
    /// </summary>
    /// <returns></returns>
    private bool CheckInventoryEnough()
    {
        try
        {
            for (int i = 0; i < selectedCommodityId_list.Count; i++)
            {
                var _commodity_id = selectedCommodityId_list[i];
                if (baseData_dict[_commodity_id] > shoppingCartCommodityObjects_dict[_commodity_id].CommodityData.quantity)
                {
                    return false;
                }
            }
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex, this);
            return false;
        }
    }

    //==============================================================================================
    /// <summary>
    /// 顯示購物車畫面
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void OpenView()
    {
        this_moveOut.Move(true);
    }

    //==============================================================================================
    /// <summary>
    /// 關閉購物車畫面
    /// <para>編輯者: Dimos</para>
    /// <para>此函式在OpenView()的同時會由ReturnEventHandler來Push一次</para>
    /// <para>所以觸發時會Pop掉一次返回鍵事件</para>
    /// </summary>
    public void QuitView()
    {
        this_moveOut.Move(false);
        ReturnManager.Instance.PopReturnEvent();
    }

    //==============================================================================================
}
