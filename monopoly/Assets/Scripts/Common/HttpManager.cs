using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
/// <summary>
/// 統一管理Http請求
/// <para>編輯者: Dimos</para>
/// </summary>
public class HttpManager : MonoBehaviour
{
    //====================================================================
    private static HttpManager instance;
    public static HttpManager Instance { get => instance; }

    //====================================================================
    const string serverHttpUrl_str = "https://tickettest.cloudprogrammingonline.com/graphql";
    //const string serverHttpUrl_str = "http://192.168.0.107:3344/graphql";
    //====================================================================
    private void Awake()
    {
        instance = this;
    }
    //====================================================================
    /// <summary>
    /// API Http 網址總整理
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public static class UrlPath
    {
        public static string GraphQLPath()
        {
            return serverHttpUrl_str;
        }
    }

    //====================================================================
    /// <summary>
    /// Http 錯誤訊息處理及顯示
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private void HttpErrorHandle(string _error_str)
    {
        NotificationManager.Instance.CreateConfirmView(LocalizationManager.KeyTable.common_httpRequestError);
    }

    //====================================================================
    /// <summary>
    /// Http_Get 總管理
    /// <para>編輯者: Dimos</para>
    /// <para>_path_str: 為請求的路徑網址</para>
    /// <para>_authToken_str: 為請求的JWT</para>
    /// <para>_Callback: 為請求完成時的callback function，當發生網路錯誤時會回傳null</para>
    /// <para>==================================</para>
    /// <para>_authToken_str可為空值，如果有設定則會有("Authorization": _authToken_str)的header</para>
    /// </summary>
    public IEnumerator HttpGetHandler(string _path_str, string _authToken_str, System.Action<string> _Callback = null)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(_path_str))
        {
            //----------------------------------------------------------
            if (!string.IsNullOrEmpty(_authToken_str))
                request.SetRequestHeader("Authorization", "Bearer " + _authToken_str);
            //----------------------------------------------------------
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("HttpManager.HttpGetHandler() Get request error: " + request.error);
                HttpErrorHandle(request.error);
                if (_Callback != null)
                    _Callback(null);
            }
            else
            {
                Debug.Log("HttpManager.HttpGetHandler() Get request success: " + request.downloadHandler.text);
                if (_Callback != null)
                    _Callback(request.downloadHandler.text);
            }
            //----------------------------------------------------------
        }
    }

    //====================================================================
    /// <summary>
    /// Http_Post_GraphQL 總管理
    /// <para>編輯者: Dimos</para>
    /// <para>_path_str: 為請求的路徑網址</para>
    /// <para>_graphQLData_str: 為請求的上傳資料</para>
    /// <para>_authToken_str: 為請求的JWT</para>
    /// <para>_Callback: 為請求完成時的callback function，當發生網路錯誤時會回傳null</para>
    /// <para>==================================</para>
    /// <para>如_graphQLData_str不為空值時，預設帶有("Content-Type": "application/json")的header</para>
    /// <para>_authToken_str可為空值，如果有設定則會有("Authorization": _authToken_str)的header</para>
    /// </summary>
    public IEnumerator HttpPostGraphQLHandler(string _path_str, string _graphQLData_str, string _authToken_str, System.Action<string> _Callback = null)
    {
        while (true)
        {
            string jsonData = JsonConvert.SerializeObject(new { query = _graphQLData_str });
            byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);
            Debug.Log(_path_str + " => " + jsonData);

            using (UnityWebRequest request = UnityWebRequest.Post(_path_str, jsonData))
            {
                //----------------------------------------------------------
                if (!string.IsNullOrEmpty(_graphQLData_str))
                {
                    request.uploadHandler = new UploadHandlerRaw(postData);
                    request.SetRequestHeader("Content-Type", "application/json");
                }
                //----------------------------------------------------------
                if (!string.IsNullOrEmpty(_authToken_str))
                    request.SetRequestHeader("Authorization", "Bearer " + _authToken_str);
                //----------------------------------------------------------
                yield return request.SendWebRequest();

                if ((request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) && request.error != "HTTP/1.1 400 Bad Request")
                {
                    Debug.LogError("HttpManager.HttpPostGraphQLHandler() Post request error: " + request.error);
                    HttpErrorHandle(request.error);
                    if (_Callback != null)
                        _Callback(null);
                    break;
                }
                else
                {
                    if (string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        Debug.Log("HttpManager.HttpPostGraphQLHandler() Post request Empty, Execute again!");
                        //StartCoroutine(HttpPostGraphQLHandler(_path_str, _graphQLData_str, _authToken_str, _Callback));
                        continue;
                    }
                    else
                    {
                        Debug.Log("HttpManager.HttpPostGraphQLHandler() Post request success: " + request.downloadHandler.text);
                        if (_Callback != null)
                            _Callback(request.downloadHandler.text);
                        break;
                    }
                }
                //----------------------------------------------------------
            }
        }
    }

    //====================================================================
    /// <summary>
    /// Http_Post_Json 總管理
    /// <para>編輯者: Dimos</para>
    /// <para>_path_str: 為請求的路徑網址</para>
    /// <para>_uploadData_str: 為請求的上傳資料</para>
    /// <para>_authToken_str: 為請求的JWT</para>
    /// <para>_Callback: 為請求完成時的callback function，當發生網路錯誤時會回傳null</para>
    /// <para>==================================</para>
    /// <para>如_uploadData_str不為空值時，預設帶有("Content-Type": "application/json")的header</para>
    /// <para>_authToken_str可為空值，如果有設定則會有("Authorization": _authToken_str)的header</para>
    /// </summary>
    public IEnumerator HttpPostJsonHandler(string _path_str, string _uploadData_str, string _authToken_str, System.Action<string> _Callback = null)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(_path_str, _uploadData_str))
        {
            //----------------------------------------------------------
            if (!string.IsNullOrEmpty(_uploadData_str))
            {
                request.SetRequestHeader("Content-Type", "application/json");
            }
            //----------------------------------------------------------
            if (!string.IsNullOrEmpty(_authToken_str))
                request.SetRequestHeader("Authorization", "Bearer " + _authToken_str);
            //----------------------------------------------------------
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("HttpManager.HttpPostJsonHandler() Post request error: " + request.error);
                HttpErrorHandle(request.error);
                if (_Callback != null)
                    _Callback(null);
            }
            else
            {
                Debug.Log("HttpManager.HttpPostJsonHandler() Post request success: " + request.downloadHandler.text);
                if (_Callback != null)
                    _Callback(request.downloadHandler.text);
            }
            //----------------------------------------------------------
        }
    }

    //====================================================================
    /// <summary>
    /// Http_Post 總管理
    /// <para>編輯者: Dimos</para>
    /// <para>_path_str: 為請求的路徑網址</para>
    /// <para>_uploadData_ary: 為請求的上傳資料</para>
    /// <para>_contentType_str: 為請求的Content-Type(如: "application/json")</para>
    /// <para>_authToken_str: 為請求的JWT</para>
    /// <para>_Callback: 為請求完成時的callback function，當發生網路錯誤時會回傳null</para>
    /// <para>==================================</para>
    /// <para>_authToken_str可為空值，如果有設定則會有("Authorization": _authToken_str)的header</para>
    /// </summary>
    public IEnumerator HttpPostHandler(string _path_str, byte[] _uploadData_ary, string _contentType_str, string _authToken_str, System.Action<string> _Callback = null)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(_path_str, UnityWebRequest.kHttpVerbPOST))
        {
            //----------------------------------------------------------
            if (_uploadData_ary != null)
            {
                request.uploadHandler = new UploadHandlerRaw(_uploadData_ary);
                request.SetRequestHeader("Content-Type", _contentType_str);
            }
            //----------------------------------------------------------
            if (!string.IsNullOrEmpty(_authToken_str))
                request.SetRequestHeader("Authorization", "Bearer " + _authToken_str);
            //----------------------------------------------------------
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("HttpManager.HttpPostHandler() Post request error: " + request.error);
                HttpErrorHandle(request.error);
                if (_Callback != null)
                    _Callback(null);
            }
            else
            {
                Debug.Log("HttpManager.HttpPostHandler() Post request success: " + request.downloadHandler.text);
                if (_Callback != null)
                    _Callback(request.downloadHandler.text);
            }
            //----------------------------------------------------------
        }
    }

    //====================================================================
    /// <summary>
    /// Http_Put_Json 總管理
    /// <para>編輯者: Dimos</para>
    /// <para>_path_str: 為請求的路徑網址</para>
    /// <para>_uploadData_str: 為請求的上傳資料</para>
    /// <para>_authToken_str: 為請求的JWT</para>
    /// <para>_Callback: 為請求完成時的callback function，當發生網路錯誤時會回傳null</para>
    /// <para>==================================</para>
    /// <para>如_uploadData_str不為空值時，預設帶有("Content-Type": "application/json")的header</para>
    /// <para>_authToken_str可為空值，如果有設定則會有("Authorization": _authToken_str)的header</para>
    /// </summary>
    public IEnumerator HttpPutJsonHandler(string _path_str, string _uploadData_str, string _authToken_str, System.Action<string> _Callback = null)
    {
        using (UnityWebRequest request = UnityWebRequest.Put(_path_str, _uploadData_str))
        {
            //----------------------------------------------------------
            if (!string.IsNullOrEmpty(_uploadData_str))
            {
                request.SetRequestHeader("Content-Type", "application/json");
            }
            //----------------------------------------------------------
            if (!string.IsNullOrEmpty(_authToken_str))
                request.SetRequestHeader("Authorization", "Bearer " + _authToken_str);
            //----------------------------------------------------------
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("HttpManager.HttpPutJsonHandler() Put request error: " + request.error);
                HttpErrorHandle(request.error);
                if (_Callback != null)
                    _Callback(null);
            }
            else
            {
                Debug.Log("HttpManager.HttpPutJsonHandler() Put request success: " + request.downloadHandler.text);
                if (_Callback != null)
                    _Callback(request.downloadHandler.text);
            }
            //----------------------------------------------------------
        }
    }

    //====================================================================
    /// <summary>
    /// Http_Put 總管理
    /// <para>編輯者: Dimos</para>
    /// <para>_path_str: 為請求的路徑網址</para>
    /// <para>_uploadData_ary: 為請求的上傳資料</para>
    /// <para>_contentType_str: 為請求的Content-Type(如: "application/json")</para>
    /// <para>_authToken_str: 為請求的JWT</para>
    /// <para>_Callback: 為請求完成時的callback function，當發生網路錯誤時會回傳null</para>
    /// <para>==================================</para>
    /// <para>_authToken_str可為空值，如果有設定則會有("Authorization": _authToken_str)的header</para>
    /// </summary>
    public IEnumerator HttpPutHandler(string _path_str, byte[] _uploadData_ary, string _contentType_str, string _authToken_str, System.Action<string> _Callback = null)
    {
        using (UnityWebRequest request = new UnityWebRequest(_path_str, UnityWebRequest.kHttpVerbPUT))
        {
            //----------------------------------------------------------
            if (_uploadData_ary != null)
            {
                request.uploadHandler = new UploadHandlerRaw(_uploadData_ary);
                request.SetRequestHeader("Content-Type", _contentType_str);
            }
            //----------------------------------------------------------
            if (!string.IsNullOrEmpty(_authToken_str))
                request.SetRequestHeader("Authorization", "Bearer " + _authToken_str);
            //----------------------------------------------------------
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("HttpManager.HttpPutHandler() Put request error: " + request.error);
                HttpErrorHandle(request.error);
                if (_Callback != null)
                    _Callback(null);
            }
            else
            {
                Debug.Log("HttpManager.HttpPutHandler() Put request success: " + request.downloadHandler.text);
                if (_Callback != null)
                    _Callback(request.downloadHandler.text);
            }
            //----------------------------------------------------------
        }
    }

    //====================================================================
    /// <summary>
    /// Http_Patch_Json 總管理
    /// <para>編輯者: Dimos</para>
    /// <para>_path_str: 為請求的路徑網址</para>
    /// <para>_uploadData_str: 為請求的上傳資料</para>
    /// <para>_authToken_str: 為請求的JWT</para>
    /// <para>_Callback: 為請求完成時的callback function，當發生網路錯誤時會回傳null</para>
    /// <para>==================================</para>
    /// <para>如_uploadData_str不為空值時，預設帶有("Content-Type": "application/json")的header</para>
    /// <para>_authToken_str可為空值，如果有設定則會有("Authorization": _authToken_str)的header</para>
    /// </summary>
    public IEnumerator HttpPatchJsonHandler(string _path_str, string _uploadData_str, string _authToken_str, System.Action<string> _Callback = null)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(_path_str, _uploadData_str))
        {
            request.method = "PATCH";
            //----------------------------------------------------------
            if (!string.IsNullOrEmpty(_uploadData_str))
            {
                request.SetRequestHeader("Content-Type", "application/json");
            }
            //----------------------------------------------------------
            if (!string.IsNullOrEmpty(_authToken_str))
                request.SetRequestHeader("Authorization", "Bearer " + _authToken_str);
            //----------------------------------------------------------
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("HttpManager.HttpPatchJsonHandler() Patch request error: " + request.error);
                HttpErrorHandle(request.error);
                if (_Callback != null)
                    _Callback(null);
            }
            else
            {
                Debug.Log("HttpManager.HttpPatchJsonHandler() Patch request success: " + request.downloadHandler.text);
                if (_Callback != null)
                    _Callback(request.downloadHandler.text);
            }
            //----------------------------------------------------------
        }
    }

    //====================================================================
    /// <summary>
    /// Http_Patch 總管理
    /// <para>編輯者: Dimos</para>
    /// <para>_path_str: 為請求的路徑網址</para>
    /// <para>_uploadData_ary: 為請求的上傳資料</para>
    /// <para>_contentType_str: 為請求的Content-Type(如: "application/json")</para>
    /// <para>_authToken_str: 為請求的JWT</para>
    /// <para>_Callback: 為請求完成時的callback function，當發生網路錯誤時會回傳null</para>
    /// <para>==================================</para>
    /// <para>_authToken_str可為空值，如果有設定則會有("Authorization": _authToken_str)的header</para>
    /// </summary>
    public IEnumerator HttpPatchHandler(string _path_str, byte[] _uploadData_ary, string _contentType_str, string _authToken_str, System.Action<string> _Callback = null)
    {
        using (UnityWebRequest request = new UnityWebRequest(_path_str, "PATCH"))
        {
            //----------------------------------------------------------
            if (_uploadData_ary != null)
            {
                request.uploadHandler = new UploadHandlerRaw(_uploadData_ary);
                request.SetRequestHeader("Content-Type", _contentType_str);
            }
            //----------------------------------------------------------
            if (!string.IsNullOrEmpty(_authToken_str))
                request.SetRequestHeader("Authorization", "Bearer " + _authToken_str);
            //----------------------------------------------------------
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("HttpManager.HttpPatchHandler() Patch request error: " + request.error);
                HttpErrorHandle(request.error);
                if (_Callback != null)
                    _Callback(null);
            }
            else
            {
                Debug.Log("HttpManager.HttpPatchHandler() Patch request success: " + request.downloadHandler.text);
                if (_Callback != null)
                    _Callback(request.downloadHandler.text);
            }
            //----------------------------------------------------------
        }
    }

    //====================================================================
    /// <summary>
    /// Http_Delete 總管理
    /// <para>編輯者: Dimos</para>
    /// <para>_path_str: 為請求的路徑網址</para>
    /// <para>_authToken_str: 為請求的JWT</para>
    /// <para>_Callback: 為請求完成時的callback function，當發生網路錯誤時會回傳null</para>
    /// <para>==================================</para>
    /// <para>_authToken_str可為空值，如果有設定則會有("Authorization": _authToken_str)的header</para>
    /// </summary>
    public IEnumerator HttpDeleteHandler(string _path_str, string _authToken_str, System.Action<string> _Callback = null)
    {
        using (UnityWebRequest request = UnityWebRequest.Delete(_path_str))
        {
            //----------------------------------------------------------
            if (!string.IsNullOrEmpty(_authToken_str))
                request.SetRequestHeader("Authorization", "Bearer " + _authToken_str);
            //----------------------------------------------------------
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("HttpManager.HttpDeleteHandler() Delete request error: " + request.error);
                HttpErrorHandle(request.error);
                if (_Callback != null)
                    _Callback(null);
            }
            else
            {
                Debug.Log("HttpManager.HttpDeleteHandler() Delete request success: " + request.downloadHandler.text);
                if (_Callback != null)
                    _Callback(request.downloadHandler.text);
            }
            //----------------------------------------------------------
        }
    }

    //====================================================================
}
