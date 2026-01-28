using Microsoft.Extensions.Caching.Distributed;
using NanoDMSRightsService.DTO.Claims;
using NanoDMSRightsService.Services.Interfaces;
using System.Text.Json;

namespace NanoDMSRightsService.Services.Implementations
{
    public class RightsCacheService : IRightsCacheService
    {
        private readonly IRightsService _rightsService;
        private readonly IDistributedCache _cache;

        public RightsCacheService(IRightsService rightsService, IDistributedCache cache)
        {
            _rightsService = rightsService;
            _cache = cache;
        }

        public async Task<UserClaimsDto> GetUserClaimsAsync(List<Guid> roleIds)
        {
            var cacheKey = $"rights:claims:{roleIds}";
            var cached = await _cache.GetStringAsync(cacheKey);

            if (cached != null)
                return JsonSerializer.Deserialize<UserClaimsDto>(cached)!;

            var claims = await _rightsService.GetClaimsByRolesAsync(roleIds);

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(claims),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });

            return claims;
        }

        public async Task InvalidateUserAsync(Guid userId)
        {
            await _cache.RemoveAsync($"rights:claims:{userId}");
        }
    }

}
