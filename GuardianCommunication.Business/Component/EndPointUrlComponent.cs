using System.Collections.Generic;
using GuardianCommunication.Data.Repository;

namespace GuardianCommunication.Business.Component
{
	// Tested
	public class EndPointUrlComponent : BaseComponent
	{
		public EndPointUrlComponent(RepositoryFactory sharedRepository)
			: base(sharedRepository)
		{ }



		public void Update(DtoEndPointUrl entity)
		{
			var repo = RepositoryFactory.GetEndPointUrlRepository();
			var entityInDatabase = (repo.GetByName(entity.EndPointName));
			if (entityInDatabase == null)
			{
				throw new OperationCannotBeDoneException(
					CommunicationSharedResource.ObjectSerial.FormatInvariantCulture(entity.EndPointName),
					OperationResultEnumeration.CommunicationStatusSettingErrorEndPointUrlStatusEndPointUrlNotFoundInDatabase);
			}

			WriteSourceValuesToInDatabaseObject(entityInDatabase, entity);

			#region Logical Validations

			LogicalValidation(entityInDatabase);

			#endregion

			ObjectHelper.OverWriteAllValues(entity, entityInDatabase);

			RepositoryFactory.GetEndPointUrlRepository().Update(new List<DtoEndPointUrl> { entityInDatabase });

		}

		public List<DtoEndPointUrl> GetAll()
		{
			return RepositoryFactory.GetEndPointUrlRepository().GetAll();
		}


		#region Private Methods


		private static void LogicalValidation(DtoEndPointUrl entity)
		{
			if (entity == null)
			{
				throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusObjectIsNull);
			}

			var result = new List<OperationResultEnumeration>();
			if (entity.EndPointName.IsNullOrEmpty())
			{
				result.Add(OperationResultEnumeration.CommunicationStatusSettingErrorEndPointUrlStatusEndPointUrlNumberIsNotValid);
			}

			if (result.Count > 0)
			{
				throw new OperationCannotBeDoneException(string.Format(CommunicationSharedResource.ObjectSerial, entity.EndPointName),
					result.ToArray());
			}


		}



		private static void WriteSourceValuesToInDatabaseObject(DtoEndPointUrl entityInDatabase, DtoEndPointUrl entity)
		{
			entityInDatabase.EndPointUrl = entity.EndPointUrl;
		}


		#endregion




	}
}
