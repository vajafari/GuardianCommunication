using System;
using System.Resources;
using System.Threading;

namespace GuardianCommunication.Shared.OperationResult
{
	public class ResultMessageProvider
	{
		static Type _resourceType;
		static Type ResourceType
		{
			get
			{
				if (_resourceType != null) return _resourceType;
				_resourceType = typeof(ResultEnumerationResource);
				return _resourceType;
			}
		}

		static ResourceManager _resourceManager;
		static ResourceManager ResourceManager
		{
			get
			{
				if (_resourceManager != null) return _resourceManager;
				_resourceManager = new ResourceManager(ResourceType);
				return _resourceManager;
			}
		}

		public static string GetMessageFromResource(string resourceId)
		{
			string value = ResourceManager.GetString(resourceId, Thread.CurrentThread.CurrentCulture);
			if (string.IsNullOrEmpty(value)) return resourceId;
			return value;
		}

		public static string GetMessageFromResource(OperationResultEnumeration status)
		{
			string value = ResourceManager.GetString(status.ToString(), Thread.CurrentThread.CurrentCulture);
			if (string.IsNullOrEmpty(value)) return status.ToString();
			return value;
		}


	}
}
