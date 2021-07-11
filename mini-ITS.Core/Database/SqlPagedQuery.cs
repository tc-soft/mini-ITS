using System;
using System.Collections.Generic;

namespace mini_ITS.Core.Database
{
    public class SqlPagedQuery<T> : SqlPagedQueryBase
    {
        public List<SqlQueryCondition> Filter { get; set; }
        public string SortColumnName { get; set; }
        public string SortDirection { get; set; }
        internal int Offset
        {
            get
            {
                return (Page - 1) * ResultsPerPage;
            }
        }
    }
}