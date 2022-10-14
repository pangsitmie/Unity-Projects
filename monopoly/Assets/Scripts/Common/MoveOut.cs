using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 將物件 移入/移出 可見範圍
/// <para>詳細內容: 可掛於想要隱藏但不刪除的物件上</para>
/// </summary>
public class MoveOut : MonoBehaviour
{
    static Vector2 showPosition = Vector2.zero;
    static Vector2 hidePosition = new Vector2(-10000, 10000);

    //====================================================================
    /// <summary>
    /// 移至預設位置
    /// <para>編輯者: zxft</para>
    /// </summary>
    public void Move(bool _show)
    {
        if (_show)
            this.transform.localPosition = showPosition;
        else
            this.transform.localPosition = hidePosition;
    }

    //====================================================================
    /// <summary>
    /// 移至指定位置
    /// <para>編輯者: zxft</para>
    /// </summary>
    public void Move(Vector2 _showPosition)
    {
        this.GetComponent<RectTransform>().anchoredPosition = _showPosition;
    }

    //====================================================================
    /// <summary>
    /// 將指定物件移至預設位置
    /// <para>編輯者: zxft</para>
    /// </summary>
    static public void MoveTarget(Transform _target_transform, bool _show)
    {
        if (_show)
            _target_transform.localPosition = showPosition;
        else
            _target_transform.localPosition = hidePosition;
    }

    //====================================================================
}
