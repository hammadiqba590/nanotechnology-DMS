using NanoDMSAdminService.Blocks;

namespace NanoDMSAdminService.Filters
{
    public class PspFilterModel
    {
        public string Name { get; set; } = null!;
        public string? Short_Name { get; set; }
        public string? Code { get; set; } 
        public string? Currency_Code { get; set; }
        public string? Registration_Number { get; set; }
        public ComplianceStatus? Compliance_Status { get; set; }
        public string? Contact_Email { get; set; }
        public string? Contact_Phone { get; set; }
        public string? Api_Key { get; set; }
        public IntegrationTypeStatus? Integration_Type { get; set; }
        public SettlementFrequencyStatus? Settlement_Frequency { get; set; }
        public bool Requires_Kyc { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
