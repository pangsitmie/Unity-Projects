using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconHandler : MonoBehaviour
{
    [SerializeField] ImageObject this_ImageObject;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        LoadIcon();
    }
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        IconManager.Instance.OnIconUpdate += LoadIcon;
    }
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        IconManager.Instance.OnIconUpdate -= LoadIcon;
    }
    public void SaveIconPath(string _path_str)
    {
        IconManager.Instance.SetIcon(MemberInformationManager.Id, _path_str);
    }
    public void LoadIcon()
    {
        string _path_str = IconManager.Instance.GetIcon(MemberInformationManager.Id);
        this_ImageObject.LoadImageWithPath(_path_str);

    }
}
