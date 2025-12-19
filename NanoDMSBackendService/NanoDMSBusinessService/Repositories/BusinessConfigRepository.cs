using Microsoft.EntityFrameworkCore;
using NanoDMSBusinessService.Data;
using NanoDMSBusinessService.Models;

namespace NanoDMSBusinessService.Repositories
{
    public class BusinessConfigRepository: Repository<BusinessConfig>, IBusinessConfigRepository
    {
        public BusinessConfigRepository(AppDbContext context) : base(context) { }

        public async Task<Dictionary<string, object>> GetConfigValuesAsync(IEnumerable<string> keys)
        {
            var configEntries = await _context.Set<BusinessConfig>()
                                              .Where(config => keys.Contains(config.NameKey))
                                              .ToListAsync();

            var result = new Dictionary<string, object>();

            foreach (var entry in configEntries)
            {
                result[entry.NameKey] = ParseConfigValue(entry.ConfigValue, entry.ConfigType);
            }

            return result;
        }

        private object ParseConfigValue(string value, string type)
        {
            return type.ToLower() switch
            {
                "bool" => bool.TryParse(value, out var boolValue) ? boolValue : false,
                "int" => int.TryParse(value, out var intValue) ? intValue : 0,
                "float" => float.TryParse(value, out var floatValue) ? floatValue : 0f,
                "double" => double.TryParse(value, out var doubleValue) ? doubleValue : 0d,
                _ => value
            };
        }
    }
}
