using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{
	public static class EnumHelper
	{

		public static bool HasAllFlags<TEnum>(TEnum valueForVerify, TEnum flags) where TEnum : struct, IConvertible
		{
			var convertedSource = Convert.ToUInt64(valueForVerify);
			var convertedFlags = Convert.ToUInt64(flags);
			return (convertedSource & convertedFlags) == convertedFlags;
		}

		public static IEnumerable<Enum> GetTrueFlags(Enum input)
		{
			foreach (Enum value in Enum.GetValues(input.GetType()))
			{
				if (input.HasFlag(value))
				{
					yield return value;
				}
			}
		}

		public static IEnumerable<Enum> GetFalseFlags(Enum input)
		{
			foreach (Enum value in Enum.GetValues(input.GetType()))
			{
				if (!input.HasFlag(value))
				{
					yield return value;
				}
			}
		}

		public static object SetFlag(Enum mainValue, Enum valuesToSet)
		{
			var mainValueUnderlyingType = Enum.GetUnderlyingType(mainValue.GetType());
			var mainValueConverted = (long)Convert.ChangeType(mainValue, typeof(long));
			var valuesToSetConverted = (long)Convert.ChangeType(valuesToSet, typeof(long));
			mainValueConverted |= valuesToSetConverted;
			return Convert.ChangeType(mainValueConverted, mainValueUnderlyingType);
			
		}

		public static object RemoveFlag(Enum mainValue, Enum valuesToRemove)
		{
			var mainValueUnderlyingType = Enum.GetUnderlyingType(mainValue.GetType()); 
			var mainValueConverted = (long)Convert.ChangeType(mainValue, typeof(long));
			var valuesToRemoveConverted = (long)Convert.ChangeType(valuesToRemove, typeof(long));
			mainValueConverted &= ~valuesToRemoveConverted;
			return Convert.ChangeType(mainValueConverted, mainValueUnderlyingType);

		}

		//public static TEnum SetFlag<TEnum>(this TEnum value, TEnum flag, bool set) where TEnum : struct, IConvertible
		//{
		//	Type underlyingType = Enum.GetUnderlyingType(value.GetType());
		//	// note: AsInt mean: math integer vs enum (not the c# int type)
		//	dynamic valueAsInt = Convert.ChangeType(value, underlyingType);
		//	dynamic flagAsInt = Convert.ChangeType(flag, underlyingType);
		//	valueAsInt |= flagAsInt;
		//	return (TEnum)valueAsInt;
		//}

		//public static TEnum RemoveFlag<TEnum>(this TEnum value, TEnum flag, bool set) where TEnum : struct, IConvertible
		//{
		//	Type underlyingType = Enum.GetUnderlyingType(value.GetType());

		//	// note: AsInt mean: math integer vs enum (not the c# int type)
		//	dynamic valueAsInt = Convert.ChangeType(value, underlyingType);
		//	dynamic flagAsInt = Convert.ChangeType(flag, underlyingType);
		//	valueAsInt &= ~flagAsInt;
		//	return (TEnum)valueAsInt;
		//}
	}
}