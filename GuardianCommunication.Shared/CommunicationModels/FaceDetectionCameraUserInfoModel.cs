using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class FaceDetectionCameraUserInfoModel
    {
        [DataMember]
        public long EmployeeNumber { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public List<string> CameraImages { get; set; }
        [DataMember]
        public double StartDate { get; set; }
        [DataMember]
        public double? EndDate { get; set; }
        [DataMember]
        public DeviceUserTypeEnumeration UserType { get; set; }
        [DataMember]
        public double? VisibilityTime { get; set; }
        [DataMember]
        public Guid? Identifier { get; set; }
        [DataMember]
        public CommandPriorityEnumeration? Priority { get; set; }
    }

}
