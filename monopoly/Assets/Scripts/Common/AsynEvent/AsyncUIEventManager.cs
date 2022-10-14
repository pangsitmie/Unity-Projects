using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsyncUIEventManager : MonoBehaviour
{
    static AsyncUIEventManager instance = null;
    public static AsyncUIEventManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AsyncUIEventManager>();
                if (instance == null)
                {
                    var obj = GameObject.Find("AsynUIEventManager");
                    if (obj == null) obj = new GameObject("AsynUIEventManager");
                    instance = obj.AddComponent<AsyncUIEventManager>();
                }
            }
            return instance;
        }
    }
    Queue<System.Action> asynUIEvent_queue = new Queue<System.Action>();
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        instance = this;
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (asynUIEvent_queue.Count > 0)
        {
            asynUIEvent_queue.Dequeue()?.Invoke();
        }
    }
    public void AddAsyncUIEvent(System.Action _UIEvent)
    {
        asynUIEvent_queue.Enqueue(_UIEvent);
    }
}
