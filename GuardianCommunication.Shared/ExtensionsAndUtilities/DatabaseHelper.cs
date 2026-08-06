using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{
	public static class DatabaseHelper
	{

		public static string GetInClause(IEnumerable<long> list)
		{
			return string.Join(", ", list);
		}
		public static string GetInClause(IEnumerable<int> list)
		{
			return string.Join(", ", list);
		}
		public static string GetInClause(IEnumerable<short> list)
		{
			return string.Join(", ", list);
		}
		public static string GetInClause(IEnumerable<string> list)
		{
			return string.Join(", ", list.Select(row => $"N'{row}'"));
		}

		public static string GetLikeClause(long number)
		{
			return $"'%{number}%'";
		}
		public static string GetLikeClause(int number)
		{
			return $"'%{number}%'";
		}
		public static string GetLikeClause(short number)
		{
			return $"'%{number}%'";
		}
		public static string GetLikeClause(string str)
		{
			return $"N'%{str}%'";
		}


		public static string ParameterValueForSql(object vlaueForConvert)
		{
			return ParameterValueForSql(new SqlParameter { ParameterName = "TestName", Value = vlaueForConvert });
		}

		private static string ParameterValueForSql(SqlParameter sp)
		{
			string retval;

			if (sp.Value == null || sp.Value == DBNull.Value)
			{
				retval = "NULL";
			}
			else
			{
				switch (sp.SqlDbType)
				{

					case SqlDbType.UniqueIdentifier:
						retval = "'" + sp.Value.ToString().Replace("'", "''") + "'";
						break;
					case SqlDbType.Char:
					case SqlDbType.NChar:
					case SqlDbType.NText:
					case SqlDbType.NVarChar:
					case SqlDbType.Text:
					case SqlDbType.Time:
					case SqlDbType.VarChar:
					case SqlDbType.Xml:
						retval = "N'" + sp.Value.ToString().Replace("'", "''") + "'";
						break;
					case SqlDbType.Date:
					case SqlDbType.DateTime:
					case SqlDbType.DateTime2:
					case SqlDbType.DateTimeOffset:
						retval = "'" + ((DateTime)sp.Value).ToString(@"yyyy/MM/dd HH:mm:ss.fff", CultureInfo.GetCultureInfo("en-US")).Replace("'", "''") + "'";
						break;
					case SqlDbType.Bit:
						retval = ((bool)sp.Value) ? "1" : "0";
						break;
					case SqlDbType.Binary:
					case SqlDbType.VarBinary:
					case SqlDbType.Image:
						retval = "'" + sp.Value.ToString().Replace("'", "''") + "'";
						break;
					case SqlDbType.Decimal:
					case SqlDbType.Float:
					case SqlDbType.Real:
						retval = "'" + ((double)sp.Value).ToString(CultureInfo.InvariantCulture).Replace("'", "''") + "'";
						break;
					default:
						retval = sp.Value.ToString().Replace("'", "''");
						break;
				}
			}

			return retval;
		}



		//public static string ByteArrayToString(byte[] ba)
		//{
		//	StringBuilder hex = new StringBuilder(ba.Length * 2);
		//	foreach (byte b in ba)
		//		hex.AppendFormat("{0:x2}", b);
		//	return $"0x{hex}";
		//}


	}
}
