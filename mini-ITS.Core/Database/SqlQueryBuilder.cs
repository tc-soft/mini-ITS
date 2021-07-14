using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mini_ITS.Core.Database
{
    internal class SqlQueryBuilder<T>
    {
        private readonly string _tableName;
        private string[] _columnNames;
        private readonly List<SqlQueryCondition> _filters = new List<SqlQueryCondition>();
        private string _sortColumnName, _sortDirection;
        private int? _offset, _count;
        public SqlQueryBuilder()
        {
            Type type = typeof(T);
            _tableName = type.Name ?? throw new ArgumentNullException(type.Name);
        }
        public SqlQueryBuilder(SqlPagedQuery<T> sqlPagedQuery)
        {
            Type type = typeof(T);
            _tableName = type.Name ?? throw new ArgumentNullException(type.Name);
            _ = sqlPagedQuery.Filter is not null ? _filters = sqlPagedQuery.Filter : null;
            _filters.RemoveAll(item => item.Value is null);

            WithSort(sqlPagedQuery.SortColumnName, sqlPagedQuery.SortDirection);
            WithPaging(sqlPagedQuery.Offset, sqlPagedQuery.ResultsPerPage);
        }

        public string GetSelectQuery()
        {
            StringBuilder queryBuilder = new StringBuilder("SELECT ");

            if ((_columnNames != null) && (_columnNames.Length > 0))
            {
                queryBuilder.Append(String.Join(", ", _columnNames));
            }
            else
            {
                queryBuilder.Append("*");
            }

            queryBuilder.Append($" FROM {_tableName}");

            if (_filters.Count > 0)
            {
                var filters = new List<string>();

                foreach (var item in _filters)
                {
                    if (item.Value is not null && item.Value is string && !String.IsNullOrWhiteSpace((string)item.Value))
                    {
                        filters.Add($"{item.Name} {item.Operator} '{item.Value}'");
                    }

                    if (item.Value is not null && item.Value is int)
                    {
                        filters.Add($"{item.Name} {item.Operator} '{item.Value.ToString()}'");
                    }

                    if (item.Value is not null && item.Value is DateTime)
                    {
                        var dateTime = (DateTime)item.Value;
                        filters.Add($"{item.Name} {item.Operator} '{dateTime.ToString(Constants.DATE_FORMAT)}'");
                    }
                }

                queryBuilder.Append($" WHERE {String.Join(" AND ", filters)}");
            }

            if (!String.IsNullOrWhiteSpace(_sortColumnName) && !String.IsNullOrWhiteSpace(_sortDirection))
            {
                queryBuilder.AppendFormat(" ORDER BY {0} {1}", _sortColumnName, _sortDirection);
            }

            if (_offset.HasValue && _count.HasValue)
            {
                queryBuilder.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", _offset, _count);
            }

            return queryBuilder.ToString();
        }
        public string GetInsertQuery()
        {
            StringBuilder queryBuilder = new StringBuilder($"INSERT INTO {_tableName} (");

            var collection = typeof(T).GetProperties().Select(p => p.Name);

            foreach (var item in collection)
            {
                _ = item == collection.Last() ? queryBuilder.Append($"[{item}]) ") : queryBuilder.Append($"[{item}], ");
            }

            queryBuilder.Append("VALUES (");

            foreach (var item in collection)
            {
                _ = item == collection.Last() ? queryBuilder.Append($"@{item}) ") : queryBuilder.Append($"@{item}, ");
            }

            return queryBuilder.ToString();
        }
        public string GetUpdateQuery()
        {
            StringBuilder queryBuilder = new StringBuilder($"UPDATE {_tableName} SET ");

            var collection = typeof(T).GetProperties().Select(p => p.Name);

            foreach (var item in collection)
            {
                _ = item == collection.Last() ? queryBuilder.Append($"{item} = @{item} ") : queryBuilder.Append($"{item} = @{item}, ");
            }

            queryBuilder.Append("WHERE Id = @Id");

            return queryBuilder.ToString();
        }
        public string GetUpdateItemQuery(string item)
        {
            StringBuilder queryBuilder = new StringBuilder($"UPDATE {_tableName} SET ");

            if (item is not null)
            {
                queryBuilder.Append($"{item} = @{item} ");
            }

            queryBuilder.Append("WHERE Id = @Id");

            return queryBuilder.ToString();
        }
        public string GetDeleteQuery()
        {
            StringBuilder queryBuilder = new StringBuilder($"DELETE FROM {_tableName} WHERE Id = @Id");
            return queryBuilder.ToString();
        }
        public string GetCountQuery()
        {
            StringBuilder queryBuilder = new StringBuilder("SELECT COUNT(*) FROM ");
            queryBuilder.Append(_tableName);

            if (_filters.Count > 0)
            {
                var filters = new List<string>();

                foreach (var item in _filters)
                {
                    if (item.Value is not null && item.Value is string && !String.IsNullOrWhiteSpace((string)item.Value))
                    {
                        filters.Add($"{item.Name} {item.Operator} '{item.Value}'");
                    }

                    if (item.Value is not null && item.Value is int)
                    {
                        filters.Add($"{item.Name} {item.Operator} '{item.Value.ToString()}'");
                    }

                    if (item.Value is not null && item.Value is DateTime)
                    {
                        var dateTime = (DateTime)item.Value;
                        filters.Add($"{item.Name} {item.Operator} '{dateTime.ToString(Constants.DATE_FORMAT)}'");
                    }
                }

                queryBuilder.Append($" WHERE {String.Join(" AND ", filters)}");
            }

            return queryBuilder.ToString();
        }
        public SqlQueryBuilder<T> WithColumns(params string[] columnNames)
        {
            _columnNames = columnNames;

            return this;
        }
        public SqlQueryBuilder<T> WithFilter(List<SqlQueryCondition> sqlQueryConditionList)
        {
            foreach (var sqlQueryCondition in sqlQueryConditionList)
            {
                if (sqlQueryCondition is not null && sqlQueryCondition.Value is not null)
                {
                    _filters.Add(sqlQueryCondition);
                }
            }

            return this;
        }
        public SqlQueryBuilder<T> WithSort(string columnName, string direction)
        {
            _sortColumnName = columnName;
            _sortDirection = direction;

            return this;
        }
        public SqlQueryBuilder<T> WithPaging(int offset, int count)
        {
            if (String.IsNullOrWhiteSpace(_sortColumnName) || String.IsNullOrWhiteSpace(_sortDirection))
            {
                throw new InvalidOperationException("Paging requires Sort");
            }

            _offset = offset;
            _count = count;

            return this;
        }   
    }
}