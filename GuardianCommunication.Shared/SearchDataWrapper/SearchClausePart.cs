using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using GuardianCommunication.Shared.ExtensionsAndUtilities;

namespace GuardianCommunication.Shared.SearchDataWrapper
{
    public class SearchClausePart
    {
        public List<string> PrevWithClause { get; set; } = new List<string>();
        public List<string> PrevTempTableClause { get; set; } = new List<string>();

        public string PrevCommandClause
        {
            get
            {
                var sb = new StringBuilder();
                if (PrevTempTableClause.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine(string.Join(Environment.NewLine, PrevTempTableClause));
                }

                if (PrevWithClause.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($"WITH {string.Join($",{Environment.NewLine}", PrevWithClause)}");
                }

                return sb.ToString();
            }
        }

        public StringBuilder WhereClauseStringBuilder { get; set; } = new StringBuilder();
        public StringBuilder JoinClauseStringBuilder { get; set; } = new StringBuilder();
        public DynamicParameters Parameters { get; set; } = new DynamicParameters();
    }
}