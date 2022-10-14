using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
/// <summary>
/// 負責主頁(MainScene)的初始化
/// </summary>
public class MainPageInitialize : MonoBehaviour
{
    [SerializeField] Text nickname_tx;
    [SerializeField] Text ticketAmount_tx;
    [SerializeField] NFCScanObject nfcScanObject_pf;
    [SerializeField] MachineInformationUI machineInformationUI_pf;
    [SerializeField] Transform machineInformationUIContent_transform;
    [SerializeField] LoadingObjectManager loadingObjectManager;
    MachineInformationUI machineInformationUI;
    //====================================================================
    private void Start()
    {
        UpdateUserInfo();
        MachineInformationManager.Instance.OnMachineMatch.AddListener(OnMachineMatch);
        OnMachineMatch();
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
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 更新文字</para>
    /// </summary>
    public void UpdateUserInfo()
    {
        Debug.Log("UpdateUserInfo Text: " + MemberInformationManager.Nickname_str);
        nickname_tx.text = MemberInformationManager.Nickname_str;
        ticketAmount_tx.text = MemberInformationManager.Ticket_int.ToString();
    }
    //====================================================================
    public void OpenPlayRecord()
    {
        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_functionNotOpenYet);
    }
    //====================================================================
    public void OpenMachineRecord()
    {
        NotificationManager.Instance.CreateHintMessage(LocalizationManager.KeyTable.common_functionNotOpenYet);
    }
    //====================================================================
    private EventView closeView;

    /// <summary>
    /// 於登入場景點擊返回鍵後的顯示與處理(此函式由ReturnEventHandler添加)
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void AddCloseEvent()
    {
        closeView = NotificationManager.Instance.CreateEventView(LocalizationManager.KeyTable.common_exitComfirmViewText, CloseViewConfirmCallback, CloseViewReturnEvent);
        UnityEvent _CloseViewReturnEvent = new UnityEvent();
        _CloseViewReturnEvent.AddListener(CloseViewReturnEvent);
        ReturnManager.Instance.PushReturnEvent(_CloseViewReturnEvent);
    }

    //====================================================================
    private void CloseViewReturnEvent()
    {
        Destroy(closeView.gameObject);
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
    private void CloseViewConfirmCallback()
    {
        Application.Quit();
        ReturnManager.Instance.PopReturnEvent();
    }

    //====================================================================
    public void OpenNFCScanView()
    {
        switch (MachineInformationManager.Instance.status)
        {
            case "未連接":
                Instantiate(nfcScanObject_pf, SceneController.Instance.CurrentSceneCanvas.transform);
                break;
        }

    }

    //====================================================================
    private void OnMachineMatch()
    {
        switch (MachineInformationManager.Instance.status)
        {
            case "未連接":
                break;
            case "等待遊戲":
            case "遊戲中":
                if (machineInformationUI == null)
                    machineInformationUI = Instantiate(machineInformationUI_pf, machineInformationUIContent_transform);
                machineInformationUI.Load(MachineInformationManager.Instance.status, MachineInformationManager.Instance.machineData, MachineInformationManager.Instance.connectDateTime, ClientDisconnect, loadingObjectManager);
                break;
        }
    }
    //====================================================================
    public void ClientDisconnect(System.Action _DisconnectSuccess)
    {
        MachineInformationManager.Instance.ClientDisconnect(_DisconnectSuccess);
    }

}
