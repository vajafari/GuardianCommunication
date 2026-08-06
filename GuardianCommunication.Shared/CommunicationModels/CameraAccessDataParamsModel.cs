using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class CameraAccessDataParamsModel
    {
        [DataMember]
        public int CameraId { get; set; }
        [DataMember]
        public double StartDate { get; set; }
        [DataMember]
        public double EndDate { get; set; }

    }
}
