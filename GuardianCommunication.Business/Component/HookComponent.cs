using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GuardianCommunication.Business.Cache;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.ExternalServices.Hooks;

namespace GuardianCommunication.Business.Component
{
    public class HookComponent : BaseComponent
    {

        public HookComponent(RepositoryFactory sharedRepository)
            : base(sharedRepository)
        { }

        #region HookSystem


        public List<DtoHookSystem> SearchHookSystem(PagingData<HookSystemFilter, HookSystemSortEnumeration> searchInfo, bool attachDetail)
        {
            var result = RepositoryFactory.GetHookSystemRepository().Search(searchInfo);
            if (result.IsCollectionNotNullOrEmpty() && attachDetail)
            {
                var details = SearchHookSystemDetail(new PagingData<HookSystemDetailFilter, HookSystemDetailSortEnumeration>
                {
                    Filter = new HookSystemDetailFilter
                    {
                        HookSystemIds = result.Select(row => row.Id).ToList()
                    }
                });
                foreach (var item in result)
                {
                    item.Details = details.Where(row => row.HookSystemId == item.Id).ToList();
                }
            }
            return result;
        }

        #region Internal methods


        internal List<DtoHookSystem> SearchHookSystemCache(Expression<Func<DtoHookSystem, bool>> expression)
        {
            return CacheWrapper.Instance.HookSystemCacheManager.Filter(expression.Compile()).ToList();
        }

        internal void UpdateHookSystem(DtoHookSystem entity)
        {
            RepositoryFactory.GetHookSystemRepository().Update(entity);
            ResetHookSystemCache(new List<int> { entity.Id });
        }

        #endregion


        #region Private method


        private static void ResetHookSystemCache(List<int> ids)
        {
            CacheWrapper.Instance.ResetHookSystemCache(ids);
        }


        #endregion

        #endregion


        #region HookSystemDetail


        public List<DtoHookSystemDetail> SearchHookSystemDetail(PagingData<HookSystemDetailFilter, HookSystemDetailSortEnumeration> searchInfo)
        {
            return RepositoryFactory.GetHookSystemDetailRepository().Search(searchInfo);
        }

        #endregion


        #region Hook actions

        internal dynamic CallHookApi<T>(DtoHookSystem system, DtoHookSystemDetail detail, T model)
        {
            var hookingServiceGeneral = new HookingService();
            return hookingServiceGeneral.CallHook(new HookSystemModel
            {
                AuthorizationType = system.AuthorizationType,
                HeaderTemplate = detail.HeaderTemplate,
                BodyTemplate = detail.BodyTemplate,
                QueryStringTemplate = detail.QueryStringTemplate,
                EndPointUrl = detail.EndPointUrl,
                HttpMethod = detail.HttpMethod,
                RequestTimeout = TimeSpan.FromSeconds(detail.RequestTimeoutInSeconds),
                AuthorizationBearerToken = system.AuthorizationToken,
                AuthorizationUsername = system.AuthorizationUsername,
                AuthorizationPassword = system.AuthorizationPassword,
                DateFormat = detail.DateFormat,

            }, model);
        }

        #endregion


    }
}
