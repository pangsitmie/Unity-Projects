using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 切換輸入框內的密碼是否顯示
/// <para>操作方式: 掛於"切換密碼是否顯示"的按鍵上</para>
/// </summary>
public class ShowPassword : MonoBehaviour
{
    [SerializeField] Image eye_Image;
    [SerializeField] Sprite closeEye, openEye;
    [SerializeField] InputField password_InputField; //掛需要隱藏or顯示內容的輸入框

    //====================================================================
    /// <summary>
    /// 切換密碼是否顯示
    /// <para>編輯者: zxft</para>
    /// </summary>
    public void SwitchPasswordShow(bool _showPassword)
    {
        if (_showPassword)
        {
            eye_Image.sprite = openEye;
            string temp = password_InputField.text;
            password_InputField.text = "";
            password_InputField.inputType = InputField.InputType.Standard;
            password_InputField.text = temp;
        }
        else
        {
            eye_Image.sprite = closeEye;
            string temp = password_InputField.text;
            password_InputField.text = "";
            password_InputField.inputType = InputField.InputType.Password;
            password_InputField.text = temp;
        }
    }

    //====================================================================
}
