using System;

namespace GuardianCommunication.Shared.SearchDataWrapper
{
	public class SortInfo<T> where T : struct, IConvertible
	{
		public SortTypeEnum SortType { get; set; }

		public T SortItemEnum { get; set; }
	}
}
