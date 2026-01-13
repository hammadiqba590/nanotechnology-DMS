namespace NanoDMSAdminService.Filters
{
    public class PspCategoryFilterModel
    {
       public string Name { get; set; } = null!;
       public int PageNumber { get; set; } = 1;
       public int PageSize { get; set; } = 10;
    }
}
