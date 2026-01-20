using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Campagin;
using NanoDMSAdminService.DTO.CampaignBank;
using NanoDMSAdminService.DTO.CampaignCardBin;
using NanoDMSAdminService.DTO.Country;
using NanoDMSAdminService.DTO.DiscountRule;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;
using NanoDMSSharedLibrary.CacheKeys;
using System.Diagnostics.Metrics;
using System.Text.Json;

namespace NanoDMSAdminService.Services.Implementations
{
    public class CampaignService : ICampaignService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDistributedCache _cache;
        public CampaignService(IUnitOfWork uow, IDistributedCache cache)
        {
            _uow = uow;
            _cache = cache;
        }

        public async Task<IEnumerable<CampaignDto>> GetAllAsync()
        {
            const string cacheKey = "campaigns:all";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<CampaignDto>>(cached)!;

            var campaigns = await _uow.Campaigns.GetAllByConditionAsync(b =>!b.Deleted && b.Is_Active);

            var result =  campaigns.Select(b => new CampaignDto
            {
                Id = b.Id,
                Campaign_Name = b.Campaign_Name,
                Description = b.Description,
                Currency_Id = b.Currency_Id,
                Tax_Amount = b.Tax_Amount,
                Fbr = b.Fbr,
                Status = b.Status,
                Budget_Limit_Type = b.Budget_Limit_Type,
                Budget_Limit_Value = b.Budget_Limit_Value,
                Priority = b.Priority,
                Start_Date = b.Start_Date,
                End_Date = b.End_Date,
                Deleted = b.Deleted,
                Published = b.Published,
                Create_Date = b.Create_Date,
                Create_User = b.Create_User,
                Last_Update_Date = b.Last_Update_Date,
                Last_Update_User = b.Last_Update_User,
                Business_Id = b.Business_Id,
                BusinessLocation_Id = b.BusinessLocation_Id,
                Is_Active = b.Is_Active,
                RecordStatus = b.RecordStatus,
            });

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

            return result;
        }

        public async Task<CampaignDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"campaigns:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<CampaignDto>(cached);

            var campaign = await _uow.Campaigns.GetByIdAsync(id);
            var dto = campaign == null ? null : MapToDto(campaign);

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return dto;
        }

        public async Task<PaginatedResponseDto<CampaignDto>> GetPagedAsync(CampaignFilterModel filter)
        {
            var cacheKey = CampaignCacheKeys.Paged(filter.PageNumber, filter.PageSize);

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResponseDto<CampaignDto>>(cached)!;

            var query = _uow.Campaigns.GetQueryable();

            if (!string.IsNullOrEmpty(filter.Campaign_Name))
                query = query.Where(x => x.Campaign_Name.Contains(filter.Campaign_Name));

            if (filter.Status.HasValue)
                query = query.Where(x => x.Status == filter.Status);

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var campaigns = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result =  new PaginatedResponseDto<CampaignDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = campaigns.Select(MapToDto).ToList()
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)});

            return result;
        }
        public async Task<CampaignDto> CreateAsync(CampaignCreateDto dto, string userId)
        {
            var campaign = new Campaign
            {
                Id = Guid.NewGuid(),
                Campaign_Name = dto.Campaign_Name,
                Description = dto.Description,
                Currency_Id = dto.Currency_Id,
                Tax_Amount = dto.Tax_Amount,
                Fbr = dto.Fbr,
                Budget_Limit_Type = dto.Budget_Limit_Type,
                Budget_Limit_Value = dto.Budget_Limit_Value,
                Priority = dto.Priority,
                Status = dto.Status,
                Start_Date = dto.Start_Date,
                End_Date = dto.End_Date,

                Published = true,
                Deleted = false,
                Is_Active = true,
                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId),
                RecordStatus = RecordStatus.Active,
                Business_Id = dto.Business_Id,
                BusinessLocation_Id = dto.Business_Location_Id
            };

            await _uow.Campaigns.AddAsync(campaign);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(CampaignCacheKeys.All);


            return MapToDto(campaign);
        }

        public async Task<CampaignDto> UpdateAsync(Guid id, CampaignUpdateDto dto, string userId)
        {
            var campaign = await _uow.Campaigns.GetByIdAsync(id);
            if (campaign == null) return new CampaignDto();

            campaign.Campaign_Name = dto.Campaign_Name;
            campaign.Description = dto.Description;
            campaign.Currency_Id = dto.Currency_Id;
            campaign.Tax_Amount = dto.Tax_Amount;
            campaign.Fbr = dto.Fbr;
            campaign.Budget_Limit_Type = dto.Budget_Limit_Type;
            campaign.Budget_Limit_Value = dto.Budget_Limit_Value;
            campaign.Priority = dto.Priority;
            campaign.Status = dto.Status;
            campaign.Start_Date = dto.Start_Date;
            campaign.End_Date = dto.End_Date;

            campaign.Last_Update_Date = DateTime.UtcNow;
            campaign.Last_Update_User = Guid.Parse(userId);

            _uow.Campaigns.Update(campaign);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(CampaignCacheKeys.All);
            await _cache.RemoveAsync(CampaignCacheKeys.ById(id));


            return MapToDto(campaign);
        }

        public async Task<CampaignDto> DeleteAsync(Guid id, string userId)
        {
            var campaign = await _uow.Campaigns.GetByIdAsync(id);
            if (campaign == null) return new CampaignDto();

            campaign.Deleted = true;
            campaign.Published = false;
            campaign.Is_Active = false;
            campaign.Status = CampaginStatus.Inactive;
            campaign.RecordStatus = RecordStatus.Inactive;
            campaign.Last_Update_Date = DateTime.UtcNow;
            campaign.Last_Update_User = Guid.Parse(userId);

            _uow.Campaigns.Update(campaign);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(CampaignCacheKeys.All);
            await _cache.RemoveAsync(CampaignCacheKeys.ById(id));


            return MapToDto(campaign);
        }
        public async Task<List<Campaign>> GetActiveCampaignsByTerminalAsync(string serialNumber)
        {
            var now = DateTime.UtcNow;

            return await _uow.Campaigns.GetQueryable()
                .Where(c =>
                    !c.Deleted &&
                    c.Is_Active &&
                    c.RecordStatus == RecordStatus.Active &&
                    c.Status == CampaginStatus.Active &&
                    c.Start_Date <= now &&
                    c.End_Date >= now &&
                    _uow.PosTerminalMasters.GetQueryable().Any(a =>
                        a.Serial_Number == serialNumber &&
                        a.BusinessLocation_Id == c.BusinessLocation_Id &&
                        !a.Deleted
                    )
                )
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CampaignFullResponseDto> CreateFullCampaignAsync(CampaignFullCreateDto dto,Guid userId)
        {
            CampaignFullResponseDto? response = null;

            await _uow.ExecuteInTransactionAsync(async () =>
            {
                var campaign = new Campaign
                {
                    Id = Guid.NewGuid(),
                    Campaign_Name = dto.Campaign_Name,
                    Description = dto.Description,
                    Currency_Id = dto.Currency_Id,
                    Psp_Id = dto.Psp_Id,
                    Tax_Amount = dto.Tax_Amount,
                    Fbr = dto.Fbr,
                    Budget_Limit_Type = dto.Budget_Limit_Type,
                    Budget_Limit_Value = dto.Budget_Limit_Value,
                    Priority = dto.Priority,
                    Start_Date = dto.Start_Date,
                    End_Date = dto.End_Date,
                    Status = dto.Status,
                    RecordStatus = RecordStatus.Active,
                    Is_Active = true,
                    Published = true,
                    Deleted = false,
                    Create_Date = DateTime.UtcNow,
                    Create_User = userId,
                    Business_Id = dto.Business_Id,
                    BusinessLocation_Id = dto.Business_Location_Id
                };

                await _uow.Campaigns.AddAsync(campaign);


                // =========================
                // RESPONSE ROOT
                // =========================
                response = new CampaignFullResponseDto
                {
                    Id = campaign.Id,
                    Campaign_Name = campaign.Campaign_Name,
                    Description = campaign.Description,
                    Currency_Id = campaign.Currency_Id,
                    Currency_Name = campaign.Currency?.Name ?? string.Empty,
                    Psp_Id = campaign.Psp_Id,
                    Psp_Name = campaign.Psps?.Name ?? string.Empty,
                    Tax_Amount = campaign.Tax_Amount,
                    Fbr = campaign.Fbr,
                    Status = campaign.Status,
                    Budget_Limit_Type = campaign.Budget_Limit_Type,
                    Budget_Limit_Value = campaign.Budget_Limit_Value,
                    Priority = campaign.Priority,
                    Start_Date = campaign.Start_Date,
                    End_Date = campaign.End_Date,
                    RecordStatus = campaign.RecordStatus,
                    Is_Active = campaign.Is_Active,
                    Published = campaign.Published,
                    Deleted = campaign.Deleted,
                    Create_Date = campaign.Create_Date,
                    Create_User = campaign.Create_User,
                    Business_Id = campaign.Business_Id,
                    BusinessLocation_Id = campaign.BusinessLocation_Id,
                    Banks = new List<CampaignBankResponseDto>()
                };

                foreach (var bankDto in dto.Banks)
                {
                    var campaignBank = new CampaignBank
                    {
                        Id = Guid.NewGuid(),
                        Campagin_Id = campaign.Id,
                        Bank_Id = bankDto.Bank_Id,
                        Budget = bankDto.Budget,
                        Discount_Share = bankDto.Discount_Share,
                        Tax_On_Merchant_Share = bankDto.Tax_On_Merchant_Share,
                        Budget_Limit_Type = bankDto.Budget_Limit_Type,
                        Budget_Limit_Value = bankDto.Budget_Limit_Value,
                        Discount_Mode = bankDto.Discount_Mode,
                        Status = bankDto.Status,
                        Start_Date = bankDto.Start_Date,
                        End_Date = bankDto.End_Date,
                        RecordStatus = RecordStatus.Active,
                        Is_Active = true,
                        Published = true,
                        Deleted = false,
                        Create_Date = DateTime.UtcNow,
                        Create_User = userId,
                        Business_Id = dto.Business_Id,
                        BusinessLocation_Id = dto.Business_Location_Id
                    };

                    await _uow.CampaignBanks.AddAsync(campaignBank);

                    var bankResponse = new CampaignBankResponseDto
                    {
                        Id = campaignBank.Id,
                        Campagin_Id = campaign.Id,
                        Campaign_Name = campaign.Campaign_Name,
                        Bank_Id = campaignBank.Bank_Id,
                        Bank_Name = campaignBank.Bank?.Name ?? string.Empty,
                        Budget = campaignBank.Budget,
                        Discount_Share = campaignBank.Discount_Share,
                        Tax_On_Merchant_Share = campaignBank.Tax_On_Merchant_Share,
                        Budget_Limit_Type = campaignBank.Budget_Limit_Type,
                        Budget_Limit_Value = campaignBank.Budget_Limit_Value,
                        Discount_Mode = campaignBank.Discount_Mode,
                        Status = campaignBank.Status,
                        Start_Date = campaignBank.Start_Date,
                        End_Date = campaignBank.End_Date,
                        RecordStatus = campaignBank.RecordStatus,
                        Is_Active = campaignBank.Is_Active,
                        Published = campaignBank.Published,
                        Deleted = campaignBank.Deleted,
                        Create_Date = campaignBank.Create_Date,
                        Create_User = campaignBank.Create_User,
                        Business_Id = campaignBank.Business_Id,
                        BusinessLocation_Id = campaignBank.BusinessLocation_Id,
                        CardBins = new List<CampaignCardBinResponseDto>()
                    };

                    response.Banks.Add(bankResponse);

                    foreach (var binDto in bankDto.CardBins)
                    {
                        var cardBin = new CampaignCardBin
                        {
                            Id = Guid.NewGuid(),
                            Campagin_Id = campaign.Id,
                            Campagin_Bank_Id = campaignBank.Id,
                            Card_Bin_Id = binDto.Card_Bin_Id,
                            Status = binDto.Status,
                            RecordStatus = RecordStatus.Active,
                            Is_Active = true,
                            Published = true,
                            Deleted = false,
                            Create_Date = DateTime.UtcNow,
                            Create_User = userId,
                            Business_Id = dto.Business_Id,
                            BusinessLocation_Id = dto.Business_Location_Id
                        };

                        await _uow.CampaignCardBins.AddAsync(cardBin);

                        var cardBinResponse = new CampaignCardBinResponseDto
                        {
                            Id = cardBin.Id,
                            Campagin_Id = cardBin.Campagin_Id,
                            Campaign_Name = campaign.Campaign_Name,
                            Card_Bin_Id = cardBin.Card_Bin_Id,
                            Campagin_Bank_Id = cardBin.Campagin_Bank_Id,
                            Bank_Name = campaignBank.Bank?.Name ?? string.Empty,
                            Status = cardBin.Status,
                            Start_Date = cardBin.Start_Date,
                            End_Date = cardBin.End_Date,
                            RecordStatus = cardBin.RecordStatus,
                            Is_Active = cardBin.Is_Active,
                            Published = cardBin.Published,
                            Deleted = cardBin.Deleted,
                            Create_Date = cardBin.Create_Date,
                            Create_User = cardBin.Create_User,
                            Business_Id = cardBin.Business_Id,
                            BusinessLocation_Id = cardBin.BusinessLocation_Id,
                            DiscountRules = new List<DiscountRuleResponseDto>()
                        };

                        bankResponse.CardBins.Add(cardBinResponse);


                        foreach (var ruleDto in binDto.DiscountRules)
                        {
                            var rule = new DiscountRule
                            {
                                Id = Guid.NewGuid(),
                                Campaign_Card_Bin_Id = cardBin.Id,
                                Discount_Type = ruleDto.Discount_Type,
                                Discount_Value = ruleDto.Discount_Value,
                                Min_Spend = ruleDto.Min_Spend,
                                Max_Discount = ruleDto.Max_Discount,
                                Payment_Type = ruleDto.Payment_Type,
                                Budget_Limit_Type = ruleDto.Budget_Limit_Type,
                                Budget_Limit_Value = ruleDto.Budget_Limit_Value,
                                Applicable_Days = ruleDto.Applicable_Days,
                                Transaction_Cap = ruleDto.Transaction_Cap,
                                Priority = ruleDto.Priority,
                                Start_Time = ruleDto.Start_Time,
                                End_Time = ruleDto.End_Time,
                                RecordStatus = RecordStatus.Active,
                                Is_Active = true,
                                Published = true,
                                Deleted = false,
                                Create_Date = DateTime.UtcNow,
                                Create_User = userId,
                                Business_Id = dto.Business_Id,
                                BusinessLocation_Id = dto.Business_Location_Id
                            };

                            await _uow.DiscountRules.AddAsync(rule);

                            await _uow.DiscountRuleHistories.AddAsync(new DiscountRuleHistory
                            {
                                Id = Guid.NewGuid(),
                                Discount_Rule_Id = rule.Id,
                                Campaign_Card_Bin_Id = cardBin.Id,
                                Currency_Id = dto.Currency_Id,
                                Discount_Type = rule.Discount_Type,
                                Discount_Value = rule.Discount_Value,
                                Min_Spend = rule.Min_Spend,
                                Max_Discount = rule.Max_Discount,
                                Payment_Type = rule.Payment_Type,
                                Applicable_Days = rule.Applicable_Days,
                                Transaction_Cap = rule.Transaction_Cap,
                                Priority = rule.Priority,
                                Stackable = true,
                                Change_Type = ChangeTypeStatus.Insert
                            });

                            cardBinResponse.DiscountRules.Add(new DiscountRuleResponseDto
                            {
                                Id = rule.Id,
                                Campaign_Card_Bin_Id = rule.Campaign_Card_Bin_Id,
                                Campaign_Name = campaign.Campaign_Name,
                                Discount_Type = rule.Discount_Type,
                                Discount_Value = rule.Discount_Value,
                                Min_Spend = rule.Min_Spend,
                                Max_Discount = rule.Max_Discount,
                                Payment_Type = rule.Payment_Type,
                                Budget_Limit_Type = rule.Budget_Limit_Type,
                                Budget_Limit_Value = rule.Budget_Limit_Value,
                                Applicable_Days = rule.Applicable_Days,
                                Transaction_Cap = rule.Transaction_Cap,
                                Priority = rule.Priority,
                                Start_Time = rule.Start_Time,
                                End_Time = rule.End_Time,
                                Start_Date = rule.Start_Date,
                                End_Date = rule.End_Date,
                                RecordStatus = rule.RecordStatus,
                                Is_Active = rule.Is_Active,
                                Published = rule.Published,
                                Deleted = rule.Deleted,
                                Create_Date = rule.Create_Date,
                                Create_User = rule.Create_User,
                                Business_Id = rule.Business_Id,
                                BusinessLocation_Id = rule.BusinessLocation_Id,
                            });
                        }
                    }
                }

                await _uow.SaveAsync();

            });

            // 🔥 cache invalidation AFTER transaction successfully committed
            await _cache.RemoveAsync(CampaignCacheKeys.All);

            return response!;
        }


        private static CampaignDto MapToDto(Campaign x) => new()
        {
            Id = x.Id,
            Campaign_Name = x.Campaign_Name,
            Description = x.Description,
            Currency_Id = x.Currency_Id,
            Tax_Amount = x.Tax_Amount,
            Fbr = x.Fbr,
            Status = x.Status,
            Budget_Limit_Type = x.Budget_Limit_Type,
            Budget_Limit_Value = x.Budget_Limit_Value,
            Priority = x.Priority,
            Start_Date = x.Start_Date,
            End_Date = x.End_Date,
            Is_Active = x.Is_Active,
            Business_Id = x.Business_Id,
            BusinessLocation_Id = x.BusinessLocation_Id,
            Deleted = x.Deleted,
            Published = x.Published,
            Create_Date = x.Create_Date,
            Create_User = x.Create_User,
            Last_Update_Date = x.Last_Update_Date,
            Last_Update_User = x.Last_Update_User,
            RecordStatus = x.RecordStatus
        };
    }

}
