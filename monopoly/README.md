# 主要功能及結構
- [主要功能及結構](#主要功能及結構)
- [主要管理](#主要管理)
  - [功能分類](#功能分類)
  - [使用腳本](#使用腳本)
    - [場景管理腳本](#場景管理腳本)
    - [多語言管理腳本](#多語言管理腳本)
    - [訊息窗管理腳本](#訊息窗管理腳本)
    - [返回鍵管理腳本](#返回鍵管理腳本)
    - [Http管理腳本](#http管理腳本)
    - [GraphQL管理腳本](#graphql管理腳本)
    - [SocketIO管理腳本](#socketio管理腳本)
  - [使用方法、詳細解說](#使用方法詳細解說)
    - [場景管理](#場景管理)
      - [可呼叫函數](#可呼叫函數)
        - [場景管理範例](#場景管理範例)
    - [多語言管理](#多語言管理)
      - [輸入多語言的Key與對應的Value](#輸入多語言的key與對應的value)
      - [輸出未載入文件解決方法](#輸出未載入文件解決方法)
      - [從Inspector設定多語言方法](#從inspector設定多語言方法)
    - [訊息窗管理](#訊息窗管理)
      - [可呼叫函數](#可呼叫函數-1)
        - [建立彈出視窗範例](#建立彈出視窗範例)
    - [返回鍵管理](#返回鍵管理)
      - [可呼叫函數](#可呼叫函數-2)
        - [新增返回事件範例](#新增返回事件範例)
    - [Http管理](#http管理)
      - [可呼叫函數](#可呼叫函數-3)
        - [Http請求範例](#http請求範例)
        - [伺服回傳格式](#伺服回傳格式)
    - [GraphQL管理](#graphql管理)
      - [一般Query範例](#一般query範例)
      - [帶入參數Query範例](#帶入參數query範例)
    - [SocketIO管理](#socketio管理)
      - [可呼叫函數](#可呼叫函數-4)
        - [呼叫函數範例](#呼叫函數範例)
      - [可監聽事件](#可監聽事件)
        - [監聽事件範例](#監聽事件範例)
# 主要管理
## 功能分類

* 場景管理 (SceneController)
* 多語言管理 (LocalizationManager)
* 訊息窗管理 (NotificationManager)
* 返回鍵管理 (ReturnManager)
* Http管理 (HttpManager)
* GraphQL管理 (GraphQLManager)
* SocketIO管理 (SocketIOManager)

## 使用腳本

### 場景管理腳本
        
        SceneController.cs
        SceneElement.cs
        CanvasElement.cs

### 多語言管理腳本
   
        LocalizationManager.cs
        LocalizationText.cs
        LocaleDropdown.cs

### 訊息窗管理腳本
   
        NotificationManager.cs
        HintMessage.cs
        HintMessageAlphaDown.cs
        ConfirmView.cs
        EventView.cs
        RemoteLoginView.cs
        UpdateView.cs
        FunctionNotYetNotification.cs

### 返回鍵管理腳本
   
        ReturnManager.cs
        ReturnEventHandler.cs

### Http管理腳本
   
        HttpManager.cs
        
### GraphQL管理腳本
   
        GraphQLManager.cs

### SocketIO管理腳本
 
        SocketIOManager.cs

## 使用方法、詳細解說

### 場景管理
因名稱如果設為`SceneManager`會與`UnityEngine.SceneManagement`函式庫的`SceneManager`衝突， 因故取名為`SceneController`  

此專案的場景轉換相關功能一律使用`SceneController.cs`進行  
`SceneController.cs`使用單例方式引用  
所以整個專案只會存在一個實例化的`SceneController.cs`物件為管理器  

`SceneController.Instance.currentSceneCanvas`會儲存玩家當前場景的`Canvas`  
每個場景的`Canvas`上都需要掛載`CanvasElement.cs`  
以將當前場景的`Canvas`登記到`SceneController`  

而如果想用`Button`的`OnClick()`之類的`UnityEvent`來執行場景轉換相關功能  
則藉由`SceneElement.cs`來執行  
***
#### 可呼叫函數

- `AddScene()`  
    - 添加場景  
      於需保持原場景狀態時使用  
      原場景不會被銷毀，所以同時可存在多個場景  
      可帶入_OnCompleted用以於加載完成後立即執行一段程式

    - 可帶入  
        - `_target_str`: 場景名稱 
        - `_OnUpdateProgress`: 回傳場景加載進度(0.0f~0.9f)
        - `_OnCompleted`: 場景加載完成(加載進度0.9f時)時觸發
  
    ***
- `ChangeScene()`  
    - 變更場景  
      開啟目標場景後，除了目標場景以外的其餘場景皆會被關閉  
      可帶入_OnCompleted用以於加載完成後立即執行一段程式

    - 可帶入  
        - `_target_str`: 場景名稱 
        - `_OnUpdateProgress`: 回傳場景加載進度(0.0f~0.9f)
        - `_OnCompleted`: 場景加載完成(加載進度0.9f時)時觸發
  
    ***
- `UnLoadScene()`  
    - 銷毀場景  
      銷毀場景前會將存在於sceneCanvas_stack內最後儲存的Canvas設為currentSceneCanvas
      
    - 可帶入  
        - `_target_str`: 場景名稱 
***
##### 場景管理範例

```cs
public void Start()
{
    //先切到載入場景
    //載入場景切換完成後再切到主場景
    //此方式在切換兩個物件過多的場景時可以使用
    //可降低內存的瞬間占用量
    SceneController.Instance.ChangeScene(
        SceneController.SceneNameEnum.LoadingScene.ToString(),
        null,
        () =>
        {
            SceneController.Instance.ChangeScene(
                SceneController.SceneNameEnum.MainScene.ToString()
            );
        }
    );
}

```
***
### 多語言管理
多語言功能使用 `com.unity.localization` 的套件來實現  
`LocalizationManager.cs`會將腳本內使用到的`Key`直接在`KeyTable`設為靜態常數  
要在腳本內設定欲顯示的字串時直接引用`LocalizationManager.KeyTable.xxxx`  
此作法為統一管理`Key`，避免未來如果需更改`Key`命名方式時找不到所有目標  
如果要取得翻譯的文字，請求`LocalizationManagerGetLocalizedString()`  
要進行翻譯的`Text`可掛載`LocalizationText.cs`  
如要選擇語言的下拉選單可使用`LocaleDropdown.cs ` 

#### 輸入多語言的Key與對應的Value 
多語言對應表開啟步驟: (`Window -> Asset Management ->Localization Tables `)  

![步驟一](https://gitlab.com/cloudprogramming/nfc/unity/monopoly/-/raw/dev/Assets/Resources/Images/README/LocalizationAddKey1.png)

拉到最下面後點擊`Add New Entry`後輸入Key與對應的Value  
![步驟二](https://gitlab.com/cloudprogramming/nfc/unity/monopoly/-/raw/dev/Assets/Resources/Images/README/LocalizationAddKey2.png)  

![步驟三](https://gitlab.com/cloudprogramming/nfc/unity/monopoly/-/raw/dev/Assets/Resources/Images/README/LocalizationAddKey3.png)  

#### 輸出未載入文件解決方法
不論是`IOS`還是`Android` Build時皆需要設定`Addressable`不然可能會沒讀到多語言的文件  
設定步驟: (`Window -> Asset Management ->Addressables -> Groups. Then Build -> New Build ->Default Build Script`)  

![步驟一](https://gitlab.com/cloudprogramming/nfc/unity/monopoly/-/raw/dev/Assets/Resources/Images/README/AddressableStep1.png)

![步驟二](https://gitlab.com/cloudprogramming/nfc/unity/monopoly/-/raw/dev/Assets/Resources/Images/README/AddressableStep2.png)  

#### 從Inspector設定多語言方法  
點擊Text組件右上角的選項，選擇Localize  
![步驟一](https://gitlab.com/cloudprogramming/nfc/unity/monopoly/-/raw/dev/Assets/Resources/Images/README/LocalizationStringEvent.png)  

於String Reference選擇欲翻譯的Key  
![步驟二](https://gitlab.com/cloudprogramming/nfc/unity/monopoly/-/raw/dev/Assets/Resources/Images/README/LocalizationStringEvent2.png)

### 訊息窗管理
此專案所有通知訊息的顯示視窗統一通過`NotificationManager.cs`進行  
`NotificationManager.cs`使用單例方式引用  
所以整個專案只會存在一個實例化的`NotificationManager.cs`物件為管理器  
所有的功能預設支援多語言，所以僅可輸入多語言的`Key`來改變顯示的文字  
如要直接顯示文字需額外調整  

而所有的`_canvas_tsF`參數皆可省略，預設為當前顯示在畫面上的場景
***
#### 可呼叫函數
- `CreateHintMessage()`  
    - 可於螢幕中間下方顯示會慢慢消失的長方形黑色半透明訊息  
    同一時間只會存在一則訊息，新訊息會覆蓋舊的訊息  

    - 可帶入  
        - `_canvas_tsF`: Canvas的Transform  
        - `_message_key`: 欲顯示文字的Key  
    ***
- `CreateConfirmView()`  
    - 可於螢幕中間顯示一個帶有確認按鈕的確認視窗  
    可關閉但不會自動消失，背景顏色會變深，點擊背景也可以關閉  
    同一時間能存在多則訊息，新訊息不會覆蓋舊的訊息  

    - 可帶入  
        - `_canvas_tsF`: Canvas的Transform  
        - `_message_key`: 欲顯示文字的Key  
    ***
- `CreateEventView()`  
    - 可於螢幕中間顯示一個帶有確認、取消按鈕的彈出視窗    
    不會自動消失，背景顏色會變深，點擊背景不會關閉  
    同一時間只會存在一則訊息，新訊息會覆蓋舊的訊息  

    - 可帶入  
        - `_canvas_tsF`: Canvas的Transform  
        - `_message_key`: 欲顯示文字的Key  
        - `_confirmEvent`: 確認按鈕的觸發事件  
        - `_cancelEvent`: 取消按鈕的觸發事件  
        - `_confirmButtonText_key`: 可改變確認按鈕顯示的文字(預設為"確認"的Key)  
        - `_cancelButtonText_key`: 可改變取消按鈕顯示的文字(預設為"取消"的Key)  
    ***
- `CreateRemoteLoginView()` {API錯誤碼未提供，所以尚未實裝}  
    - 可於螢幕中間顯示一個帶有確認彈出視窗  
    當JWT錯誤時彈出此訊息，會強制使用者重新登入  
    不會自動消失，背景顏色會變深，點擊背景不會關閉  
    同一時間只會存在一則訊息，新訊息會覆蓋舊的訊息  
    按下確認會回到登入畫面  

    - 可帶入  
        - `_canvas_tsF`: Canvas的Transform  
        - `_message_key`: 欲顯示文字的Key(預設為"你的帳號在其他裝置登入了"的Key)  
    *** 
- `CreateUpdateView()`  {API未提供，所以尚未實裝}        
    - 當檢查版本時版本過低時出現，點擊確認會連接到商店  
    不會自動消失，背景顏色會變深，點擊背景不會關閉  
    同一時間只會存在一則訊息，新訊息會覆蓋舊的訊息  
    點擊確認會連接到商店  

    - 可帶入  
        - `_canvas_tsF`: Canvas的Transform  
***  
##### 建立彈出視窗範例

```cs
public void Start()
{
    NotificationManager.Instance.CreateHintMessage(
        LocalizationManager.KeyTable.key_str
    );
}

```
***
- `FunctionNotYetNotification.cs`   
    如功能尚未實作但GUI已顯示  
    可掛上此腳本並於按鈕的`OnClick()`加上`FunctionNotYet()`  
    即可在點擊後顯示`"此功能尚未開放"`  

### 返回鍵管理
此專案所有返回事件統一通過`ReturnManager.cs`進行  
`ReturnManager.cs`雖是使用單例方式引用  
但整個專案並不會存在一個實例化的`ReturnManager.cs`物件為管理器  
理論上每個場景會有自己獨立的`ReturnManager`  
避免不同場景的返回事件相互影響  

按下返回鍵會觸發當前stack裡peek出來的事件   
而該事件的函式內必須加上  
`"ReturnManager.Instance.PopReturnEvent();"`才能`Pop`掉該事件  
開出新的`ReturnManager`時會將上一個`ReturnManager`儲存起來  
並將`Instance`設置為當前的`ReturnManager`  
可同時有多個存在 但僅只有最新的一套能觸發功能   
當最新的被銷毀後 就會從`Stack`取出第二新的並設定為`Instance`  
***
#### 可呼叫函數
- `PushReturnEvent()`
    - 將下一個按返回鍵要觸發的事件放入Stack

- `PopReturnEvent()`
    - 通常在需要觸發的返回事件裡調用，其代表該事件已完成
    並將事件Pop掉

- `returnEnable_bl`
    - 開關返回鍵功能的Bool值  
    可在不希望觸發返回事件時將其設為False  
    像是顯示載入中畫面時  

- `ReturnEventHandler.cs`  
    - 在需要新增返回事件的地方掛上此腳本
    可設定在`Start()`、`OnEnable()`時添加返回事件  
    也可以自行觸發來添加一串事件或直接將一`button`上的`onClick`添加返回事件  
    通常事件內必須自行加上`"PopReturnEvent()"`才能結束掉該返回件  
    使用`enableEvent`、`startEvent`時必須將對應的`boolean`打勾  

    - 大部分用於載入新場景於`Start()`時加上返回事件  
    (像是顯示離開APP的確認視窗)  

    - 設定方法:  
        - 於Unity的Inspector可直接設定事件  
    ![Inspector](https://gitlab.com/cloudprogramming/nfc/unity/monopoly/-/raw/dev/Assets/Resources/Images/README/ReturnEventHandlerReadMe.png)

##### 新增返回事件範例

```cs
using UnityEngine.Events; //UnityEvent需引用此函式庫

private void Start()
{
    UnityEvent _ReturnEvent = new UnityEvent(); //建立UnityEvent物件
    _ReturnEvent.AddListener(ReturnCallback);   //加入監聽方法
    ReturnManager.Instance.PushReturnEvent(_ReturnEvent); //添加進當前管理器的Stack
}
private void ReturnCallback()
{
    /*
        返回鍵事件處理
    */
    ReturnManager.Instance.PopReturnEvent(); //不移除當前返回事件將導致返回功能卡在現在的處理方法
}
```
### Http管理
此專案所有`HTTP請求`統一通過`HttpManager.cs`進行  
`HttpManager.cs`使用單例方式引用  
所以整個專案只會存在一個實例化的`HttpManager.cs`物件為管理器  
***
#### 可呼叫函數
- `HttpPostGraphQLHandler()`
    - 如果API是使用GraphQL方式，則使用此方法來請求  
    Query將會於此處轉碼

    - 可帶入
        - `_path_str`: 為請求的路徑網址
        - `_graphQLData_str`: 為請求的上傳資料。如_graphQLData_str不為空值時，預設帶有("Content-Type": "application/json")的header
        - `_authToken_str`: 為請求的JWT。可為空值，如果有設定則會有("Authorization":_authToken_str)的header
        - `_Callback`: 為請求完成時的callback function，當發生路錯誤時會回傳null
    ***  
- `HttpGetHandler()`
    - 如果API是使用一般GET方式，則使用此方法來請求

    - 可帶入
        - `_path_str`: 為請求的路徑網址
        - `_authToken_str`: 為請求的JWT。可為空值，如果有設定則會有("Authorization":_authToken_str)的header
        - `_Callback`: 為請求完成時的callback function，當發生路錯誤時會回傳null
    ***  
- `HttpPostJsonHandler()`
    - 如果API是使用一般Post方式且為Json格式，則使用此方法來請求

    - 可帶入
        - `_path_str`: 為請求的路徑網址
        - `_uploadData_str`: 為請求的上傳資料。如_uploadData_str不為空值時，預設帶有("Content-Type": "application/json")的header
        - `_authToken_str`: 為請求的JWT。可為空值，如果有設定則會有("Authorization":_authToken_str)的header
        - `_Callback`: 為請求完成時的callback function，當發生路錯誤時會回傳null
    ***  
- `HttpPostHandler()`
    - 如果API是使用一般Post方式，則使用此方法來請求

    - 可帶入
        - `_path_str`: 為請求的路徑網址
        - `_uploadData_str`: 為請求的上傳資料。
        - `_contentType_str`: 為請求的Content-Type(如: "application/json")
        - `_authToken_str`: 為請求的JWT。可為空值，如果有設定則會有("Authorization":_authToken_str)的header
        - `_Callback`: 為請求完成時的callback function，當發生路錯誤時會回傳null
    ***  
- `HttpPutJsonHandler()`
    - 如果API是使用一般Put方式且為Json格式，則使用此方法來請求

    - 可帶入
        - `_path_str`: 為請求的路徑網址
        - `_uploadData_str`: 為請求的上傳資料。如_uploadData_str不為空值時，預設帶有("Content-Type": "application/json")的header
        - `_authToken_str`: 為請求的JWT。可為空值，如果有設定則會有("Authorization":_authToken_str)的header
        - `_Callback`: 為請求完成時的callback function，當發生路錯誤時會回傳null
    ***  
- `HttpPutHandler()`
    - 如果API是使用一般Put方式，則使用此方法來請求
        
    - 可帶入
        - `_path_str`: 為請求的路徑網址
        - `_uploadData_str`: 為請求的上傳資料。
        - `_contentType_str`: 為請求的Content-Type(如: "application/json")
        - `_authToken_str`: 為請求的JWT。可為空值，如果有設定則會有("Authorization":_authToken_str)的header
        - `_Callback`: 為請求完成時的callback function，當發生路錯誤時會回傳null
    ***  
- `HttpPatchJsonHandler()`
    - 如果API是使用一般Patch方式且為Json格式，則使用此方法來請求

    - 可帶入
        - `_path_str`: 為請求的路徑網址
        - `_uploadData_str`: 為請求的上傳資料。如_uploadData_str不為空值時，預設帶有("Content-Type": "application/json")的header
        - `_authToken_str`: 為請求的JWT。可為空值，如果有設定則會有("Authorization":_authToken_str)的header
        - `_Callback`: 為請求完成時的callback function，當發生路錯誤時會回傳null
    ***  
- `HttpPatchHandler()`
    - 如果API是使用一般Patch方式，則使用此方法來請求

    - 可帶入
        - `_path_str`: 為請求的路徑網址
        - `_uploadData_str`: 為請求的上傳資料。
        - `_contentType_str`: 為請求的Content-Type(如: "application/json")
        - `_authToken_str`: 為請求的JWT。可為空值，如果有設定則會有("Authorization":_authToken_str)的header
        - `_Callback`: 為請求完成時的callback function，當發生路錯誤時會回傳null
    ***  
- `HttpDeleteHandler()`
    - 如果API是使用一般Delete方式，則使用此方法來請求

    - 可帶入
        - `_path_str`: 為請求的路徑網址
        - `_authToken_str`: 為請求的JWT。可為空值，如果有設定則會有("Authorization":_authToken_str)的header
        - `_Callback`: 為請求完成時的callback function，當發生路錯誤時會回傳null  
***
##### Http請求範例

```cs
public void Start()
{
    StartCoroutine(
        HttpManager.Instance.HttpPostJsonHandler(
            url_str, 
            data_str, 
            token_str, 
            CallbackFunction
        )
    );
}

private void CallbackFunction(string _responseData_str)
{
    if (!string.IsNullOrEmpty(_responseData_str))
    {
        try
        {
            var _response = JsonUtility.FromJson<ResponseStruct.ResponseMessage<bool>>(_responseData_str);
            if (_response.errors == null)
            {
                // do some thing success
            }
            else
            {
                switch (_response.errors[0].code)
                {
                    default:
                        // do some thing error
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
```
***
##### 伺服回傳格式
```json
{
    "data":{},
    "errors":[{
        "message":"",
        "code":"",
        "description":"",
        "validationError":"",
        "path":""
    }]
}
```
***

### GraphQL管理
此專案使用`GraphQL`方式來進行API請求  
而APP端使用了`graphQL-client-unity`套件來方便建立API請求  
`GraphQLManager.cs`統一管理套件所使用的API請求參考  
`GraphQLManager.cs`使用單例方式引用  
所以整個專案只會存在一個實例化的`GraphQLManager.cs`物件為管理器  

使用GraphQL的腳本需`using GraphQlClient.Core`

`GraphQLManager`會將腳本內使用到Query的`name`直接在`GetQueryByName`設為靜態常數  
要在腳本內設定欲顯示的字串時直接引用`GraphQLManager.GetQueryByName.xxxx`  
此作法為統一管理`name`，避免未來如果需更改`name`命名方式時找不到所有目標  
`Mutation`同理
#### 一般Query範例

```cs
using GraphQlClient.Core; //引用套件

void Start()
{
    GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(
        GraphQLManager.GetQueryByName.name_str,
        GraphApi.Query.Type.Query //Mutation則需改成GraphApi.Query.Type.Mutation
    );
    
    StartCoroutine(
        HttpManager.Instance.HttpPostGraphQLHandler(
            HttpManager.UrlPath.GraphQLPath(),
            _graphQuery.query,
            SettingData.Data.token,
            CallbackFunction
        )
    );
}
private void CallbackFunction(string _responseData_str)
{
    /* 
        回傳處理
    */
}
```
***
#### 帶入參數Query範例
```cs
using GraphQlClient.Core; //引用套件

void Start()
{
    GraphApi.Query _graphQuery = GraphQLManager.Instance.TicketSystemAPIReference.GetQueryByName(
        GraphQLManager.GetQueryByName.name_str,
        GraphApi.Query.Type.Query //Mutation則需改成GraphApi.Query.Type.Mutation
    );

    _graphQuery.SetArgs(_parm); //設定參數

    StartCoroutine(
        HttpManager.Instance.HttpPostGraphQLHandler(
            HttpManager.UrlPath.GraphQLPath(),
            _graphQuery.query,
            SettingData.Data.token,
            CallbackFunction
        )
    );
}
private void CallbackFunction(string _responseData_str)
{
    /*
        回傳處理
    */
}
```

### SocketIO管理
此專案的`SocketIO`相關功能一律使用`SocketIOManager.cs`進行  
`SceneController.cs`使用單例方式引用  
所以整個專案只會存在一個實例化的`SocketIOManager.cs`物件為管理器  
***
#### 可呼叫函數
- `Connect()`
    - 連接SocketIO 
    - 需要在有正確JWT後再連接，所以是在HTTP完成登入後才能呼叫
    - ExtraHeaders在此添加的JWT Token
    - 因例外錯誤發生時不會重新嘗試，所以使用`ConnectRetry()`來補強
  
    *** 
- `ConnectRetry()`
    - 連接SocketIO 
    - 需要在有正確JWT後再連接，所以是在HTTP完成登入後才能呼叫
    - 如果有例外錯誤發生會再自行重複運行
    - 目前這個專案主要呼叫這個來連接
    
    ***  
- `DisConnect()`
    - 中斷連接SocketIO 
  
    ***  
- `SetSocketOnEvent()`
    - 設置SocketIO的監聽事件
  
    - 可帶入
        - `_eventName_str`: 欲監聽事件的名稱
        - `_status_bl`: true為監聽事件(On)、false為解除監聽事件(Off)
        - `_Response`: 為請求完成時的callback function
    - 有實作防重覆事件檢查  
      如果有重覆`On`同一個`_eventName_str`的事件則會先將上次的事件`Off`，並覆蓋上新的事件

     ***  
- `SetSocketEmitEventAsync()`
    - 用以觸發Server監聽的事件

    - 可帶入
        - `_eventName_str`: 欲觸發事件的名稱
        - `_Callback`: 為請求完成時的callback function(可以省略不帶)
        - `_args_obj`: 請求時需求的參數
    *** 
##### 呼叫函數範例
```cs

void Start()
{
    SocketIOManager.Instance.SetSocketOnEvent("onEventName_str", true, CallbackFunction_On);
    SocketIOManager.Instance.SetSocketEmitEventAsync("emitEventName_str", CallbackFunction_Emit, args);
}
private void CallbackFunction_On(string _responseData_str)
{
    /*
        回傳處理
    */
}
private void CallbackFunction_Emit(string _responseData_str)
{
    /* 
        回傳處理
    */
}
```
***  
#### 可監聽事件

- `OnConnected`
    - 連結成功時觸發

    ***  
- `OnDisconnected`
    - 中斷連結成功時觸發

    ***  
- `OnReconnectFailed`
    - 重連結失敗時觸發

    ***  
- `OnReconnected`
    - 重連結成功時觸發

    ***  
- `OnReconnectAttempt`
    - 重連結嘗試時觸發
    - 會帶入第幾次嘗試

    ***  
- `OnReconnectError`
    - 重連結發生錯誤時觸發

    ***  
- `OnSystemError`
    - 套件自帶的錯誤觸發
  
    ***  
- `OnError`
    - Server發生錯誤時觸發
  
    ***  
- `OnPing`
    - Ping時觸發  

    ***  
##### 監聽事件範例

```cs
async void Start()
{
    SocketIOManager.Instance.OnError += SoceketError; //監聽事件
}

public void SoceketError(SocketIOClient.SocketIOResponse _response)
{
    /*
        Socket IO 錯誤處理
    */
}

async void OnDestroy()
{
    SocketIOManager.Instance.OnError -= SoceketError; //解除監聽事件
}
```

***

```cs
async void Start()
{
    await SocketIOManager.Instance.ConnectRetry(); //SocketIO 連線
}
```
***