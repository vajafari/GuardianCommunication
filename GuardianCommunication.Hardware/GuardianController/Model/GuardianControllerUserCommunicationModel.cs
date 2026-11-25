using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// کاربر با گروه‌های دسترسی و دسترسی‌های آسانش روی همین برد؛ بدنه POST api/v1/users/upsert در Standalone
    /// و محتوای دستور SetUserInfo در Push. فهرست‌ها جایگزین فهرست‌های قبلی کاربر روی برد می‌شوند
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerUserCommunicationModel
    {
        /// <summary>
        /// شماره کاربر روی دستگاه
        /// </summary>
        public long user_id_on_device { get; set; }

        public string name { get; set; }

        /// <summary>
        /// 1 آقا، 2 خانم، خالی برای کاربری که شخص نیست
        /// </summary>
        public short? gender { get; set; }

        public bool is_enable { get; set; }

        /// <summary>
        /// شروع اعتبار به میلی‌ثانیه epoch (UTC)
        /// </summary>
        public long start_date { get; set; }

        /// <summary>
        /// پایان اعتبار به میلی‌ثانیه epoch (UTC)؛ خالی یعنی بدون پایان
        /// </summary>
        public long? end_date { get; set; }

        public List<string> rf_card_numbers { get; set; }
        public string password { get; set; }
        public List<Guid> access_group_ids { get; set; }
        public List<GuardianControllerEasyAccessCommunicationModel> easy_accesses { get; set; }
    }
}
