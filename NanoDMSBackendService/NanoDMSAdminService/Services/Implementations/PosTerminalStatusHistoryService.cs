using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PosTerminalConfiguration;
using NanoDMSAdminService.DTO.PosTerminalStatusHistory;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Services.Implementations
{
    public class PosTerminalStatusHistoryService : IPosTerminalStatusHistoryService
    {
        private readonly IUnitOfWork _uow;

        public PosTerminalStatusHistoryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PosTerminalStatusHistoryDto> CreateAsync(PosTerminalStatusHistoryCreateDto dto, string userId)
        {
            var terminalStatusHistory = new PosTerminalStatusHistory
            {
                Id = Guid.NewGuid(),
                Pos_Terminal_Id = dto.Pos_Terminal_Id,
                Status = dto.Status,
                Notes = dto.Notes,
                BusinessLocation_Id = dto.Business_Location_Id,
                Business_Id = dto.Business_Id,

                Is_Active = true,
                Deleted = false,
                Published = true,
                RecordStatus = Blocks.RecordStatus.Active,

                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId)
            };

            await _uow.PosTerminalStatusHistories.AddAsync(terminalStatusHistory);
            await _uow.SaveAsync();

            return MapToDto(terminalStatusHistory);
        }

        public async Task<PosTerminalStatusHistoryDto> DeleteAsync(Guid id, string userId)
        {
            var terminalStatusHistory = await _uow.PosTerminalStatusHistories.GetByIdAsync(id);
            if (terminalStatusHistory == null) return new PosTerminalStatusHistoryDto();

            terminalStatusHistory.Deleted = true;
            terminalStatusHistory.Published = false;
            terminalStatusHistory.Is_Active = false;
            terminalStatusHistory.RecordStatus = Blocks.RecordStatus.Inactive;
            terminalStatusHistory.Last_Update_Date = DateTime.UtcNow;
            terminalStatusHistory.Last_Update_User = Guid.Parse(userId);

            _uow.PosTerminalStatusHistories.Update(terminalStatusHistory);
            await _uow.SaveAsync();

            return MapToDto(terminalStatusHistory);
        }

        public async Task<IEnumerable<PosTerminalStatusHistoryDto>> GetAllAsync()
        {
            var terminalStatusHistory = await _uow.PosTerminalStatusHistories.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            return terminalStatusHistory.Select(x => new PosTerminalStatusHistoryDto
            {
                Id = x.Id,
                Pos_Terminal_Id = x.Pos_Terminal_Id,
                Status = x.Status,
                Notes = x.Notes,
                Is_Active = x.Is_Active,
                Deleted = x.Deleted,
                Published = x.Published,
                Business_Id = x.Business_Id,
                BusinessLocation_Id = x.BusinessLocation_Id,
                Create_Date = x.Create_Date,
                Create_User = x.Create_User,
                Last_Update_Date = x.Last_Update_Date,
                Last_Update_User = x.Last_Update_User,
                RecordStatus = x.RecordStatus
            });
        }

        public async Task<PosTerminalStatusHistoryDto?> GetByIdAsync(Guid id)
        {
            var terminalStatusHistory = await _uow.PosTerminalStatusHistories.GetByIdAsync(id);
            return terminalStatusHistory == null ? null : MapToDto(terminalStatusHistory);
        }

        public async Task<PaginatedResponseDto<PosTerminalStatusHistoryDto>> GetPagedAsync(PosTerminalStatusHistoryFilterModel filter)
        {
            var query = _uow.PosTerminalStatusHistories.GetQueryable();

            if (filter.Pos_Terminal_Id.HasValue)
                query = query.Where(x => x.Pos_Terminal_Id == filter.Pos_Terminal_Id);

            if (filter.Status.HasValue)
                query = query.Where(x => x.Status == filter.Status);

            if (!string.IsNullOrWhiteSpace(filter.Notes))
                query = query.Where(x => x.Notes!.Contains(filter.Notes));

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var terminalStatusHistory = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseDto<PosTerminalStatusHistoryDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = terminalStatusHistory.Select(MapToDto).ToList()
            };
        }

        public async Task<PosTerminalStatusHistoryDto> UpdateAsync(Guid id, PosTerminalStatusHistoryUpdateDto dto, string userId)
        {
            var entity = await _uow.PosTerminalStatusHistories.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Pos Terminal Status HIstory not found");

            entity.Pos_Terminal_Id = dto.Pos_Terminal_Id;
            entity.Status = dto.Status;
            entity.Notes = dto.Notes;

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.PosTerminalStatusHistories.Update(entity);
            await _uow.SaveAsync();
            return MapToDto(entity);
        }

        private static PosTerminalStatusHistoryDto MapToDto(PosTerminalStatusHistory x) => new()
        {
            Id = x.Id,
            Pos_Terminal_Id = x.Pos_Terminal_Id,
            Status = x.Status,
            Notes = x.Notes,
            Is_Active = x.Is_Active,
            Deleted = x.Deleted,
            Published = x.Published,
            Business_Id = x.Business_Id,
            BusinessLocation_Id = x.BusinessLocation_Id,
            Create_Date = x.Create_Date,
            Create_User = x.Create_User,
            Last_Update_Date = x.Last_Update_Date,
            Last_Update_User = x.Last_Update_User,
            RecordStatus = x.RecordStatus
        };
    }
}
