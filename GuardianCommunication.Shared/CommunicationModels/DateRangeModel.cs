using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	
	[DataContract]
	public class DateRangeModel
	{
		[DataMember]
		public double StartDate { get; set; }

        [DataMember]
        public double EndDate { get; set; }

    }
}
