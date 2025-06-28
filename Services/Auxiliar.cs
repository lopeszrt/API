using System.Data;

namespace API.Services
{
    public static class Auxiliar
    {
        public static Dictionary<string, object>? FirstRowToDictionary(DataTable table)
        {
            return table.Rows.Count > 0
                ? table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => table.Rows[0][col])
                : null;
        }

        public static List<Dictionary<string, object>> DataTableToList(DataTable table)
        {
            return table.AsEnumerable()
                .Select(row => table.Columns.Cast<DataColumn>()
                    .ToDictionary(col => col.ColumnName, col => row[col]))
                .ToList();
        }

    }
}
