namespace NanoDMSAdminService.Filters
{
    public class CountryFilterModel
    {
        public string? Name { get; set; }
        public string? Iso2 { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
