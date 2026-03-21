using System;
using System.Collections.Generic;

namespace mini_ITS.Core
{
    public static class SqlSortProfile
    {
        public static readonly Dictionary<string, HashSet<string>> NullsLastColumnsByTable = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Enrollments"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "DateEndEnrollment"
            }
        };
    }
}