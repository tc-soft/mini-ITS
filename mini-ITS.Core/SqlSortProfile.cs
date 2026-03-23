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
        public static readonly Dictionary<string, Dictionary<string, string>> SpecialSortExpressionsByTable = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Enrollments"] = new(StringComparer.OrdinalIgnoreCase)
            {
                ["Priority"] =
                    "CASE [Priority] " +
                    " WHEN 0 THEN N'Normalny' " +
                    " WHEN 1 THEN N'Wysoki' " +
                    " WHEN 2 THEN N'Krytyczny' " +
                    " END COLLATE Polish_CI_AS",

                ["State"] =
                    "CASE [State] " +
                    " WHEN N'New'      THEN N'Nowy' " +
                    " WHEN N'Assigned' THEN N'W trakcie' " +
                    " WHEN N'Closed'   THEN N'Zamknięte' " +
                    " WHEN N'ReOpened' THEN N'Otwarte ponownie' " +
                    " END COLLATE Polish_CI_AS"
            }
        };
        public static readonly Dictionary<string, Dictionary<string, string>> CompositeSortTemplatesByTable = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Enrollments"] = new(StringComparer.OrdinalIgnoreCase)
            {
                ["NrDisplay"] = "[Year] {dir}, [Nr] {dir}"
            }
        };
    }
}