using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuardianCommunication.Shared.Dto
{

    public class DtoDeviceSettings
    {
        /// <summary>
        /// آیا Server Matching در دستگاه فعال است
        /// </summary>
        public bool IsServerMatch { get; set; } = false;

        /// <summary>
        /// عدم ذخیره سازی تردد‌های مجاز
        /// </summary>
        public bool DontSaveAttendance { get; set; } = false;

        /// <summary>
        /// عدم ذخیره سازی تردد‌های نامجاز
        /// </summary>
        public bool DontSaveInvalidAttendance { get; set; } = false;

        /// <summary>
        /// عدم ذخیره سازی رویدادها
        /// </summary>
        public bool DontSaveEvents { get; set; } = false;

        /// <summary>
        /// دستگاه نمونه گیری
        /// </summary>
        public bool IsMasterDevice { get; set; } = false;

        /// <summary>
        /// جمع‌آوری خودکار داده‌ها
        /// </summary>
        public bool IsAutomaticDataCollectActive { get; set; } = false;

        /// <summary>
        /// ارسال عکس پروفایل به دستگاه
        /// </summary>
        public bool IsSendProfileImageActive { get; set; } = false;

        /// <summary>
        /// آیا هوک دستگاه 
        /// </summary>
        public bool IsHookActive { get; set; } = false;


        public DtoSuprema1DeviceSettings Suprema1DeviceSettings { get; set; } = new DtoSuprema1DeviceSettings();
        public DtoSuprema2DeviceSettings Suprema2DeviceSettings { get; set; } = new DtoSuprema2DeviceSettings();
        public DtoTimyDeviceSettings TimyDeviceSettings { get; set; } = new DtoTimyDeviceSettings();
        public DtoZkDeviceSettings ZkDeviceSettings { get; set; } = new DtoZkDeviceSettings();
    }


    public class DtoZkDeviceSettings
    {
        public bool IsZkOldName { get; set; } = false;
        public bool IsOldVersion { get; set; } = false;
    }

    public class DtoSuprema1DeviceSettings
    {
        public bool IsSupremaAutoCollectEventsActive { get; set; } = false;
    }

    public class DtoSuprema2DeviceSettings
    {
        public bool IsSupremaAutoCollectEventsActive { get; set; } = false;
    }

    public class DtoTimyDeviceSettings
    {
        public bool IsUserId32Bit { get; set; } = false;
    }
}
