using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using GuardianCommunication.Shared.SearchDataWrapper;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{

    public static class DatabaseHelper
    {


        public const int GeneralMaxCountForUseInString = 100;
        public const int GeneralMaxCountForUseParsString = 300;
        public const int GeneralMaxCountForUseOpenJson = 2000;

        public static string GetInClause(IEnumerable<long> list)
        {
            return list.JoinTogether();
        }

        public static string GetInClause(IEnumerable<int> list)
        {
            return list.JoinTogether();
        }

        public static string GetInClause(IEnumerable<short> list)
        {
            return list.JoinTogether();
        }

        public static string GetInClause(IEnumerable<string> list)
        {
            return list.Select(row => $"N'{row}'").JoinTogether();
        }


        public static string GetLikeClause(long number)
        {
            return $"%{number}%";
        }

        public static string GetLikeClause(int number)
        {
            return $"%{number}%";
        }

        public static string GetLikeClause(short number)
        {
            return $"%{number}%";
        }

        public static string GetLikeClause(string str)
        {
            return $"%{str}%";
        }


        public static string ParameterValueForSql(object valueForConvert)
        {
            if (valueForConvert is Enum)
            {
                var underlyingType = Enum.GetUnderlyingType(valueForConvert.GetType());
                var value = Convert.ChangeType(valueForConvert, underlyingType); // x will be int
                return ParameterValueForSql(new SqlParameter { ParameterName = "TestName", Value = value });
            }

            return ParameterValueForSql(new SqlParameter { ParameterName = "TestName", Value = valueForConvert });

        }

        private static string ParameterValueForSql(SqlParameter sp)
        {
            string retrieval;

            if (sp.Value == null || sp.Value == DBNull.Value)
            {
                retrieval = "NULL";
            }
            else
            {
                switch (sp.SqlDbType)
                {

                    case SqlDbType.UniqueIdentifier:
                        retrieval = "'" + sp.Value.ToString().Replace("'", "''") + "'";
                        break;
                    case SqlDbType.Char:
                    case SqlDbType.NChar:
                    case SqlDbType.NText:
                    case SqlDbType.NVarChar:
                    case SqlDbType.Text:
                    case SqlDbType.Time:
                    case SqlDbType.VarChar:
                    case SqlDbType.Xml:
                        retrieval = "N'" + sp.Value.ToString().Replace("'", "''") + "'";
                        break;
                    case SqlDbType.Date:
                    case SqlDbType.DateTime:
                    case SqlDbType.DateTime2:
                    case SqlDbType.DateTimeOffset:
                        retrieval = "'" + ((DateTime)sp.Value)
                            .ToString(@"yyyy/MM/dd HH:mm:ss.fff", new CultureInfo("en-US")).Replace("'", "''") + "'";
                        break;
                    case SqlDbType.Bit:
                        retrieval = (bool)sp.Value ? "1" : "0";
                        break;
                    case SqlDbType.Binary:
                    case SqlDbType.VarBinary:
                    case SqlDbType.Image:
                        retrieval = "'" + sp.Value.ToString().Replace("'", "''") + "'";
                        break;
                    case SqlDbType.Decimal:
                    case SqlDbType.Float:
                    case SqlDbType.Real:
                        retrieval = "'" + ((double)sp.Value).ToString(CultureInfo.InvariantCulture).Replace("'", "''") +
                                    "'";
                        break;
                    default:
                        retrieval = sp.Value.ToString().Replace("'", "''");
                        break;
                }
            }

            return retrieval;
        }


        public static void ApplyInClause<T>(List<T> items
            , SearchClausePart searchClausePart
            , string fieldName
            , string parameterName
            , string dataTypeString
            , bool isNumber
        )
        {
            if (items.IsCollectionNotNullOrEmpty())
            {
                var itemCount = items.Count;
                if (itemCount == 1)
                {
                    searchClausePart.Parameters.Add($"@{parameterName}", items);
                    searchClausePart.WhereClauseStringBuilder.AppendLine($" AND {fieldName} = @{parameterName}");
                }
                else if (itemCount <= GeneralMaxCountForUseInString)
                {
                    searchClausePart.Parameters.Add($"@{parameterName}", items);
                    searchClausePart.WhereClauseStringBuilder.AppendLine($" AND {fieldName} IN @{parameterName}");
                }
                else if (itemCount <= GeneralMaxCountForUseParsString)
                {
                    searchClausePart.Parameters.Add($"@{parameterName}", items.JoinTogether(","));
                    searchClausePart.PrevWithClause.Add(
                        $@"{parameterName}List AS (
    SELECT CAST([value] AS {dataTypeString}) AS EntityId
    FROM STRING_SPLIT(@{parameterName}, ',')
)"
                    );
                    searchClausePart.JoinClauseStringBuilder.AppendLine(
                        $"INNER JOIN {parameterName}List AS eil{parameterName} ON eil{parameterName}.EntityId = {fieldName}");
                }
                else if (itemCount <= GeneralMaxCountForUseOpenJson)
                {
                    searchClausePart.Parameters.Add(parameterName,
                        isNumber
                            ? $"[{items.Select(i => $"{i}").JoinTogether(",")}]"
                            : $"[{items.Select(i => $"\"{i}\"").JoinTogether(",")}]");
                    searchClausePart.JoinClauseStringBuilder.AppendLine(
                        $"INNER JOIN OPENJSON(@{parameterName}) WITH (EntityId {dataTypeString} '$') eil{parameterName} ON eil{parameterName}.[EntityId] = {fieldName}");
                }
                else
                {
                    searchClausePart.Parameters.Add(parameterName,
                        isNumber
                            ? $"[{items.Select(i => $"{i}").JoinTogether(",")}]"
                            : $"[{items.Select(i => $"\"{i}\"").JoinTogether(",")}]");
                    searchClausePart.PrevTempTableClause.Add(
                        $@"CREATE TABLE #{parameterName}Ids (EntityId {dataTypeString} NOT NULL);
INSERT INTO #{parameterName}Ids (EntityId)
SELECT CAST([value] AS {dataTypeString}) AS [EntityId]
FROM OPENJSON(@{parameterName});
CREATE CLUSTERED INDEX IX_{parameterName}TempIds ON #{parameterName}Ids(EntityId);"
                    );
                    searchClausePart.JoinClauseStringBuilder.AppendLine(
                        $"INNER JOIN #{parameterName}Ids eil{parameterName} ON eil{parameterName}.[EntityId] = {fieldName}");
                }
            }
        }


    }
}