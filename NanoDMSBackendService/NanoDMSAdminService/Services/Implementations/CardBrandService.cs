using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CardBin;
using NanoDMSAdminService.DTO.CardBrand;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Services.Implementations
{
    public class CardBrandService : ICardBrandService
    {
        private readonly IUnitOfWork _uow;

        public CardBrandService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CardBrandDto> CreateAsync(CardBrandCreateDto dto, string userId)
        {
            var entity = new CardBrand
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

            await _uow.CardBrands.AddAsync(entity);
            await _uow.SaveAsync();

            return MapToDto(entity);
        }

        public async Task<CardBrandDto> DeleteAsync(Guid id, string userId)
        {
            var cardBrand = await _uow.CardBrands.GetByIdAsync(id);
            if (cardBrand == null) return new CardBrandDto();

            cardBrand.Deleted = true;
            cardBrand.Published = false;
            cardBrand.Is_Active = false;
            cardBrand.RecordStatus = Blocks.RecordStatus.Inactive;
            cardBrand.Last_Update_Date = DateTime.UtcNow;
            cardBrand.Last_Update_User = Guid.Parse(userId);

            _uow.CardBrands.Update(cardBrand);
            await _uow.SaveAsync();

            return MapToDto(cardBrand);
        }

        public async Task<IEnumerable<CardBrandDto>> GetAllAsync()
        {
            var cardBrand = await _uow.CardBrands.GetAllByConditionAsync(b =>
               !b.Deleted && b.Is_Active
           );

            return cardBrand.Select(x => new CardBrandDto
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

        public async Task<CardBrandDto?> GetByIdAsync(Guid id)
        {
            var cardBrand = await _uow.CardBrands.GetByIdAsync(id);
            return cardBrand == null ? null : MapToDto(cardBrand);
        }

        public async Task<PaginatedResponseDto<CardBrandDto>> GetPagedAsync(CardBrandFilterModel filter)
        {
            var query = _uow.CardBrands.GetQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(x => x.Name.Contains(filter.Name));

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var cardBrands = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseDto<CardBrandDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = cardBrands.Select(MapToDto).ToList()
            };
        }

        public async Task<CardBrandDto> UpdateAsync(Guid id, CardBrandUpdateDto dto, string userId)
        {
            var entity = await _uow.CardBrands.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Card Brand not found");

            entity.Name = dto.Name;

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.CardBrands.Update(entity);
            await _uow.SaveAsync();
            return MapToDto(entity);
        }

        private static CardBrandDto MapToDto(CardBrand x) => new()
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
