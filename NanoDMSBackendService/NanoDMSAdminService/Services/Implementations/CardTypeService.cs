using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CardLevel;
using NanoDMSAdminService.DTO.CardType;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Services.Implementations
{
    public class CardTypeService : ICardTypeService
    {
        private readonly IUnitOfWork _uow;

        public CardTypeService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CardTypeDto> CreateAsync(CardTypeCreateDto dto, string userId)
        {
            var entity = new CardType
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

            await _uow.CardTypes.AddAsync(entity);
            await _uow.SaveAsync();

            return MapToDto(entity);
        }

        public async Task<CardTypeDto> DeleteAsync(Guid id, string userId)
        {
            var cardType = await _uow.CardTypes.GetByIdAsync(id);
            if (cardType == null) return new CardTypeDto();

            cardType.Deleted = true;
            cardType.Published = false;
            cardType.Is_Active = false;
            cardType.RecordStatus = Blocks.RecordStatus.Inactive;
            cardType.Last_Update_Date = DateTime.UtcNow;
            cardType.Last_Update_User = Guid.Parse(userId);

            _uow.CardTypes.Update(cardType);
            await _uow.SaveAsync();

            return MapToDto(cardType);
        }

        public async Task<IEnumerable<CardTypeDto>> GetAllAsync()
        {
            var cardType = await _uow.CardTypes.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            return cardType.Select(x => new CardTypeDto
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

        public async Task<CardTypeDto?> GetByIdAsync(Guid id)
        {
            var cardType = await _uow.CardTypes.GetByIdAsync(id);
            return cardType == null ? null : MapToDto(cardType);
        }

        public async Task<PaginatedResponseDto<CardTypeDto>> GetPagedAsync(CardTypeFilterModel filter)
        {
            var query = _uow.CardTypes.GetQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(x => x.Name.Contains(filter.Name));

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var cardTypes = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseDto<CardTypeDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = cardTypes.Select(MapToDto).ToList()
            };
        }

        public async Task<CardTypeDto> UpdateAsync(Guid id, CardTypeUpdateDto dto, string userId)
        {
            var entity = await _uow.CardTypes.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Card Type not found");

            entity.Name = dto.Name;

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.CardTypes.Update(entity);
            await _uow.SaveAsync();
            return MapToDto(entity);
        }

        private static CardTypeDto MapToDto(CardType x) => new()
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
