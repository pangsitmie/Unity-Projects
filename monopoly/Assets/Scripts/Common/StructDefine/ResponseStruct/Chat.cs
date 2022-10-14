using System;

namespace ResponseStruct.Chat
{
    namespace PrivateChat
    {
        //PrivateChat===============================================
        #region PrivateChat
        //--------------------------------------------
        [System.Serializable]
        public struct FetchLastPrivateMsgResponce_struct
        {
            public PrivateChatRecord_struct[] fetchLastPrivateMsg;
        }
        //--------------------------------------------
        [System.Serializable]
        public struct FetchPrivateChatRecord_struct
        {
            public PrivateChatRecord_struct[] fetchPrivateChatRecord;
        }
        //--------------------------------------------
        [System.Serializable]
        public struct SendPrivateMsgResponce_struct
        {
            public PrivateChatRecord_struct sendPrivateMsg;
        }
        //--------------------------------------------
        [System.Serializable]
        public class PrivateChatRecord_struct
        {
            public int id;
            public OtherMemberInfo_struct initiator;
            public OtherMemberInfo_struct target;
            public MessageType_struct messageType;
            public string dateTime;
            public string message;
            public PointTransactionStatus_struct pointTransactionInfo;
        }
        //--------------------------------------------
        [System.Serializable]
        public struct PointTransactionInfo_struct
        {
            public int id;
            public OtherMemberInfo_struct initiator;
            public OtherMemberInfo_struct target;
            public int pointNum;
            public string createDateTime;
            public string expirationDateTime;
            public PointTransactionStatus_struct status;
        }
    }
    //--------------------------------------------
    [System.Serializable]
    public struct PointTransactionStatus_struct
    {
        public int id;
        public string statement;
    }
    //--------------------------------------------
    [System.Serializable]
    public struct MessageType_struct
    {
        public int id;
        public string statement;
    }
    //--------------------------------------------
    #endregion
    //==========================================================
}
