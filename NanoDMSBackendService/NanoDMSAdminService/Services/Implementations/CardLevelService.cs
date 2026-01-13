using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CardBrand;
using NanoDMSAdminService.DTO.CardLevel;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Services.Implementations
{
    public class CardLevelService : ICardLevelService
    {
        private readonly IUnitOfWork _uow;

        public CardLevelService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CardLevelDto> CreateAsync(CardLevelCreateDto dto, string userId)
        {
            var entity = new CardLevel
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                RecordStatus = RecordStatus.Active,
                Published = true,
                Deleted = false,
                Is_Active = true,
                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId),
                Business_Id = dto.Business_Id,
                BusinessLocation_Id = dto.Business_Location_Id
            };

            await _uow.CardLevels.AddAsync(entity);
            await _uow.SaveAsync();

            return MapToDto(entity);
        }

        public async Task<CardLevelDto> DeleteAsync(Guid id, string userId)
        {
            var cardLevel = await _uow.CardLevels.GetByIdAsync(id);
            if (cardLevel == null) return new CardLevelDto();

            cardLevel.Deleted = true;
            cardLevel.Published = false;
            cardLevel.Is_Active = false;
            cardLevel.RecordStatus = Blocks.RecordStatus.Inactive;
            cardLevel.Last_Update_Date = DateTime.UtcNow;
            cardLevel.Last_Update_User = Guid.Parse(userId);

            _uow.CardLevels.Update(cardLevel);
            await _uow.SaveAsync();

            return MapToDto(cardLevel);
        }

        public async Task<IEnumerable<CardLevelDto>> GetAllAsync()
        {
            var cardLevel = await _uow.CardLevels.GetAllByConditionAsync(b =>
               !b.Deleted && b.Is_Active
           );

            return cardLevel.Select(x => new CardLevelDto
            {
                Id = x.Id,
                Name = x.Name,
                BusinessLocation_Id = x.BusinessLocation_Id,
                Business_Id = x.Business_Id,
                Is_Active = x.Is_Active,
                Deleted = x.Deleted,
                Published = x.Published,
                Create_Date = x.Create_Date,
                Create_User = x.Create_User,
                Last_Update_Date = x.Last_Update_Date,
                Last_Update_User = x.Last_Update_User,
                RecordStatus = x.RecordStatus
            });
        }

        public async Task<CardLevelDto?> GetByIdAsync(Guid id)
        {
            var cardLevel = await _uow.CardLevels.GetByIdAsync(id);
            return cardLevel == null ? null : MapToDto(cardLevel);
        }

        public async Task<PaginatedResponseDto<CardLevelDto>> GetPagedAsync(CardLevelFilterModel filter)
        {
            var query = _uow.CardLevels.GetQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(x => x.Name.Contains(filter.Name));

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var cardLevels = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseDto<CardLevelDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = cardLevels.Select(MapToDto).ToList()
            };
        }

        public async Task<CardLevelDto> UpdateAsync(Guid id, CardLevelUpdateDto dto, string userId)
        {
            var entity = await _uow.CardLevels.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Card Level not found");

            entity.Name = dto.Name;

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.CardLevels.Update(entity);
            await _uow.SaveAsync();
            return MapToDto(entity);
        }

        private static CardLevelDto MapToDto(CardLevel x) => new()
        {
            Id = x.Id,
            Name = x.Name,
            Is_Active = x.Is_Active,
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
