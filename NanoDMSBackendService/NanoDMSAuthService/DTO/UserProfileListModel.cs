using NanoDMSAuthService.Common;

namespace NanoDMSAuthService.DTO
{
    public class UserProfileListModel:BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string PermanentAddress { get; set; } = string.Empty;
        public string CurrentAddress { get; set; } = string.Empty;
        public Guid City { get; set; }
        public Guid State { get; set; }
        public Guid Country { get; set; }
        public string PostalCode { get; set; } = string.Empty;
        public string PersonalEmailAddress { get; set; } = string.Empty;
        public string CNIC { get; set; } = string.Empty;
        public DateTime Dob { get; set; }
        public Guid Gender { get; set; }
        public Guid MaritalStatus { get; set; }
        public string BloodGroup { get; set; } = string.Empty;
        public string AlternateNumber { get; set; } = string.Empty;
        public string EmergencyContact { get; set; } = string.Empty;
        public string NTNNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string BankBranch { get; set; } = string.Empty;
        public string BankIBAN { get; set; } = string.Empty;
        public string BankAccountNumber { get; set; } = string.Empty;
        public string BankAccountName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;

        // From AspNetUsers
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string UserRole { get; set; } = string.Empty;


    }
}
