using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 場景管理
/// <para>因名稱如果設為SceneManager會與UnityEngine.SceneManagement函式庫的SceneManager衝突， 因故取名為SceneController</para>
/// <para>操作方式: 此腳本使用Instance(單例)引用</para>
/// </summary>
public class SceneController : MonoBehaviour
{
    public enum SceneNameEnum
    {
        BeginScene,
        LoadingScene,
        LoginScene,
        MainScene,
        MallScene,
        OrderScene,
        PlayRecordScene,
        MachineScene,
        OptionScene,
        UserInformationScene
    }

    //====================================================================
    private static SceneController instance = null;
    public static SceneController Instance { get => instance; }

    //Canvas管理-----------------------------
    private CanvasElement currentSceneCanvas = null;
    public CanvasElement CurrentSceneCanvas { get => currentSceneCanvas; }
    private Stack<CanvasElement> sceneCanvas_stack = new Stack<CanvasElement>();

    //====================================================================
    private void Awake()
    {
        instance = this;
    }

    //====================================================================
    /// <summary>
    /// 註冊當前的Canvas
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 在開啟新場景時將該場景的Canvas設為currentSceneCanvas，
    ///       並且如果存在上一個Canvas，將其存入Stack</para>
    /// </summary>
    public void CanvasRegister(CanvasElement _target)
    {
        Debug.Log("SceneController.CanvasRegister() Canvas's name: " + _target.name);
        if (currentSceneCanvas != null)
        {
            sceneCanvas_stack.Push(currentSceneCanvas);
        }
        currentSceneCanvas = _target;
    }

    //====================================================================
    /// <summary>
    /// 關閉場景
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 關閉場景時會將Stack儲存的Canvas Pop到currentSceneCanvas</para>
    /// </summary>
    public void UnLoadScene(string _target_str)
    {
        Debug.Log("SceneController.UnLoadScene() Scene's name: " + _target_str);

        if (sceneCanvas_stack.Count > 0)
        {
            try
            {
                currentSceneCanvas = sceneCanvas_stack.Pop();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex, this);
            }
        }
        else
        {
            Debug.Log("SceneController.UnLoadScene() Scene_Canvas stack is empty");
        }

        SceneManager.UnloadSceneAsync(_target_str);
    }

    //====================================================================
    private bool canAddScene_bl = true;
    /// <summary>
    /// 添加場景
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 添加場景時可選擇添加進_OnUpdateProgress、_OnCompleted來處理相關操作</para>
    /// <para>_OnUpdateProgress: 場景進度刷新</para>
    /// <para>_OnCompleted: 場景加載完成</para>
    /// </summary>
    public void AddScene(string _target_str, System.Action<float> _OnUpdateProgress = null, System.Action _OnCompleted = null)
    {
        if (canAddScene_bl)
        {
            canAddScene_bl = false;
            StartCoroutine(AddSceneAsync(_target_str, _OnUpdateProgress, _OnCompleted));
        }
    }

    //====================================================================
    private bool canChangeScene_bl = true;
    /// <summary>
    /// 轉換場景
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 開啟目標場景後關閉其餘場景,轉換場景時可選擇添加進_OnUpdateProgress、_OnCompleted來處理相關操作</para>
    /// <para>_OnUpdateProgress: 場景進度刷新</para>
    /// <para>_OnCompleted: 場景加載完成</para>
    /// </summary>
    public void ChangeScene(string _target_str, System.Action<float> _OnUpdateProgress = null, System.Action _OnCompleted = null)
    {
        if (canChangeScene_bl)
        {
            canChangeScene_bl = false;
            StartCoroutine(ChangeSceneAsync(_target_str, _OnUpdateProgress, _OnCompleted));
        }
    }

    //====================================================================
    /// <summary>
    /// 異步添加場景
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 添加場景時可選擇添加進_OnUpdateProgress、_OnCompleted來處理相關操作</para>
    /// <para>_OnUpdateProgress: 場景進度刷新</para>
    /// <para>_OnCompleted: 場景加載完成</para>
    /// </summary>
    private IEnumerator AddSceneAsync(string _target_str, System.Action<float> _OnUpdateProgress = null, System.Action _OnCompleted = null)
    {
        AsyncOperation _asyncScene = SceneManager.LoadSceneAsync(_target_str, LoadSceneMode.Additive);

        _asyncScene.allowSceneActivation = false;
        while (_asyncScene.progress < 0.9f)
        {
            if (_OnUpdateProgress != null)
                _OnUpdateProgress(_asyncScene.progress);

            yield return null;
        }
        _asyncScene.allowSceneActivation = true;

        yield return new WaitForSeconds(0.3f);

        canAddScene_bl = true;

        if (_OnCompleted != null)
            _OnCompleted();
    }

    //====================================================================
    /// <summary>
    /// 異步轉換場景
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 開啟目標場景後關閉其餘場景,轉換場景時可選擇添加進_OnUpdateProgress、_OnCompleted來處理相關操作</para>
    /// <para>_OnUpdateProgress: 場景進度刷新</para>
    /// <para>_OnCompleted: 場景加載完成</para>
    /// </summary>
    private IEnumerator ChangeSceneAsync(string _target_str, System.Action<float> _OnUpdateProgress = null, System.Action _OnCompleted = null)
    {
        AsyncOperation _asyncScene = SceneManager.LoadSceneAsync(_target_str);

        _asyncScene.allowSceneActivation = false;
        while (_asyncScene.progress < 0.9f)
        {
            if (_OnUpdateProgress != null)
                _OnUpdateProgress(_asyncScene.progress);
            yield return null;
        }
        _asyncScene.allowSceneActivation = true;

        sceneCanvas_stack.Clear();

        yield return new WaitForSeconds(0.3f);

        canChangeScene_bl = true;

        if (_OnCompleted != null)
            _OnCompleted();
    }

    //====================================================================
}
