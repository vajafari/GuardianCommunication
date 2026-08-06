using GuardianCommunication.Data.Repository;

namespace GuardianCommunication.Service
{
	public class BaseService
	{

		#region Protected Methods


		protected static RepositoryFactory GetRepositoryFactory()
		{
			return new RepositoryFactory();
		}

		#endregion

	}
}
