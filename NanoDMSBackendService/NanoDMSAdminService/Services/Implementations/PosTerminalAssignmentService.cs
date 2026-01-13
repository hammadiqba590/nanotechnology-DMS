using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PosTerminalAssignment;
using NanoDMSAdminService.DTO.PosTerminalMaster;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Services.Implementations
{
    public class PosTerminalAssignmentService : IPosTerminalAssignmentService
    {
        private readonly IUnitOfWork _uow;

        public PosTerminalAssignmentService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PosTerminalAssignmentDto> CreateAsync(PosTerminalAssignmentCreateDto dto, string userId)
        {
            var terminalAssignment = new PosTerminalAssignment
            {
                Id = Guid.NewGuid(),
                PosTerminal_Id = dto.PosTerminal_Id,
                Mid = dto.Mid,
                Tid = dto.Tid,
                Assigned_At = dto.Assigned_At,
                Unassigned_At = dto.Unassigned_At,
                BusinessLocation_Id = dto.Business_Location_Id,
                Business_Id = dto.Business_Id,

                Is_Active = true,
                Deleted = false,
                Published = true,
                RecordStatus = Blocks.RecordStatus.Active,

                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId)
            };

            await _uow.PosTerminalAssignments.AddAsync(terminalAssignment);
            await _uow.SaveAsync();

            return MapToDto(terminalAssignment);
        }

        public async Task<PosTerminalAssignmentDto> DeleteAsync(Guid id, string userId)
        {
            var terminalAssignment = await _uow.PosTerminalAssignments.GetByIdAsync(id);
            if (terminalAssignment == null) return new PosTerminalAssignmentDto();

            terminalAssignment.Deleted = true;
            terminalAssignment.Published = false;
            terminalAssignment.Is_Active = false;
            terminalAssignment.RecordStatus = Blocks.RecordStatus.Inactive;
            terminalAssignment.Last_Update_Date = DateTime.UtcNow;
            terminalAssignment.Last_Update_User = Guid.Parse(userId);

            _uow.PosTerminalAssignments.Update(terminalAssignment);
            await _uow.SaveAsync();

            return MapToDto(terminalAssignment);
        }

        public async Task<IEnumerable<PosTerminalAssignmentDto>> GetAllAsync()
        {
            var terminalAssignment = await _uow.PosTerminalAssignments.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            return terminalAssignment.Select(x => new PosTerminalAssignmentDto
            {
                Id = x.Id,
                PosTerminal_Id = x.PosTerminal_Id,
                Mid = x.Mid,
                Tid = x.Tid,
                Assigned_At = x.Assigned_At,
                Unassigned_At = x.Unassigned_At,
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

        public async Task<PosTerminalAssignmentDto?> GetByIdAsync(Guid id)
        {
            var terminalAssignment = await _uow.PosTerminalAssignments.GetByIdAsync(id);
            return terminalAssignment == null ? null : MapToDto(terminalAssignment);
        }

        public async  Task<PaginatedResponseDto<PosTerminalAssignmentDto>> GetPagedAsync(PosTerminalAssignmentFilterModel filter)
        {
            var query = _uow.PosTerminalAssignments.GetQueryable();

            if (filter.PosTerminal_Id.HasValue)
                query = query.Where(x => x.PosTerminal_Id == filter.PosTerminal_Id);

            if (!string.IsNullOrWhiteSpace(filter.Mid))
                query = query.Where(x => x.Mid.Contains(filter.Mid));

            if (!string.IsNullOrWhiteSpace(filter.Tid))
                query = query.Where(x => x.Tid!.Contains(filter.Tid));

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var terminalAssignment = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseDto<PosTerminalAssignmentDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = terminalAssignment.Select(MapToDto).ToList()
            };
        }

        public async Task<PosTerminalAssignmentDto> UpdateAsync(Guid id, PosTerminalAssignmentUpdateDto dto, string userId)
        {
            var entity = await _uow.PosTerminalAssignments.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Pos Terminal Assignment not found");

            entity.PosTerminal_Id = dto.PosTerminal_Id;
            entity.Mid = dto.Mid;
            entity.Tid = dto.Tid;
            entity.Assigned_At = dto.Assigned_At;
            entity.Unassigned_At = dto.Unassigned_At;
            

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.PosTerminalAssignments.Update(entity);
            await _uow.SaveAsync();
            return MapToDto(entity);
        }

        private static PosTerminalAssignmentDto MapToDto(PosTerminalAssignment x) => new()
        {
            Id = x.Id,
            PosTerminal_Id = x.PosTerminal_Id,
            Mid = x.Mid,
            Tid = x.Tid,
            Assigned_At = x.Assigned_At,
            Unassigned_At = x.Unassigned_At,
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
