using System;
using System.Collections.Generic;
using GuardianCommunication.Business.Cache;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.ExternalServices.Hooks;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.SearchDataWrapper;

namespace GuardianCommunication.Business.Component
{
    public class HookComponent : BaseComponent
    {

        public HookComponent(RepositoryFactory sharedRepository)
            : base(sharedRepository)
        { }

        #region HookDefinition


        public List<DtoHookDefinition> SearchHookDefinition(PagingData<HookDefinitionFilter, HookDefinitionSortEnumeration> searchInfo, bool attachDetail)
        {
            return RepositoryFactory.GetHookDefinitionRepository().Search(searchInfo);
        }

        #region Internal methods


        internal List<DtoHookDefinition> GetHookDefinitionCache()
        {
            return GuardianCommunicationInMemoryCacheWrapper.Instance.GetHookDefinitions();
        }


        #endregion


        #endregion


        #region Hook actions

        internal dynamic CallHookApi<T>(DtoHookDefinition hookDefinitions, T model)
        {
            var hookingServiceGeneral = new HookingService();
            return hookingServiceGeneral.CallHook(new HookSystemModel
            {
                AuthorizationType = hookDefinitions.AuthorizationType,
                HeaderTemplate = hookDefinitions.HeaderTemplate,
                BodyTemplate = hookDefinitions.BodyTemplate,
                QueryStringTemplate = hookDefinitions.QueryStringTemplate,
                EndPointUrl = hookDefinitions.EndPointUrl,
                HttpMethod = hookDefinitions.HttpMethod,
                RequestTimeout = TimeSpan.FromSeconds(hookDefinitions.RequestTimeoutInSeconds),
                AuthorizationUsername = hookDefinitions.AuthorizationUsername,
                AuthorizationPassword = hookDefinitions.AuthorizationPassword,
                DateFormat = hookDefinitions.DateFormat,

            }, model);
        }

        #endregion


        #region Cron

        /// <summary>
        /// یک نوبت ردیف زمان بند: آدرس ردیف صدا زده می شود و بعد زمان شروع همین نوبت LastRunAt ردیف می شود، چه فراخوانی موفق باشد چه نه؛
        /// خطا فقط در لاگ ثبت می شود و نوبت بعدی طبق Cron می آید (کارهای زمان بند خودشان نوبت جاافتاده را جبران می کنند).
        /// آدرسی که با / شروع شود مسیری روی Guardian است و با آدرس و کاربر Guardian در تنظیمات سرویس صدا زده می شود (همان که بقیه
        /// فراخوانی های Guardian دارند)، و آدرس کامل با احراز هویت خود ردیف
        /// </summary>
        internal void RunCronHook(DtoHookDefinition hookDefinition, DateTime runAt)
        {
            try
            {
                var guardianApiConfig = hookDefinition.EndPointUrl.StartsWith("/", StringComparison.Ordinal)
                    ? new SystemConfigComponent(RepositoryFactory).GetGuardianApiConfig()
                    : null;
                CallHookApi<object>(guardianApiConfig == null
                    ? hookDefinition
                    : new DtoHookDefinition
                    {
                        EndPointUrl = ApiCallHelpers.CombineUri(guardianApiConfig.BaseUri, hookDefinition.EndPointUrl),
                        HttpMethod = hookDefinition.HttpMethod,
                        AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                        AuthorizationUsername = guardianApiConfig.Username,
                        AuthorizationPassword = guardianApiConfig.Password,
                        QueryStringTemplate = hookDefinition.QueryStringTemplate,
                        HeaderTemplate = hookDefinition.HeaderTemplate,
                        BodyTemplate = hookDefinition.BodyTemplate,
                        DateFormat = hookDefinition.DateFormat,
                        RequestTimeoutInSeconds = hookDefinition.RequestTimeoutInSeconds,
                    }, null);
            }
            catch (Exception exp)
            {
                // کلمه عبور در لاگ نمی آید
                LoggingSystem.LogHookError(exp, new { hookDefinition.Id, hookDefinition.EndPointUrl, hookDefinition.Cron, RunAt = runAt });
            }
            RepositoryFactory.GetHookDefinitionRepository().UpdateLastRunAt(hookDefinition.Id, runAt);
        }

        #endregion


    }
}
