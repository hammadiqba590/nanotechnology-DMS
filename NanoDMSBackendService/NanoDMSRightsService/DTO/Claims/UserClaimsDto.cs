namespace NanoDMSRightsService.DTO.Claims
{
    public class UserClaimsDto
    {
        public Guid User_Id { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
    }

}
