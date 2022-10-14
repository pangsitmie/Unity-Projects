using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleChangeMaterial : MonoBehaviour
{
    public Material imageActiveColor, imageNoneActiveColor;
    public Image[] m_image;
    public Material textActiveColor, textNoneActiveColor;
    public Text[] m_text;
    //=============================================================
    /// <summary>
    /// 變更當前被選擇到的toggle的圖片或字體的顏色
    /// </summary>
    public void StatusChange(bool status)
    {
        if (status)
        {
            foreach (var _image in m_image)
            {
                _image.material = imageActiveColor;
            }
            foreach (var _text in m_text)
            {
                _text.material = textActiveColor;
            }
        }
        else
        {
            foreach (var _image in m_image)
            {
                _image.material = imageNoneActiveColor;
            }
            foreach (var _text in m_text)
            {
                _text.material = textNoneActiveColor;
            }
        }
    }
    //=============================================================
}
