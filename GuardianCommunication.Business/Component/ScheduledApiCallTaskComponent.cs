using System.Collections.Generic;
using GuardianCommunication.Data.Repository;

namespace GuardianCommunication.Business.Component
{
    public class ScheduledApiCallTaskComponent : BaseComponent
    {

        public ScheduledApiCallTaskComponent(RepositoryFactory sharedRepository)
            : base(sharedRepository)
        { }


        public List<DtoScheduledApiCallTask> Search(PagingData<ScheduledApiCallTaskFilter, ScheduledApiCallTaskSortEnumeration> searchInfo)
        {
            return RepositoryFactory.GetScheduledApiCallTaskRepository().Search(searchInfo);
        }

        public void Insert(DtoScheduledApiCallTask entity)
        {
            RepositoryFactory.GetScheduledApiCallTaskRepository().Insert(entity);
        }

        public void Update(DtoScheduledApiCallTask entity)
        {
            RepositoryFactory.GetScheduledApiCallTaskRepository().Update(entity);
        }

        public void Delete(int id)
        {
            RepositoryFactory.GetScheduledApiCallTaskRepository().Delete(id);
        }

    }
}
