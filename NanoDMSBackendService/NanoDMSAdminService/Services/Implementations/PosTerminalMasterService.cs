using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CardType;
using NanoDMSAdminService.DTO.PosTerminalMaster;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Services.Implementations
{
    public class PosTerminalMasterService : IPosTerminalMasterService
    {
        private readonly IUnitOfWork _uow;

        public PosTerminalMasterService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PosTerminalMasterDto> CreateAsync(PosTerminalMasterCreateDto dto, string userId)
        {
            var terminal = new PosTerminalMaster
            {
                Id = Guid.NewGuid(),
                Serial_Number = dto.Serial_Number,
                Terminal_Code = dto.Terminal_Code,
                Company = dto.Company,
                Model_Number = dto.Model_Number,
                Software_Version = dto.Software_Version,
                Description = dto.Description,
                BusinessLocation_Id = dto.Business_Location_Id,
                Business_Id = dto.Business_Id,

                Is_Active = true,
                Deleted = false,
                Published = true,
                RecordStatus = Blocks.RecordStatus.Active,

                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId)
            };

            await _uow.PosTerminalMasters.AddAsync(terminal);
            await _uow.SaveAsync();

            return MapToDto(terminal);
        }

        public async Task<PosTerminalMasterDto> DeleteAsync(Guid id, string userId)
        {
            var terminal = await _uow.PosTerminalMasters.GetByIdAsync(id);
            if (terminal == null) return new PosTerminalMasterDto();

            terminal.Deleted = true;
            terminal.Published = false;
            terminal.Is_Active = false;
            terminal.RecordStatus = Blocks.RecordStatus.Inactive;
            terminal.Last_Update_Date = DateTime.UtcNow;
            terminal.Last_Update_User = Guid.Parse(userId);

            _uow.PosTerminalMasters.Update(terminal);
            await _uow.SaveAsync();

            return MapToDto(terminal);
        }

        public async Task<IEnumerable<PosTerminalMasterDto>> GetAllAsync()
        {
            var terminal = await _uow.PosTerminalMasters.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            return terminal.Select(x => new PosTerminalMasterDto
            {
                Id = x.Id,
                Serial_Number = x.Serial_Number,
                Terminal_Code = x.Terminal_Code,
                Company = x.Company,
                Model_Number = x.Model_Number,
                Software_Version = x.Software_Version,
                Description = x.Description,
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

        public async Task<PosTerminalMasterDto?> GetByIdAsync(Guid id)
        {
            var terminal = await _uow.PosTerminalMasters.GetByIdAsync(id);
            return terminal == null ? null : MapToDto(terminal);
        }

        public async Task<PaginatedResponseDto<PosTerminalMasterDto>> GetPagedAsync(PosTerminalMasterFilterModel filter)
        {
            var query = _uow.PosTerminalMasters.GetQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Serial_Number))
                query = query.Where(x => x.Serial_Number.Contains(filter.Serial_Number));

            if(!string.IsNullOrWhiteSpace(filter.Terminal_Code))
                query = query.Where(x => x.Terminal_Code!.Contains(filter.Terminal_Code));

            if (!string.IsNullOrWhiteSpace(filter.Company))
                query = query.Where(x => x.Company!.Contains(filter.Company));

            if (!string.IsNullOrWhiteSpace(filter.Model_Number))
                query = query.Where(x => x.Model_Number!.Contains(filter.Model_Number));


            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var terminal = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseDto<PosTerminalMasterDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = terminal.Select(MapToDto).ToList()
            };
        }

        public async Task<PosTerminalMasterDto> UpdateAsync(Guid id, PosTerminalMasterUpdateDto dto, string userId)
        {
            var entity = await _uow.PosTerminalMasters.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Pos Terminal not found");

            entity.Serial_Number = dto.Serial_Number;
            entity.Model_Number = dto.Model_Number;
            entity.Software_Version = dto.Software_Version;
            entity.Company = dto.Company;
            entity.Terminal_Code = dto.Terminal_Code;
            entity.Description = dto.Description;

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.PosTerminalMasters.Update(entity);
            await _uow.SaveAsync();
            return MapToDto(entity);
        }

        private static PosTerminalMasterDto MapToDto(PosTerminalMaster x) => new()
        {
            Id = x.Id,
            Serial_Number = x.Serial_Number,
            Terminal_Code = x.Terminal_Code,
            Company = x.Company,
            Model_Number = x.Model_Number,
            Software_Version = x.Software_Version,
            Description = x.Description,
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
