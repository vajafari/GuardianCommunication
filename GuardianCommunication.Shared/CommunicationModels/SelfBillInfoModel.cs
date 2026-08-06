using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class SelfBillInfoModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public int DeviceNumber { get; set; }

        [DataMember]
        public string PrinterName { get; set; }

        [DataMember]
        public string PrinterIp { get; set; }

        [DataMember]
        public string AdditionalData { get; set; }


        #region Headers

        [DataMember]
        public List<string> HeaderTitles { get; set; }

        [DataMember]
        public string EmployeeTitle { get; set; }

        [DataMember]
        public double IssueDate { get; set; }

        #endregion


        #region Detail

        [DataMember]
        public List<SelfBillFoodInfoModel> FoodInfos { get; set; }

        [DataMember]
        public string FishNumber { get; set; }

        [DataMember]
        public string ManualBill { get; set; }

        #endregion


    }

}
