namespace GuardianCommunication.Shared.CommunicationModels
{
    public class DeviceMealOrderDataModel
    {
        public int DeviceNumber { get; set; }
        public int RestaurantNumber { get; set; }
        public int MealOrderStartDate { get; set; }
        public string[] Meals { get; set; }
    }
}
