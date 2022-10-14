using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    static FirebaseManager instance = null;
    public static FirebaseManager Instance
    {
        get
        {
            return instance;
        }
    }

    //====================================================================
    Firebase.FirebaseApp firebaseApp;

    public Firebase.FirebaseApp FirebaseApp { get => firebaseApp; }


    //====================================================================
    public System.Action OnFirebaseReady;


    //====================================================================
    bool isFirebaseReady_bl = false;
    public bool IsFirebaseReady_bl { get => isFirebaseReady_bl; }


    //====================================================================
    /// <summary>
    /// firebase的版本檢查，完成後才可使用相關功能
    /// </summary>
    void Awake()
    {
        instance = this;
#if UNITY_ANDROID
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                firebaseApp = Firebase.FirebaseApp.DefaultInstance;
                UnityEngine.Debug.Log("Firebase Version is OK");
                isFirebaseReady_bl = true;
                OnFirebaseReady?.Invoke();

                //firebaseMessageManager.enabled = true;
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
#else
        isFirebaseReady_bl = true;
        UnityEngine.Debug.Log("Firebase is Ready");
        OnFirebaseReady?.Invoke();
#endif
    }


    //====================================================================
}
