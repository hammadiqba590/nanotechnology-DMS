namespace NanoDMSAdminService.Filters
{
    public class CurrencyFilterModel
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public Guid? Country_Id { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
