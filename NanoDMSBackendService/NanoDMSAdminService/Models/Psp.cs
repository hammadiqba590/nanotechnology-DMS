using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class Psp : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!; //'Full name of PSP, e.g., Keenu, Alfalah',

        [MaxLength(50)]
        public string? Short_Name { get; set; } //'Short display name',

        [MaxLength(20)]
        public string? Code { get; set; } // 'Unique PSP internal code',

        public Guid Psp_Category_Id { get; set; }
        [ForeignKey(nameof(Psp_Category_Id))]
        public PspCategory PspCategory { get; set; } = null!;

        public Guid? Country_Id { get; set; }
        [ForeignKey(nameof(Country_Id))]
        public Country? Country { get; set; }

        [StringLength(3)]
        public string? Currency_Code { get; set; } // 'Default currency (PKR, USD)',

        [MaxLength(10)]
        public string? Currency_Symbol { get; set; } // 'Currency symbol',

        [MaxLength(50)]
        public string? Registration_Number { get; set; } // 'Company registration/license number',

        [MaxLength(255)]
        public string? Reg_Doc_Url { get; set; } // 'Uploaded legal doc URL',

        public ComplianceStatus? Compliance_Status { get; set; } // 'pending','approved','rejected'  //'PSP approval status',

        [MaxLength(255)]
        public string? Website { get; set; }

        [MaxLength(100)]
        public string? Contact_Email { get; set; }

        [MaxLength(20)]
        public string? Contact_Phone { get; set; }

        [MaxLength(255)]
        public string? Api_Endpoint { get; set; } // 'Base API URL',

        [MaxLength(255)]
        public string? Sandbox_Endpoint { get; set; } // 'Testing environment URL',

        [MaxLength(255)]
        public string? Webhook_Url { get; set; } // 'Callback for payment updates',

        [MaxLength(255)]
        public string? Api_Key { get; set; } // 'Encrypted key for API',

        [MaxLength(255)]
        public string? Documentation_Url { get; set; } //'Integration docs link',
        public IntegrationTypeStatus? Integration_Type { get; set; } // 'direct','aggregator','gateway'

        public string? Supported_Payment_Methods { get; set; } // JSON // "card","wallet","bank_transfer"
        public string? Supported_Currencies { get; set; }     // JSON // "PKR","USD","AED"
        public SettlementFrequencyStatus? Settlement_Frequency { get; set; } // 'daily','weekly','monthly'
        public decimal? Transaction_Limit { get; set; } // 'Max single transaction',
        public decimal? Daily_Volume_Limit { get; set; } // 'Max allowed daily volume',

        public int? Risk_Score { get; set; } // 'Internal risk rating',
        public bool Requires_Kyc { get; set; } = false; // 'Does PSP require KYC for users',

        public Guid? Onboarded_By { get; set; } // 'Admin/User who onboarded PSP',
        public DateTime Onboarded_At { get; set; } = DateTime.UtcNow;

        public ICollection<PspDocument>? PspDocuments { get; set; }
        public ICollection<PspPaymentMethod>? PspPaymentMethods { get; set; }
        public ICollection<PspCurrency>? PspCurrencies { get; set; }
    }
}
