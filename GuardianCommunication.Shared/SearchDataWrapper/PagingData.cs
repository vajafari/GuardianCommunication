using System;
using System.Collections.Generic;
using System.Linq;
using GuardianCommunication.Shared.ExtensionsAndUtilities;

namespace GuardianCommunication.Shared.SearchDataWrapper
{
    public class PagingData<TFilter, TSortEnum>
        where TFilter : class
        where TSortEnum : struct,
        IConvertible
    {

        public PagingData() : this(null, null, null) { }

        public PagingData(TFilter filter) : this(filter, null, null) { }

        public PagingData(TFilter filter, List<SortInfo<TSortEnum>> sortItems) : this(filter, sortItems, null) { }

        public PagingData(TFilter filter, List<SortInfo<TSortEnum>> sortItems, CurrentPageInfo currentPage)
        {
            Filter = filter;
            SortItems = sortItems;
            CurrentPage = currentPage;
        }

        public CurrentPageInfo CurrentPage { get; set; }

        public List<SortInfo<TSortEnum>> SortItems { get; set; }

        public TFilter Filter { get; set; }

        public SearchTypeEnumeration GetSearchType()
        {
            var result = SearchTypeEnumeration.SimpleSearch;
            if (CurrentPage == null) return result;
            if (SortItems.IsCollectionNotNullOrEmpty())
                result = SearchTypeEnumeration.SearchWithPaging;
            return result;
        }

        public string GetNormalSortString(Dictionary<TSortEnum, string> map, string tableAccPrefix = "")
        {
            var result = string.Empty;
            if (!SortItems.IsCollectionNotNullOrEmpty()) return result;
            tableAccPrefix = tableAccPrefix.IsNotNullOrEmpty() ? tableAccPrefix + "." : "";
            var sortItemStringsList = SortItems.Select(item => $"{tableAccPrefix}{map[item.SortItemEnum]} {item.SortType}").ToList();
            result = " ORDER BY " + sortItemStringsList.JoinWithComma();
            return result;
        }

        public string GetRowNumberClause(string rowNumberColumnName)
        {
            return GetRowNumberClause("", rowNumberColumnName);
        }

        public string GetRowNumberClause(string prefix, string rowNumberColumnName)
        {
            var result = string.Empty;
            if (CurrentPage == null || CurrentPage.PageNumber <= 0 || CurrentPage.ItemPerPage <= 0) return result;
            var columnName = (prefix.IsNotNullOrEmpty() ? prefix + "." : "") + rowNumberColumnName;
            return $" WHERE {columnName} BETWEEN {(CurrentPage.PageNumber - 1) * CurrentPage.ItemPerPage + 1} AND {CurrentPage.PageNumber * CurrentPage.ItemPerPage}";
        }

        //public string GetSqliteClauseWithLimit()
        //{
	       // var result = string.Empty;
	       // if (CurrentPage == null || CurrentPage.PageNumber <= 0 || CurrentPage.ItemPerPage <= 0) return result;
	       // return $" LIMIT {(CurrentPage.PageNumber - 1) * CurrentPage.ItemPerPage}, {CurrentPage.PageNumber * CurrentPage.ItemPerPage}";
        //}
    }
}
