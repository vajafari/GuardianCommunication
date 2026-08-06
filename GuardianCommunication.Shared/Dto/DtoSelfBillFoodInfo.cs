using System.Collections.Generic;
using GuardianCommunication.Shared.ExtensionsAndUtilities;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoSelfBillFoodInfo
    {

        public string FoodType { get; set; }

        public string FoodTitle { get; set; }

        public string FishCount { get; set; }

        public string Price { get; set; }

        public string Description { get; set; }

        public bool IsAllowed { get; set; }

        public string Separator { get; set; }

        public string GetShowString()
        {
            var items = new List<string>();
            if (FoodType.IsNotNullOrEmpty())
            {
                items.Add(FoodType);
            }
            if (FoodTitle.IsNotNullOrEmpty())
            {
                items.Add(FoodTitle);
            }
            if (FishCount.IsNotNullOrEmpty())
            {
                items.Add(FishCount);
            }
            if (Price.IsNotNullOrEmpty())
            {
                items.Add(Price);
            }
            return string.Join(Separator ?? "  ", items);
        }

    }
}
