using System;
using System.Collections.Generic;

namespace mini_ITS.Core.Database
{
    public class SqlPagedResult<T> : SqlPagedResultBase
    {
        public IEnumerable<T> Results { get; set; }

        public static SqlPagedResult<T> From(SqlPagedResultBase sqlPagedResultBase, IEnumerable<T> results)
            => new SqlPagedResult<T>
            {
                Results = results,
                Page = sqlPagedResultBase.Page,
                ResultsPerPage = sqlPagedResultBase.ResultsPerPage,
                TotalResults = sqlPagedResultBase.TotalResults,
                TotalPages = sqlPagedResultBase.TotalPages
            };

        public static SqlPagedResult<T> Create(IEnumerable<T> results, 
            SqlPagedQueryBase sqlPagedQueryBase, int totalResults)
            => Create(results, sqlPagedQueryBase.Page, sqlPagedQueryBase.ResultsPerPage, totalResults);

        public static SqlPagedResult<T> Create(IEnumerable<T> results, 
            int page, int resultsPerPage, int totalResults)
            {
                return new SqlPagedResult<T>
                {
                    Results = results,
                    Page = page,
                    ResultsPerPage = resultsPerPage,
                    TotalResults = totalResults,
                    TotalPages = (int)Math.Ceiling((double)totalResults/resultsPerPage)
                };
            }
    }
}