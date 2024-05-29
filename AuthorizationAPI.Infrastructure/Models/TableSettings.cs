namespace AuthorizationAPI.Infrastructure.Models;

public class TableSettings
{
    public TableDensity Density { get; set; } = TableDensity.Comfortable;
    public TablePagination Pagination { get; set; } = new()
    {
        Page = 1,
        RowsPerPage = 50
    };

    public enum TableDensity
    {
        Comfortable,
        Compact,
        Spacious
    }

    public class TablePagination
    {
        public int Page { get; set; }
        public int RowsPerPage { get; set; }
    }
}