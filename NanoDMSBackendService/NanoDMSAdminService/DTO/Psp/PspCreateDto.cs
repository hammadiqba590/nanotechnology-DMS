using NanoDMSAdminService.Blocks;
using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.Psp
{
    public class PspCreateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;
        [MaxLength(50)]
        public string? Short_Name { get; set; }
        [MaxLength(20)]
        public string? Code { get; set; }
        //public string Psp_Category_Name { get; set; } = null!;
        public Guid Psp_Category_Id { get; set; }
       // public string? Country_Name { get; set; }
        public Guid? Country_Id { get; set; }
        [StringLength(3)]
        public string? Currency_Code { get; set; }
        [MaxLength(10)]
        public string? Currency_Symbol { get; set; }
        [MaxLength(50)]
        public string? Registration_Number { get; set; }
        [MaxLength(255)]
        public string? Reg_Doc_Url { get; set; }
        public ComplianceStatus? Compliance_Status { get; set; }
        [MaxLength(255)]
        public string? Website { get; set; }
        [MaxLength(100)]
        public string? Contact_Email { get; set; }
        [MaxLength(20)]
        public string? Contact_Phone { get; set; }
        [MaxLength(255)]
        public string? Api_Endpoint { get; set; }
        [MaxLength(255)]
        public string? Sandbox_Endpoint { get; set; }
        [MaxLength(255)]
        public string? Webhook_Url { get; set; }
        [MaxLength(255)]
        public string? Api_Key { get; set; }
        [MaxLength(255)]
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
        public Guid Business_Id { get; set; }
        public Guid Business_Location_Id { get; set; }
    }
}
