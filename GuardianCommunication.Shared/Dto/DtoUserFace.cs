using GuardianCommunication.Shared.ExtensionsAndUtilities;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoUserFace
    {
		public long UserIdOnDevice { get; set; }
		public byte[] TemplateData { get; set; }
		public int FaceIndex { get; set; }
		public uint CheckSum { get; set; }
		public int Length { get; set; }
        public string AdditionalDataInJson { get; set; }

        public DtoUserTemplateAdditionalData AdditionalDataProcessed { get; set; }

        private DtoUserTemplateAdditionalData _additionalData;
        public DtoUserTemplateAdditionalData AdditionalData
        {
            get
            {
                if (_additionalData == null && AdditionalDataInJson.IsNotNullOrEmpty())
                {
                    _additionalData = ObjectHelper.DeserializeAsJson<DtoUserTemplateAdditionalData>(AdditionalDataInJson)
                                      ?? new DtoUserTemplateAdditionalData();
                }
                return _additionalData;
            }
            set => _additionalData = value;
        }


        public void UpdateDeviceSettingsJson()
        {
            AdditionalDataInJson = _additionalData == null ? null : ObjectHelper.SerializeAsJson(_additionalData);
        }


    }




}
