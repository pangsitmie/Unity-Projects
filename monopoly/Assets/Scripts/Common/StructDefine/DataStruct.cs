using System.Collections;
using System.Collections.Generic;
namespace DataStruct
{
    [System.Serializable]
    public struct TaiwanDistrictsData_struct
    {
        public CountryData_struct[] taiwan_districts;
    }
    [System.Serializable]
    public struct CountryData_struct
    {
        public string name;
        public DistrictsData_struct[] districts;
    }
    [System.Serializable]
    public struct DistrictsData_struct
    {
        public string zip;
        public string name;
    }
    [System.Serializable]
    public class PhoneFormat_struct
    {
        public string number;
        public string location;
    }
    public struct CommodityCheckoutData_struct
    {
        public ResponseStruct.CommodityDataResponse_struct commodityData;
        public int amount;
    }
}
