using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocketIOStruct
{
    [System.Serializable]
    public struct SocketIOResponseMessage<Data>
    {
        public string status;
        public string message;
        public Data payload;
    }

    [System.Serializable]
    public struct NFCPasswordResponse_struct
    {
        public string NFCPassword;
    }
    [System.Serializable]
    public struct machineTypeResponse_struct
    {
        public int machineType;
    }

}
