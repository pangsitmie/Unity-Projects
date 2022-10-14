using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class UnityClickToggleEvent : UnityEvent<bool> { }

/// <summary>
/// 用以仿造Toggle的行為表現，但能達成不觸發OnValueChange()的狀態改變
/// <para>(2021_09_23: 目前主要使用於購物車相關功能)</para>
/// <para>位於 => MallScene: MallCanvas/ShoppingCartBackground/Body/SelectAll_clickToggle </para>
/// <para>位於 => Prefab/Mall/ProductInShoppingCartObject: ProductInShoppingCartObject/Toggle </para>
/// </summary>
public class ClickToggle : MonoBehaviour
{
    private bool isOn;
    public bool IsOn { get => isOn; set { isOn = value; ChangeDisplay(); } }

    [SerializeField] Image toggle_img;

    [SerializeField] UnityClickToggleEvent OnValueChange;

    //====================================================================
    public void ChangeStatus()
    {
        isOn = !isOn;
        OnValueChange.Invoke(isOn);
        ChangeDisplay();
    }

    //====================================================================
    /// <summary>
    /// 只改變狀態/外觀，不觸發OnValueChange()
    /// </summary>
    private void ChangeDisplay()
    {
        toggle_img.gameObject.SetActive(isOn);
    }

    //====================================================================
}
