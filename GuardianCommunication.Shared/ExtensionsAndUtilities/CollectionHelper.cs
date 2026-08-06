using System.Collections.Generic;
using System.Linq;
using GuardianCommunication.Shared.OperationResult;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{
	public static class CollectionHelper
	{
       
        public static bool IsCollectionNullOrEmpty<T>(this IEnumerable<T> collection)
		{
			return collection == null || !collection.Any();
		}

		public static bool IsCollectionNotNullOrEmpty<T>(this IEnumerable<T> collection)
		{
			return collection != null && collection.Any();
		}

		public static string JoinWithComma<T>(this IEnumerable<T> collection)
		{
			if (collection.IsCollectionNullOrEmpty())
			{
				return string.Empty;
			}
			return string.Join(",", collection);
		}


		/// <summary>
		/// این متد برای بررسی دسترسی کاربران استفاده می شود
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sourceList"> فهرستی که کاربر درخواست انجام عملیات بر روی آنها را داده است</param>
		/// <param name="validList">فهرستی که کاربر مجاز به دسترسی به آن است- این فهرست می تواند کل درستری های کاربر باشد و یا زیرمجوعه ای از فهرست کاربر، که مجاز به دسترسی  به انها است</param>
		/// <param name="exceptionToThrow"></param>
		public static void CheckValidityOfCurrentListWithValidList<T>(IEnumerable<T> sourceList, IEnumerable<T> validList, OperationResultEnumeration exceptionToThrow)
		{
			if (sourceList != null && validList != null)
			{
				List<T> distinctSourceList = sourceList.Distinct().ToList();
				List<T> distinctValidList = validList.Distinct().ToList();
				if (distinctValidList.Intersect(distinctSourceList).Count() != distinctSourceList.Count)
				{
					string additionalInfo = string.Join(",", distinctSourceList.Where(row => !distinctValidList.Contains(row)));
					throw new OperationCannotBeDoneException(string.Format(CommunicationSharedResource.ObjectSerial, additionalInfo), exceptionToThrow);
				}
			}
			else if (sourceList != null && sourceList.Any())
			{
				string additionalInfo = string.Join(",", sourceList);
				throw new OperationCannotBeDoneException(string.Format(CommunicationSharedResource.ObjectSerial, additionalInfo), exceptionToThrow);
			}
		}

	}
}

