using DataStruct;
namespace RequestStruct
{
    [System.Serializable]
    public struct LoginRequest_struct
    {
        public PhoneFormat_struct phone;
        public string password;
        public string registrationToken;
    }
    [System.Serializable]
    public struct SMSRequest_struct
    {
        public PhoneFormat_struct phone;
        public int type;
    }
    [System.Serializable]
    public struct SignUpRequest_struct
    {
        public PhoneFormat_struct phone;
        public string password;
        public string verificationCode;
        public string nickname;
    }
    [System.Serializable]
    public struct CommodityFilterRequest_struct
    {
        public int categoryId;
        public int startId;
        public int quantity;
        public SortedMethod_struct sort;
    }
    [System.Serializable]
    public struct SortedMethod_struct
    {
        public string order;
        public string field;
    }
    [System.Serializable]
    public struct RecipientInformation_struct
    {
        public string name;
        public PhoneFormat_struct phone;
        public string postalCode;
        public string countyCity;
        public string address;
    }
    [System.Serializable]
    public struct OrderCommodity_struct
    {
        public int commodityId;
        public int quantity;
    }
    [System.Serializable]
    public struct CheckNFCIdRequest_struct
    {
        public string NFCId;
    }
    [System.Serializable]
    public struct CheckNFCDataRequest_struct
    {
        public string NFCId;
        public string NFCData;
    }
}
