using System.ServiceModel;

//using Communication.ServiceShared.SharedSettings;

namespace GuardianCommunication.Service.WCF
{
	public class CustomOperationContext : IExtension<OperationContext>
	{
		
		//public CallSharedParameters CurrentCallSharedParameters { get; set; }
		public static CustomOperationContext Current
		{
			get
			{
				var context = OperationContext.Current.Extensions.Find<CustomOperationContext>();
				if (context == null)
				{
					context = new CustomOperationContext();
					OperationContext.Current.Extensions.Add(context);
				}
				return context;
			}
		}


		public void Attach(OperationContext owner) { }

		public void Detach(OperationContext owner) { }

	}
}
