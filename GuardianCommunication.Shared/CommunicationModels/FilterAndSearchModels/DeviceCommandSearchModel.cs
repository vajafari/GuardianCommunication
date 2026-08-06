namespace GuardianCommunication.Shared.CommunicationModels.FilterAndSearchModels
{
	public class DeviceCommandSearchModel
	{
		public DeviceCommandFilterModel Filter { get; set; }

		public CurrentPageModel CurrentPage { get; set; }

		public DeviceCommandSortModel[] SortInfos { get; set; }
	}
}
