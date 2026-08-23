using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class UserAndDeviceListModel
    {
        public List<UserAndDeviceModel> Records { get; set; }
    }
}
