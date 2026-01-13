using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.Psp
{
    public class PspDto : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Short_Name { get; set; }
        public string? Code { get; set; }
        public string Psp_Category_Name { get; set; } = null!;
        public Guid Psp_Category_Id { get; set; }
        public string? Country_Name { get; set; }
        public Guid? Country_Id { get; set; }
        public string? Currency_Code { get; set; } 
        public string? Currency_Symbol { get; set; } 
        public string? Registration_Number { get; set; } 
        public string? Reg_Doc_Url { get; set; } 
        public ComplianceStatus? Compliance_Status { get; set; } 
        public string? Website { get; set; }
        public string? Contact_Email { get; set; }
        public string? Contact_Phone { get; set; }
        public string? Api_Endpoint { get; set; } 
        public string? Sandbox_Endpoint { get; set; }
        public string? Webhook_Url { get; set; } 
        public string? Api_Key { get; set; } 
        public string? Documentation_Url { get; set; }
        public IntegrationTypeStatus? Integration_Type { get; set; }
        public string? Supported_Payment_Methods { get; set; } 
        public string? Supported_Currencies { get; set; }    
        public SettlementFrequencyStatus? Settlement_Frequency { get; set; } 
        public decimal? Transaction_Limit { get; set; } 
        public decimal? Daily_Volume_Limit { get; set; } 
        public int? Risk_Score { get; set; } 
        public bool Requires_Kyc { get; set; }
        public Guid? Onboarded_By { get; set; } 
        public DateTime Onboarded_At { get; set; }
    }

}
