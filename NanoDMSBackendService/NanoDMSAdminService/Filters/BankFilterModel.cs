using NanoDMSAdminService.Common;

namespace NanoDMSAdminService.Filters
{
    public class BankFilterModel : PaginationFilter
    {
        public string? Name { get; set; }
        public Guid? Country_Id { get; set; }
        public bool? Is_Active { get; set; }
    }

}
