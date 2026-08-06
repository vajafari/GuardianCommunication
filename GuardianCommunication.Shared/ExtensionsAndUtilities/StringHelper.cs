using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{
	public static class StringHelper
	{
		#region string Extenstions

		public static int IndexOfEx(this string str, string value, int startIndex = 0, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
		{
			return str.IndexOf(value, startIndex, stringComparison);
		}

		private static string RemoveArabicChars(string inputString)
		{
			return inputString.Replace("ي", "ی").Replace("ك", "ک").Replace("ﻙ", "ک");
		}

		public static string ToPersianCharacter(this string inputString)
		{
			if (string.IsNullOrEmpty(inputString)) return inputString;

			var sb = new StringBuilder();
			foreach (var character in inputString)
			{

				switch (character)
				{
					case 'ﻙ':
					case 'ك':
						sb.Append("ک");
						break;
					case 'ي':
						//case 'ئ':
						sb.Append("ی");
						break;
					default:
						sb.Append(character);
						break;

				}
			}

			return sb.ToString();
		}

		public static string ToNotNullPersianCharacter(this string inputString)
		{
		    return string.IsNullOrEmpty(inputString) ? string.Empty : ToPersianCharacter(inputString);
		}

		public static string ToStandardPersianStringWithPersianNumber(this string inputString)
		{
			if (string.IsNullOrEmpty(inputString)) return inputString;

			var sb = new StringBuilder();
			foreach (var character in inputString)
			{
				switch (character)
				{
					//case 'آ':
					//	sb.Append("ا");
					//	break;
					case 'ﻙ':
						sb.Append("ک");
						break;
					case 'ي':
						//case 'ئ':
						sb.Append("ی");
						break;
					case '۰':
						sb.Append("0");
						break;
					case '۱':
						sb.Append("1");
						break;
					case '۲':
						sb.Append("2");
						break;
					case '۳':
						sb.Append("3");
						break;
					case '۴':
						sb.Append("4");
						break;
					case '۵':
						sb.Append("5");
						break;
					case '۶':
						sb.Append("6");
						break;
					case '۷':
						sb.Append("7");
						break;
					case '۸':
						sb.Append("8");
						break;
					case '۹':
						sb.Append("9");
						break;
					default:
						sb.Append(character);
						break;

				}
			}
			return sb.ToString();
		}

		public static string ToNotNullStandardPersianStringWithPersianNumber(this string inputString)
		{
			if (inputString == null) return string.Empty;

			var sb = new StringBuilder();
			foreach (var character in inputString)
			{
				switch (character)
				{
					case 'آ':
						sb.Append("ا");
						break;
					case 'ﻙ':
						sb.Append("ک");
						break;
					case 'ي':
					case 'ئ':
						sb.Append("ی");
						break;
					case '۰':
						sb.Append("0");
						break;
					case '۱':
						sb.Append("1");
						break;
					case '۲':
						sb.Append("2");
						break;
					case '۳':
						sb.Append("3");
						break;
					case '۴':
						sb.Append("4");
						break;
					case '۵':
						sb.Append("5");
						break;
					case '۶':
						sb.Append("6");
						break;
					case '۷':
						sb.Append("7");
						break;
					case '۸':
						sb.Append("8");
						break;
					case '۹':
						sb.Append("9");
						break;
					default:
						sb.Append(character);
						break;

				}
			}
			return sb.ToString();
		}

		public static string ToNotNullString(this string inputString)
		{
		    return inputString ?? string.Empty;
		}

		public static bool IsNullOrEmpty(this string inputString)
		{
			return string.IsNullOrEmpty(inputString);
		}

		public static bool IsNotNullOrEmpty(this string inputString)
		{
			return !string.IsNullOrEmpty(inputString);
		}

		public static string FormatInvariantCulture(this string inputParameter, params object[] args)
		{
			string result = null;
			if (inputParameter != null)
			{
				result = string.Format(CultureInfo.InvariantCulture, inputParameter, args);
			}
			return result;
		}

		public static string FormatCurrentCulture(this string inputParameter, params object[] args)
		{
			string result = null;
			if (inputParameter != null)
			{
				result = string.Format(CultureInfo.CurrentCulture, inputParameter, args);
			}
			return result;
		}

		public static string RemoveRedundantLeftZeroes(this string inputString)
		{
			var hasNegativeSign = Regex.IsMatch(inputString, @"^-+", RegexOptions.IgnoreCase);
			var returnValue = Regex.Replace(inputString, @"^[0۰]+\s*[:\.][0۰]+$|^[-0۰]+(?![.:])", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			if (hasNegativeSign)
				returnValue = $"-{returnValue}";
			return returnValue;
		}

		public static string ToStringInvariantCulture(this string inputParameter, params object[] args)
		{
			string result = null;
			if (inputParameter != null)
			{
				result = string.Format(CultureInfo.InvariantCulture, inputParameter, args);
			}
			return result;
		}

		public static string ToStringCurrentCulture(this string inputParameter, params object[] args)
		{
			string result = null;
			if (inputParameter != null)
			{
				result = string.Format(CultureInfo.CurrentCulture, inputParameter, args);
			}
			return result;
		}

		#endregion

		#region Data Convert
		
		public static bool CanConvertToBoolean(this string inputParameter)
		{
			return bool.TryParse(inputParameter, out _);
		}

		public static bool CanConvertToInt8(this string inputParameter)
		{
			return byte.TryParse(inputParameter, out _);
		}

		public static bool CanConvertToInt16(this string inputParameter)
		{
			return short.TryParse(inputParameter, out _);
		}

		public static bool CanConvertToInt32(this string inputParameter)
		{
			return int.TryParse(inputParameter, out _);
		}

		public static bool CanConvertToUInt32(this string inputParameter)
		{
			return uint.TryParse(inputParameter, out _);
		}

		public static bool CanConvertToInt64(this string inputParameter)
		{
			return long.TryParse(inputParameter, out _);
		}

		public static bool ToBoolean(this string inputParameter)
		{
			return bool.Parse(inputParameter);
		}

		public static byte ToInt8(this string inputParameter)
		{
			return byte.Parse(inputParameter);
		}

		public static short ToInt16(this string inputParameter)
		{
			return short.Parse(inputParameter);
		}

        public static uint ToUInt32(this string inputParameter)
        {
            uint.TryParse(inputParameter, out var res);
            return res;
        }

        public static int ToInt32(this string inputParameter)
		{
			int.TryParse(inputParameter,out var res);
		    return res;
		}

		public static long ToInt64(this string inputParameter)
		{
			return long.Parse(inputParameter);
		}

		public static double ToDouble(this string inputParameter)
		{
			return double.Parse(inputParameter);
		}



		public static bool TryConvertToBoolean(this string inputParameter, bool defaultValue = false)
		{
			return bool.TryParse(inputParameter, out var result) ? result : defaultValue;
		}

		public static byte TryConvertToInt8(this string inputParameter, byte defaultValue = 0)
		{
			return byte.TryParse(inputParameter, out var result) ? result : defaultValue;
		}

		public static short TryConvertToInt16(this string inputParameter, short defaultValue = 0)
		{
			return short.TryParse(inputParameter, out var result) ? result : defaultValue;
		}

		public static int TryConvertToInt32(this string inputParameter, int defaultValue = 0)
		{
			return int.TryParse(inputParameter, out var result) ? result : defaultValue;
		}

		public static long TryConvertToInt64(this string inputParameter, long defaultValue = 0)
		{
			return long.TryParse(inputParameter, out long result) ? result : defaultValue;
		}

		public static double TryConvertToDouble(this string inputParameter, double defaultValue = 0)
		{
			return double.TryParse(inputParameter, out var result) ? result : defaultValue;
		}



		#endregion
	}
}
