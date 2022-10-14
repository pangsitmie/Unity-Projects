using DataStruct;
namespace ResponseStruct
{
    //--------------------------------------------
    [System.Serializable]
    public struct ResponseMessage<Data>
    {
        public Data data;
        public GraphQLError_struct[] errors;
    }

    //--------------------------------------------
    [System.Serializable]
    public class GraphQLError_struct
    {
        public string message;
        public GraphQLErrorLocations_struct locations;
        public string[] path;
        public GraphQLErrorExtensions_struct extensions;
    }
    //--------------------------------------------
    [System.Serializable]
    public class GraphQLErrorLocations_struct
    {
        public int line;
        public int column;
    }
    //--------------------------------------------
    [System.Serializable]
    public class GraphQLErrorExtensions_struct
    {
        public string description;
        public string code;
    }
    //--------------------------------------------
    [System.Serializable]
    public struct LoginResponse_struct
    {
        public MemberInfoResponse_struct signIn;
    }

    //--------------------------------------------
    [System.Serializable]
    public struct MemberInfoResponse_struct
    {
        public int id;
        public string nickname;
        public string token;
        public PhoneFormat_struct phone;
        public int lotteryTicket;
    }

    //--------------------------------------------
    [System.Serializable]
    public struct SMSResponse_struct
    {
        public string sendSMS;
    }



    //Mall======================================================
    #region Mall
    //--------------------------------------------
    [System.Serializable]
    public struct CategoriesResponse_struct
    {
        public CategoryDataResponse_struct[] allCommodityCategoryName;
    }

    //--------------------------------------------
    [System.Serializable]
    public struct CategoryDataResponse_struct
    {
        public int id;
        public string name;
    }

    //--------------------------------------------
    [System.Serializable]
    public struct commodityInfoResponse_struct
    {
        public CommodityDataResponse_struct[] commodityInfo;
    }

    //--------------------------------------------
    [System.Serializable]
    public struct CommodityDataResponse_struct
    {
        public int id;
        public int price;
        public int quantity;
        public string name;
        public string description;
        public string image;
    }

    //--------------------------------------------
    [System.Serializable]
    public struct RecipientInfoResponse_struct
    {
        public RecipientInfo_struct[] allRecipientInfo;
    }

    //--------------------------------------------
    [System.Serializable]
    public struct GetMemberInfoResponse_struct
    {
        public MemberInfoResponse_struct memberInfo;
    }

    //--------------------------------------------
    [System.Serializable]
    public struct RecipientInfo_struct
    {
        public int id;
        public string name;
        public PhoneFormat_struct phone;
        public string postalCode;
        public string countyCity;
        public string address;
    }
    #endregion
    //==========================================================

    //Order=====================================================
    #region Order
    //--------------------------------------------
    [System.Serializable]
    public struct OrderInfo_struct
    {
        public int id;

        public int memberId;
        public string orderDate;
        public int commodityTotalPrice;
        public string name;
        public PhoneFormat_struct phone;
        public string address;
        public int deliveryFee;
        public string logisticsName;
        public string deliveryId;
        public string deliveryTime;
        public OrderCommodityDetail_struct[] details;
        public OrderStatus_struct status;
    }
    //--------------------------------------------
    [System.Serializable]
    public struct OrderCommodityDetail_struct
    {
        public int id;
        public int orderId;
        public IOrderCommodityInfo_struct commodityInfo;
    }
    //--------------------------------------------
    [System.Serializable]
    public struct IOrderCommodityInfo_struct
    {
        public int unitPrice;
        public int quantity;
        public string name;
        public string discription;
        public string image;
    }
    //--------------------------------------------
    [System.Serializable]
    public struct OrderStatus_struct
    {
        public int id;
        public string statement;
    }
    //--------------------------------------------
    [System.Serializable]
    public struct OrderInfoResponse_struct
    {
        public OrderInfo_struct[] allOrderInfo;
    }
    //--------------------------------------------
    #endregion
    //==========================================================

    //Machine===================================================
    #region Machine    
    //--------------------------------------------
    [System.Serializable]
    public struct NFCPasswordResponse_struct
    {
        public string NFCPassword;
    }
    //--------------------------------------------
    [System.Serializable]
    public struct MachineTypeResponse_struct
    {
        public int type;
    }
    //--------------------------------------------
    [System.Serializable]
    public struct MachineGameResultResponse_struct
    {
        public string playDate;
        public int[] beadResult;
        public int ticket;
    }
    //--------------------------------------------
    [System.Serializable]
    public struct IPinballGameResult_struct
    {
        public int id;
        public MachineInfo_struct machineInfo;
        public string beadResult;
        public string playDate;
        public int ticket;
    }
    //--------------------------------------------
    [System.Serializable]
    public struct GameResultResponse_struct
    {
        public IPinballGameResult_struct[] gameResult;
    }
    //--------------------------------------------
    [System.Serializable]
    public struct MachineInfo_struct
    {
        public int id;
        public MachineDetail_struct details;
        public string shopCode;
        public string localCode;
    }
    //--------------------------------------------
    [System.Serializable]
    public struct MachineDetail_struct
    {
        public int id;
        public string uDId;
        public string managementId;
        public MachineTypesInfo_struct typeInfo;
    }
    //--------------------------------------------
    [System.Serializable]
    public struct MachineTypesInfo_struct
    {
        public int id;
        public string name;
    }
    //--------------------------------------------
    [System.Serializable]
    public struct MachineTypesInfoResponce_struct
    {
        public MachineTypesInfo_struct machineTypesInfo;
    }
    //--------------------------------------------
    #endregion
    //==========================================================

    //Friend====================================================
    #region Friend
    //--------------------------------------------
    [System.Serializable]
    public struct FriendListResponce_struct
    {
        public OtherMemberInfo_struct[] friendList;
    }
    //--------------------------------------------
    [System.Serializable]
    public struct FriendInviteListResponce_struct
    {
        public OtherMemberInfo_struct[] friendInviteList;
    }
    //--------------------------------------------
    [System.Serializable]
    public class PlayerSearchResponce_struct
    {
        public OtherMemberInfo_struct[] searchPlayers;
    }

    //--------------------------------------------
    [System.Serializable]
    public class OtherMemberInfo_struct
    {
        public int id;
        public string nickname;
        public string image;
    }
    //--------------------------------------------
    #endregion
    //==========================================================

    //Firebase====================================================
    #region Firebase
    //--------------------------------------------
    [System.Serializable]
    public struct UpdateFirebaseTokenResponce_struct
    {
        public FirebaseToken_struct updateRegistrationToken;
    }
    //--------------------------------------------
    [System.Serializable]
    public struct FirebaseToken_struct
    {
        public string registrationToken;
    }
    //--------------------------------------------
    #endregion
    //==========================================================
}

