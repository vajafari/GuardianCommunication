using System;
using System.Collections.Generic;
using GuardianCommunication.Shared.ExtensionsAndUtilities;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoSelfBillInfo
    {

        public long Id { get; set; }
        
        public int DeviceNumber { get; set; }

        public string PrinterName { get; set; }

        public string PrinterIp { get; set; }

        public string AdditionalData { get; set; }


        #region Headers

        public List<string> HeaderTitles { get; set; }

        public string EmployeeTitle { get; set; }

        public DateTime IssueDate { get; set; }

        #endregion


        #region Detail

        public List<DtoSelfBillFoodInfo> FoodInfos { get; set; }

        public string FishNumber { get; set; }

        public string ManualBill { get; set; }

        public string GetIssueDateString()
        {
            return $"{IssueDate.ToPersianDateTimeString("yyyy/MM/dd       HH:mm:ss")}";
        }

        #endregion

    }
}
