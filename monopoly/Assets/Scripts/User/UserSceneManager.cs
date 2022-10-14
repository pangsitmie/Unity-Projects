using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class UserSceneManager : MonoBehaviour
{
    [SerializeField] Text nickname_tx;
    [SerializeField] Text phoneNumber_tx;
    [SerializeField] NicknameEditView nicknameEditView;

    //====================================================================
    private void Start()
    {
        UpdateUserInfo();
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 登記更新文字事件</para>
    /// </summary>
    private void OnEnable()
    {
        MemberInformationManager.UserInfoUpdate += UpdateUserInfo;
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 解除更新文字事件</para>
    /// </summary>
    private void OnDisable()
    {
        MemberInformationManager.UserInfoUpdate -= UpdateUserInfo;
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: zxft</para>
    /// <para>詳細內容: 更新文字</para>
    /// </summary>
    private void UpdateUserInfo()
    {
        nickname_tx.text = MemberInformationManager.Nickname_str;

        StringBuilder _fullPhoneNumber = new StringBuilder();
        _fullPhoneNumber
            .Append("(").Append(CountryCode.GetCountryPhoneTitleByLocation(MemberInformationManager.Phone.location)).Append(") ")
            .Append(MemberInformationManager.Phone.number);
        //Example: (+886) 0900123456
        phoneNumber_tx.text = _fullPhoneNumber.ToString();
    }

    //====================================================================
    /// <summary>
    /// 修改暱稱
    /// <para>編輯者: Dimos</para>
    /// <para>開啟暱稱編輯視窗</para>
    /// </summary>
    public void EditNickname()
    {
        nicknameEditView.OpenView();
    }

    //====================================================================
    /// <summary>
    /// 修改密碼
    /// <para>編輯者: Dimos</para>
    /// <para>暫時尚未開放</para>
    /// </summary>
    public void EditPassword()
    {
        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_functionNotOpenYet);
    }

    //====================================================================
    /// <summary>
    /// 登出
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void Logout()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/ShoppingCart.json"))
        {
            System.IO.File.Delete(Application.persistentDataPath + "/ShoppingCart.json");
        }

        SettingData.Data.token = null;
        SettingData.UpdateData();

        SocketIOManager.Instance.DisConnect();
        SceneController.Instance.ChangeScene(SceneController.SceneNameEnum.LoginScene.ToString());
    }

    //====================================================================
    /// <summary>
    /// 離開使用者資訊場景
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void QuitUserScene()
    {
        ReturnManager.Instance.PopReturnEvent();
        SceneController.Instance.UnLoadScene(SceneController.SceneNameEnum.UserInformationScene.ToString());
    }

    //====================================================================
}
