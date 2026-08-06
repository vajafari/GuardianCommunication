using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class SelfBillFoodInfoModel
    {
        [DataMember]
        public string FoodType { get; set; }

        [DataMember]
        public string FoodTitle { get; set; }

        [DataMember]
        public string FishCount { get; set; }

        [DataMember]
        public string Price { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool IsAllowed { get; set; }

        [DataMember]
        public string Separator { get; set; }
    }

}
