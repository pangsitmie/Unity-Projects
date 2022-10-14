using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 顯示於結帳畫面的收件人資料
/// </summary>
public class CheckoutRecipient : MonoBehaviour
{
    private readonly int heightInNoData = 120;
    private readonly int heigthInHaveData = 390;

    [SerializeField] RectTransform this_RectTransform;

    [SerializeField] GameObject noDataHintText_gObj;
    [SerializeField] Text name_tx, phoneNumber_tx, address_tx;

    //====================================================================
    /// <summary>
    /// 載入收件人資料
    /// <para>編輯者: Dimos</para>
    /// <para>_recipientData.id == -1代表尚未填寫過收件人資料，會改變成相應的顯示</para>
    /// </summary>
    public void Set(ResponseStruct.RecipientInfo_struct _recipientData)
    {
        if (_recipientData.id == -1)
        {
            this_RectTransform.sizeDelta = new Vector2(this_RectTransform.sizeDelta.x, heightInNoData);
            noDataHintText_gObj.SetActive(true);
            name_tx.gameObject.SetActive(false);
            phoneNumber_tx.gameObject.SetActive(false);
            address_tx.gameObject.SetActive(false);
        }
        else
        {
            name_tx.text = "收件人名: " + _recipientData.name;

            StringBuilder _fullPhoneNumber = new StringBuilder();
            _fullPhoneNumber
                .Append("連絡電話: ")
                .Append("(").Append(CountryCode.GetCountryPhoneTitleByLocation(_recipientData.phone.location)).Append(") ")
                .Append(_recipientData.phone.number);
            phoneNumber_tx.text = _fullPhoneNumber.ToString();

            StringBuilder _fullAddress = new StringBuilder();
            _fullAddress
                .Append("收件地址: ")
                .Append(_recipientData.postalCode);
            address_tx.text = "收件地址: " + _recipientData.postalCode + " " + _recipientData.countyCity + " " + _recipientData.address;

            this_RectTransform.sizeDelta = new Vector2(this_RectTransform.sizeDelta.x, heigthInHaveData);
            noDataHintText_gObj.SetActive(false);
            name_tx.gameObject.SetActive(true);
            phoneNumber_tx.gameObject.SetActive(true);
            address_tx.gameObject.SetActive(true);
        }
    }

    //====================================================================
    /// <summary>
    /// 編輯收件人資料(開啟編輯收件人資料畫面)
    /// <para>編輯者: zxft</para>
    /// </summary>
    public void EditRecipient()
    {
        RecipientManager.Instance.OpenRecipientEditView();
    }

    //====================================================================
}
