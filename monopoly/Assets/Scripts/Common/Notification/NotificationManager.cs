using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 管理建置各種彈出訊息
/// <para>編輯者: Dimos</para>
/// <para>詳細內容: 管理建置各種彈出訊息</para>
/// <para>所有訊息皆為多載，可選擇不傳_canvas_tsF，預設為SceneController.Instance.CurrentSceneCanvas</para>
/// </summary>
public class NotificationManager : MonoBehaviour
{
    static NotificationManager instance = null;
    public static NotificationManager Instance { get => instance; }

    //====================================================================
    private void Awake()
    {
        instance = this;
    }

    //====================================================================
    [SerializeField] HintMessage hintMessage_pf;
    HintMessage hintMessage = null;

    /// <summary>
    /// 創建小型彈出訊息
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 創建一則會逐漸消失不可互動的小訊息，新訊息會取代掉舊訊息</para>
    /// <para>多載可選擇不傳_canvas_tsF，預設為SceneController.Instance.CurrentSceneCanvas</para>
    /// </summary>
    public void CreateHintMessage(string _message_key, params string[] _args)
    {
        _CreateHintMessage(SceneController.Instance.CurrentSceneCanvas.gameObject.transform, _message_key, _args);
    }

    /// <summary>
    /// 創建小型彈出訊息
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 創建一則會逐漸消失不可互動的小訊息，新訊息會取代掉舊訊息</para>
    /// <para>多載可選擇不傳_canvas_tsF，預設為SceneController.Instance.CurrentSceneCanvas</para>
    /// </summary>
    public void CreateHintMessage(Transform _canvas_tsF, string _message_key, params string[] _args)
    {
        _CreateHintMessage(_canvas_tsF, _message_key, _args);
    }

    void _CreateHintMessage(Transform _canvas_tsF, string _message_key, params string[] _args)
    {
        if (hintMessage == null)
        {
            hintMessage = Instantiate(hintMessage_pf, _canvas_tsF);
            hintMessage.Set(_message_key, _args);
        }
        else
        {
            hintMessage.Reset(_canvas_tsF);
            hintMessage.Set(_message_key, _args);
        }
    }

    //====================================================================
    [SerializeField] EventView eventView_pf;
    EventView eventView = null;

    /// <summary>
    /// 創建事件視窗
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 有確認跟取消，不會自動消失，背景顏色會變深，點擊背景不會關閉</para>
    /// <para>多載可選擇不傳_canvas_tsF，預設為SceneController.Instance.CurrentSceneCanvas</para>
    /// </summary>
    public EventView CreateEventView(string _message_key, System.Action _confirmEvent = null,
                                System.Action _cancelEvent = null, string _confirmButtonText_key = "common_confirm",
                                string _cancelButtonText_key = "common_cancel")
    {
        return _CreateEventView(SceneController.Instance.CurrentSceneCanvas.gameObject.transform, _message_key, _confirmEvent,
                         _cancelEvent, _confirmButtonText_key, _cancelButtonText_key);
    }

    /// <summary>
    /// 創建事件視窗
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 有確認跟取消，不會自動消失，背景顏色會變深，點擊背景不會關閉</para>
    /// <para>多載可選擇不傳_canvas_tsF，預設為SceneController.Instance.CurrentSceneCanvas</para>
    /// </summary>
    public EventView CreateEventView(Transform _canvas_tsF, string _message_key, System.Action _confirmEvent = null,
                                System.Action _cancelEvent = null, string _confirmButtonText_key = "common_confirm",
                                string _cancelButtonText_key = "common_cancel")
    {
        return _CreateEventView(_canvas_tsF, _message_key, _confirmEvent, _cancelEvent, _confirmButtonText_key, _cancelButtonText_key);
    }

    EventView _CreateEventView(Transform _canvas_tsF, string _message_key, System.Action _confirmEvent = null,
                                 System.Action _cancelEvent = null, string _confirmButtonText_key = "common_confirm",
                                 string _cancelButtonText_key = "common_cancel")
    {
        if (eventView == null)
        {
            eventView = Instantiate(eventView_pf, _canvas_tsF);
            eventView.Set(_message_key, _confirmEvent, _cancelEvent, _confirmButtonText_key, _cancelButtonText_key);
        }
        else
        {
            eventView.gameObject.transform.SetParent(_canvas_tsF);
            eventView.transform.SetAsLastSibling();
            eventView.Set(_message_key, _confirmEvent, _cancelEvent, _confirmButtonText_key, _cancelButtonText_key);
        }
        return eventView;
    }

    //====================================================================
    [SerializeField] ConfirmView confirmView_pf;
    ConfirmView confirmView = null;

    /// <summary>
    /// 創建確認視窗
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 可關閉但不會自動消失，背景顏色會變深，點擊背景也可以關閉</para>
    /// <para>允許同時存在多個</para>
    /// <para>多載可選擇不傳_canvas_tsF，預設為SceneController.Instance.CurrentSceneCanvas</para>
    /// </summary>
    public void CreateConfirmView(string _message_key, params string[] _args)
    {
        _CreateConfirmView(SceneController.Instance.CurrentSceneCanvas.gameObject.transform, _message_key, _args);
    }

    /// <summary>
    /// 創建確認視窗
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 可關閉但不會自動消失，背景顏色會變深，點擊背景也可以關閉</para>
    /// <para>允許同時存在多個</para>
    /// <para>多載可選擇不傳_canvas_tsF，預設為SceneController.Instance.CurrentSceneCanvas</para>
    /// </summary>
    public void CreateConfirmView(Transform _canvas_tsF, string _message_key, params string[] _args)
    {
        _CreateConfirmView(_canvas_tsF, _message_key, _args);
    }

    void _CreateConfirmView(Transform _canvas_tsF, string _message_key, params string[] _args)
    {
        confirmView = Instantiate(confirmView_pf, _canvas_tsF);
        confirmView.Set(_message_key, _args);
    }

    //====================================================================
    [SerializeField] RemoteLoginView remoteLoginView_pf;
    RemoteLoginView remoteLoginView = null;

    /// <summary>
    /// 創建異地登入視窗
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 當JWT錯誤時彈出此訊息，會強制使用者重新登入，背景顏色會變深，點擊背景不會關閉視窗</para>
    /// <para>多載可選擇不傳_canvas_tsF，預設為SceneController.Instance.CurrentSceneCanvas</para>
    /// </summary>
    public void CreateRemoteLoginView(string _message_key = "common_remoteLoginText")
    {
        _CreateRemoteLoginView(SceneController.Instance.CurrentSceneCanvas.gameObject.transform, _message_key);
    }

    /// <summary>
    /// 創建異地登入視窗
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 當JWT錯誤時彈出此訊息，會強制使用者重新登入，背景顏色會變深，點擊背景不會關閉視窗</para>
    /// <para>多載可選擇不傳_canvas_tsF，預設為SceneController.Instance.CurrentSceneCanvas</para>
    /// </summary>
    public void CreateRemoteLoginView(Transform _canvas_tsF, string _message_key = "common_remoteLoginText")
    {
        _CreateRemoteLoginView(_canvas_tsF, _message_key);
    }

    void _CreateRemoteLoginView(Transform _canvas_tsF, string _message_key = "common_remoteLoginText")
    {
        if (remoteLoginView == null)
        {
            remoteLoginView = Instantiate(remoteLoginView_pf, _canvas_tsF);
            remoteLoginView.Set(_message_key);
        }
        else
        {
            remoteLoginView.gameObject.transform.SetParent(_canvas_tsF);
            remoteLoginView.transform.SetAsLastSibling();
            remoteLoginView.Set(_message_key);
        }
    }

    //====================================================================
    [SerializeField] UpdateView updateView_pf;
    UpdateView updateView = null;

    /// <summary>
    /// 創建更新視窗
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 當檢查版本時版本過低時出現，點擊確認會連接到商店，點擊背景不會關閉視窗</para>
    /// </summary>
    public void CreateUpdateView()
    {
        _CreateUpdateView(SceneController.Instance.CurrentSceneCanvas.gameObject.transform);
    }

    /// <summary>
    /// 創建更新視窗
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 當檢查版本時版本過低時出現，點擊確認會連接到商店，點擊背景不會關閉視窗</para>
    /// </summary>
    public void CreateUpdateView(Transform _canvas_tsF)
    {
        _CreateUpdateView(_canvas_tsF);
    }

    void _CreateUpdateView(Transform _canvas_tsF)
    {
        if (updateView == null)
        {
            updateView = Instantiate(updateView_pf, _canvas_tsF);
        }
        else
        {
            updateView.gameObject.transform.SetParent(_canvas_tsF);
            updateView.transform.SetAsLastSibling();
        }
    }

    //====================================================================
}
