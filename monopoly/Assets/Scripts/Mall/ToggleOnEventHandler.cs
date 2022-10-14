using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 僅在輸入為On時觸發
/// <para>(使用於MallScene的SortView內的Toggle上)</para>
/// </summary>
public class ToggleOnEventHandler : MonoBehaviour
{
    [SerializeField] UnityEvent ToggleValueOnEvent;

    //====================================================================
    public void ToggleValueChange(bool _status_bl)
    {
        if (_status_bl)
        {
            ToggleValueOnEvent?.Invoke();
        }
    }

    //====================================================================
}
