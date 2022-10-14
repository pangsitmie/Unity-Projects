using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 用以初始化FirebaseMessage
/// 因需要在檢查完Firebase版本後才能使用Firebase相關功能
/// </summary>
public class FirebaseMessageInit : MonoBehaviour
{
    //====================================================================
    void Start()
    {
        if (FirebaseManager.Instance.IsFirebaseReady_bl)
        {
            InitFirebaseMessageManager();
        }
        FirebaseManager.Instance.OnFirebaseReady += InitFirebaseMessageManager;
    }

    //====================================================================
    public void InitFirebaseMessageManager()
    {
        UnityEngine.Debug.Log("Init Firebase...");
        AsyncUIEventManager.Instance.AddAsyncUIEvent(() => { var _instance = FirebaseMessageManager.Instance; });
    }

    //====================================================================
    void OnDestroy()
    {
        FirebaseManager.Instance.OnFirebaseReady -= InitFirebaseMessageManager;
    }

    //====================================================================
}
