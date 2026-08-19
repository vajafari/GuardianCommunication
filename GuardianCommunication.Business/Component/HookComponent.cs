using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GuardianCommunication.Business.Cache;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.ExternalServices.Hooks;
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
                AuthorizationUsername = hookDefinitions.AuthorizationPassword,
                AuthorizationPassword = hookDefinitions.AuthorizationPassword,
                DateFormat = hookDefinitions.DateFormat,

            }, model);
        }

        #endregion


    }
}
