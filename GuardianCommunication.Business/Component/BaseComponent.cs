using GuardianCommunication.Data.Repository;

namespace GuardianCommunication.Business.Component
{
	public class BaseComponent
	{
		protected RepositoryFactory RepositoryFactory { get; }

		protected BaseComponent(RepositoryFactory repositoryFactory)
		{
			RepositoryFactory = repositoryFactory;
		}

		

		protected BaseComponent()
			: this(null)
		{ }


	}
}
