using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 用來讓個別的場景呼叫SceneController
/// </summary>
public class SceneElement : MonoBehaviour
{
    //====================================================================
    public void UnLoadScene(string _target)
    {
        SceneController.Instance.UnLoadScene(_target);
    }

    //====================================================================
    public void AddScene(string _target)
    {
        SceneController.Instance.AddScene(_target);
    }

    //====================================================================
    public void ChangeScene(string _target)
    {
        SceneController.Instance.ChangeScene(_target);
    }

    //====================================================================
}
