using System;
using System.Linq;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{
	public static class NumericHelper
	{

		#region Max

		public static byte Max(byte firstParam, byte secondParam)
		{
			return Math.Max(firstParam, secondParam);
		}

		public static byte Max(byte firstParam, byte secondParam, byte thirdParam)
		{
			return Math.Max(firstParam, Math.Max(secondParam, thirdParam));
		}

		public static byte Max(byte firstParam, byte secondParam, byte thirdParam, byte fourthParam)
		{
			return Math.Max(fourthParam, Math.Max(firstParam, Math.Max(secondParam, thirdParam)));
		}

		public static byte Max(params byte[] inputParameters)
		{
			return inputParameters.Max();
		}




		public static short Max(short firstParam, short secondParam)
		{
			return Math.Max(firstParam, secondParam);
		}

		public static short Max(short firstParam, short secondParam, short thirdParam)
		{
			return Math.Max(firstParam, Math.Max(secondParam, thirdParam));
		}

		public static short Max(short firstParam, short secondParam, short thirdParam, short fourthParam)
		{
			return Math.Max(fourthParam, Math.Max(firstParam, Math.Max(secondParam, thirdParam)));
		}

		public static short Max(params short[] inputParameters)
		{
			return inputParameters.Max();
		}



		public static int Max(int firstParam, int secondParam)
		{
			return Math.Max(firstParam, secondParam);
		}

		public static int Max(int firstParam, int secondParam, int thirdParam)
		{
			return Math.Max(firstParam, Math.Max(secondParam, thirdParam));
		}

		public static int Max(int fourthParam, int firstParam, int secondParam, int thirdParam)
		{
			return Math.Max(fourthParam, Math.Max(firstParam, Math.Max(secondParam, thirdParam)));
		}

		public static int Max(params int[] inputParameters)
		{
			return inputParameters.Max();
		}



		public static long Max(long firstParam, long secondParam)
		{
			return Math.Max(firstParam, secondParam);
		}

		public static long Max(long firstParam, long secondParam, long thirdParam)
		{
			return Math.Max(firstParam, Math.Max(secondParam, thirdParam));
		}

		public static long Max(long fourthParam, long firstParam, long secondParam, long thirdParam)
		{
			return Math.Max(fourthParam, Math.Max(firstParam, Math.Max(secondParam, thirdParam)));
		}

		public static long Max(params long[] inputParameters)
		{
			return inputParameters.Max();
		}

		#endregion


		#region Min


		public static byte Min(byte firstParam, byte secondParam)
		{
			return Math.Min(firstParam, secondParam);
		}

		public static byte Min(byte firstParam, byte secondParam, byte thirdParam)
		{
			return Math.Min(firstParam, Math.Min(secondParam, thirdParam));
		}

		public static byte Min(byte firstParam, byte secondParam, byte thirdParam, byte fourthParam)
		{
			return Math.Min(fourthParam, Math.Min(firstParam, Math.Min(secondParam, thirdParam)));
		}

		public static byte Min(params byte[] inputParameters)
		{
			return inputParameters.Min();
		}


		public static short Min(short firstParam, short secondParam)
		{
			return Math.Min(firstParam, secondParam);
		}

		public static short Min(short firstParam, short secondParam, short thirdParam)
		{
			return Math.Min(firstParam, Math.Min(secondParam, thirdParam));
		}

		public static short Min(short firstParam, short secondParam, short thirdParam, short fourthParam)
		{
			return Math.Min(fourthParam, Math.Min(firstParam, Math.Min(secondParam, thirdParam)));
		}

		public static short Min(params short[] inputParameters)
		{
			return inputParameters.Min();
		}


		public static int Min(int firstParam, int secondParam)
		{
			return Math.Min(firstParam, secondParam);
		}

		public static int Min(int firstParam, int secondParam, int thirdParam)
		{
			return Math.Min(firstParam, Math.Min(secondParam, thirdParam));
		}

		public static int Min(int firstParam, int secondParam, int thirdParam, int fourthParam)
		{
			return Math.Min(fourthParam, Math.Min(firstParam, Math.Min(secondParam, thirdParam)));
		}

		public static int Min(params int[] inputParameters)
		{
			return inputParameters.Min();
		}



		public static long Min(long firstParam, long secondParam)
		{
			return Math.Min(firstParam, secondParam);
		}

		public static long Min(long firstParam, long secondParam, long thirdParam)
		{
			return Math.Min(firstParam, Math.Min(secondParam, thirdParam));
		}

		public static long Min(long firstParam, long secondParam, long thirdParam, long fourthParam)
		{
			return Math.Min(fourthParam, Math.Min(firstParam, Math.Min(secondParam, thirdParam)));
		}

		public static long Min(params long[] inputParameters)
		{
			return inputParameters.Min();
		}

		#endregion


		#region range

		public static bool IsInRange(this long valueForCheck, long minValue, long maxValue)
		{
			return valueForCheck >= minValue && valueForCheck <= maxValue;
		}

		public static bool IsInRange(this int valueForCheck, int minValue, int maxValue)
		{
			return valueForCheck >= minValue && valueForCheck <= maxValue;
		}

		public static bool IsInRange(this short valueForCheck, short minValue, short maxValue)
		{
			return valueForCheck >= minValue && valueForCheck <= maxValue;
		}

		public static bool IsInRange(this byte valueForCheck, byte minValue, byte maxValue)
		{
			return valueForCheck >= minValue && valueForCheck <= maxValue;
		}

		#endregion




		public static int SetOrUnsetBit(int number, byte bitNumber, bool isSet)
		{
			if (isSet)
			{
				number |= 1 << bitNumber;
			}
			else
			{
				number &= ~(1 << bitNumber);

			}
			return number;
		}


	}
}
