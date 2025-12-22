using System.ComponentModel.DataAnnotations.Schema;
using NanoDMSSetupService.Common;
using NanoDMSSetupService.Models;

namespace NanoDMSSetupService.DTO
{
    public class RegisterCityModel 
    {
        
        public string StateId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
