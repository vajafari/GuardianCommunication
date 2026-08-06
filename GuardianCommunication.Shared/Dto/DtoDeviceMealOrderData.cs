using System;
using System.Collections.Generic;
using System.Linq;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoDeviceMealOrderData
    {
        public int DeviceNumber { get; set; }
        public int FoodMealId { get; set; }
        public int RestaurantId { get; set; }
        public int RestaurantQueueId { get; set; }
        public DateTime StartDateTime { get; set; }
        public List<string> FoodTitles { get; set; } = new List<string>();


        protected bool Equals(DtoDeviceMealOrderData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != this.GetType()) return false;

            return DeviceNumber == other.DeviceNumber
                   && FoodMealId == other.FoodMealId
                   && RestaurantId == other.RestaurantId
                   && RestaurantQueueId == other.RestaurantQueueId
                   && StartDateTime == other.StartDateTime
                   && FoodTitles.SequenceEqual(other.FoodTitles);
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is DtoDeviceMealOrderData data)
            {
                return Equals(data);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = DeviceNumber;
                hashCode = (hashCode * 397) ^ RestaurantId;
                hashCode = (hashCode * 397) ^ StartDateTime.GetHashCode();
                hashCode = (hashCode * 397) ^ (FoodTitles != null ? FoodTitles.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(DtoDeviceMealOrderData a, DtoDeviceMealOrderData b)
        {
            if (a is null)
            {
                return b is null;
            }

            return a.Equals(b);

        }
        public static bool operator !=(DtoDeviceMealOrderData a, DtoDeviceMealOrderData b)
        {
            return !(a == b);
        }

    }
}
