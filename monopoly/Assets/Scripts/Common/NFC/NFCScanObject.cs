using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;
using UnityEngine.Localization.Components;

public class NFCScanObject : MonoBehaviour
{
    const float targetNFCScanTimeout_f = 2.0f;
    const float targetSocketTimeout_f = 5.0f;
    const float targetMatchMachineTimeout_f = 25.0f;
    [SerializeField] string status = "Waitting";
    [SerializeField] string NFCId = null;
    byte[] NFCData = new byte[4];
    [SerializeField] byte[] NFCPassword = new byte[4];
    string Type = null;
    int Page = 5;
    float currentNFCScanTimeout_f = 0;
    float currentSocketTimeout_f = 0;
    SocketIOStruct.SocketIOResponseMessage<SocketIOStruct.machineTypeResponse_struct> machineTypeResponse;
    [SerializeField] LocalizationText scanHint_tx;
    string errorMessage_str;
    bool isConnectRetry = false;
    bool haveNfcErrorConfirmView_bl = false;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        scanHint_tx.Set(LocalizationManager.KeyTable.nfc_scanning);
        SocketIOManager.Instance.OnError += ErrorEvent;
    }
    void ClearData()
    {
        scanHint_tx.Set(LocalizationManager.KeyTable.nfc_scanning);
        NFCId = "";
        NFCData = null;
        NFCPassword = null;
        Type = null;
        isConnectRetry = false;
        currentNFCScanTimeout_f = 0;
    }
    private void Update()
    {
        if (SocketIOManager.Instance.Connected)
        {
            if (currentNFCScanTimeout_f < targetNFCScanTimeout_f)
            {
                switch (status)
                {
                    case "Waitting":
                        ClearData();
                        status = "GetNFCId";
                        break;
                    case "GetNFCId":

                        if (Application.platform == RuntimePlatform.Android)
                        {
                            if (AndroidNFCScan.NowIntent().Call<String>("getAction") == "android.nfc.action.TECH_DISCOVERED")
                            {
                                try
                                {
                                    NFCId = AndroidNFCScan.ReadNFCID();
                                }
                                catch (System.Exception _exception)
                                {
                                    Debug.LogError("NFCScan ReadNFCID error: " + _exception.Message);
                                }

                            }
                        }
                        else
                        {
                            NFCId = "049a10a2105780";
                        }

                        if (!string.IsNullOrEmpty(NFCId))
                        {
                            currentNFCScanTimeout_f = 0;

                            var _ckeckID = new RequestStruct.CheckNFCIdRequest_struct()
                            {
                                NFCId = NFCId
                            };
                            status = "CheckNFCIdAndGetNFCKey";
                            SocketIOManager.Instance.SetSocketEmitEventAsync("checkNFCId", GetNFCPasswordCallback, _ckeckID);

                        }
                        break;
                    case "CheckNFCIdAndGetNFCKey":
                        scanHint_tx.Set(LocalizationManager.KeyTable.nfc_dataReading);
                        if (NFCPassword != null)
                        {
                            status = "DecodingData";
                        }
                        break;
                    case "DecodingData":
                        if (Application.platform == RuntimePlatform.Android)
                        {
                            if (AndroidNFCScan.NowIntent().Call<String>("getAction") == "android.nfc.action.TECH_DISCOVERED")
                            {
                                try
                                {
                                    NFCData = AndroidNFCScan.DecodeNFCData(NFCPassword, Page);
                                }
                                catch (System.Exception _exception)
                                {
                                    Debug.LogError("NFCScan DecodeNFCData error: " + _exception.Message);
                                }
                                if (NFCData == null)
                                {
                                    currentNFCScanTimeout_f += Time.deltaTime;
                                    Debug.Log("NFCScan Timeout: " + currentNFCScanTimeout_f);
                                }
                            }
                        }
                        else
                        {
                            NFCData = NFCdataHandler.HexStringArrayToByteArray(NFCdataHandler.StringtoStringArray("662BB164", 2));
                        }
                        if (NFCData != null)
                        {
                            currentNFCScanTimeout_f = 0;
                            Debug.Log("NFCData: " + NFCdataHandler.ByteArrayToString(NFCData));
                            var _ckeckData = new RequestStruct.CheckNFCDataRequest_struct()
                            {
                                NFCId = NFCId,
                                NFCData = NFCdataHandler.ByteArrayToString(NFCData)
                            };
                            status = "CheckData";
                            SocketIOManager.Instance.SetSocketEmitEventAsync("checkNFCData", CheckNFCDataCallback, _ckeckData);

                            scanHint_tx.Set(LocalizationManager.KeyTable.nfc_requestingPairing);
                        }
                        break;
                    case "CheckData":

                        if (currentSocketTimeout_f < targetSocketTimeout_f)
                        {
                            currentSocketTimeout_f += Time.deltaTime;
                        }
                        else
                        {
                            currentSocketTimeout_f = 0;
                            var _ckeckData = new RequestStruct.CheckNFCDataRequest_struct()
                            {
                                NFCId = NFCId,
                                NFCData = NFCdataHandler.ByteArrayToString(NFCData)
                            };
                            SocketIOManager.Instance.SetSocketEmitEventAsync("checkNFCData", CheckNFCDataCallback, _ckeckData);
                        }
                        break;
                    case "MatchMachine":
                        var _matchMachineID = new RequestStruct.CheckNFCIdRequest_struct()
                        {
                            NFCId = NFCId
                        };
                        status = "Matching";
                        SocketIOManager.Instance.SetSocketEmitEventAsync("matchMachine", MatchMachineCallback, _matchMachineID);
                        break;
                    case "Matching":
                        if (currentSocketTimeout_f < targetMatchMachineTimeout_f)
                        {
                            currentSocketTimeout_f += Time.deltaTime;
                        }
                        else
                        {
                            currentSocketTimeout_f = 0;
                            Debug.LogError("Matching faild");
                            errorMessage_str = "Matching faild";
                            status = "Error";
                        }
                        break;
                    case "Matched":
                        MachineInformationManager.Instance.SetMachineData(machineTypeResponse.payload);
                        QuitView();
                        break;
                    case "Error":
                        //NotificationManager.Instance.CreateConfirmView(LocalizationManager.KeyTable.common_handleFail);

                        GameObject.Find("MainCanvas/SocketErrorMessageView/ConfirmView/Message_Text").GetComponent<Text>().text = errorMessage_str;
                        GameObject.Find("MainCanvas/SocketErrorMessageView").GetComponent<MoveOut>().Move(true);
                        Destroy(this.gameObject);
                        break;
                }
            }
            else
            {
                if (!haveNfcErrorConfirmView_bl)
                {
                    haveNfcErrorConfirmView_bl = false;
                    NotificationManager.Instance.CreateConfirmView(LocalizationManager.KeyTable.nfc_nfcError);
                }

                Destroy(this.gameObject);
            }
        }
        else
        {
            if (!isConnectRetry)
            {
                isConnectRetry = true;
                scanHint_tx.Set(LocalizationManager.KeyTable.common_socketReconnect);
                SocketIOManager.Instance.ConnectRetry();
            }
        }
    }

    //====================================================================
    /// <summary>
    /// NFC金鑰獲取成功
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void GetNFCPasswordCallback(SocketIOClient.SocketIOResponse _response)
    {
        if (status == "CheckNFCIdAndGetNFCKey")
        {
            Debug.Log("GetNFCPassword: " + _response.ToString());
            SocketIOStruct.SocketIOResponseMessage<SocketIOStruct.NFCPasswordResponse_struct> _responseData;
            try
            {
                _responseData = JsonConvert.DeserializeObject<List<SocketIOStruct.SocketIOResponseMessage<SocketIOStruct.NFCPasswordResponse_struct>>>(_response.ToString())[0];
            }
            catch (Exception ex)
            {
                Debug.LogError("NFCPassword parse error: " + ex);
                errorMessage_str = "資料處理失敗";
                status = "Error";
                return;
            }

            switch (_responseData.status)
            {
                case "0x000":
                    Debug.Log("NFCPassword parse: " + _responseData.payload.NFCPassword.ToString());
                    NFCPassword = NFCdataHandler.HexStringArrayToByteArray(NFCdataHandler.StringtoStringArray(_responseData.payload.NFCPassword, 2));
                    break;
                default:
                    errorMessage_str = _responseData.message;
                    status = "Error";
                    break;
            }
        }
        else
        {
            Debug.Log("Duplicate GetNFCPassword: " + _response.ToString());
        }
    }

    //====================================================================
    /// <summary>
    /// 配對NFC成功
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void CheckNFCDataCallback(SocketIOClient.SocketIOResponse _response)
    {
        if (status == "CheckData")
        {
            Debug.Log("GetMachineType(): " + _response.ToString());

            try
            {
                machineTypeResponse = JsonConvert.DeserializeObject<List<SocketIOStruct.SocketIOResponseMessage<SocketIOStruct.machineTypeResponse_struct>>>(_response.ToString())[0];
            }
            catch (Exception ex)
            {
                Debug.LogError("GetMachineType parse error: " + ex);
                errorMessage_str = "資料處理失敗";
                status = "Error";
                return;
            }

            switch (machineTypeResponse.status)
            {
                case "0x000":
                    status = "MatchMachine";
                    break;
                default:
                    errorMessage_str = machineTypeResponse.message;
                    status = "Error";
                    break;
            }
        }
        else
        {
            Debug.Log("Duplicate GetMachineType(): " + _response.ToString());
        }
    }

    //====================================================================
    /// <summary>
    /// 配對機台成功
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void MatchMachineCallback(SocketIOClient.SocketIOResponse _response)
    {
        if (status == "Matching")
        {
            Debug.Log("MatchMachineCallback(): " + _response.ToString());

            try
            {
                machineTypeResponse = JsonConvert.DeserializeObject<List<SocketIOStruct.SocketIOResponseMessage<SocketIOStruct.machineTypeResponse_struct>>>(_response.ToString())[0];
            }
            catch (Exception ex)
            {
                Debug.LogError("MatchMachineCallback parse error: " + ex);
                errorMessage_str = "資料處理失敗";
                status = "Error";
                return;
            }

            switch (machineTypeResponse.status)
            {
                case "0x000":
                    status = "Matched";
                    break;
                default:
                    errorMessage_str = machineTypeResponse.message;
                    status = "Error";
                    break;
            }
        }
        else
        {
            Debug.Log("Duplicate MatchMachineCallback(): " + _response.ToString());
        }
    }
    //====================================================================
    /// <summary>
    /// 觸發錯誤事件
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void ErrorEvent(SocketIOClient.SocketIOResponse _response)
    {
        errorMessage_str = "資料處理失敗";
        status = "Error";
    }

    //====================================================================
    /// <summary>
    /// 關閉是視窗
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void QuitView()
    {
        Destroy(this.gameObject);
        SocketIOManager.Instance.OnError -= ErrorEvent;
        ReturnManager.Instance.PopReturnEvent();
    }
}
