using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;
using System.Linq;

namespace RL.Backend.Commands.Handlers.Procedure
{
    public class DeleteUsersFromProcedureCommandHandler : IRequestHandler<DeleteUsersFromProcedureCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;
        public DeleteUsersFromProcedureCommandHandler(RLContext context)
        {
            _context = context;
        }
    public async Task<ApiResponse<Unit>> Handle(DeleteUsersFromProcedureCommand request, CancellationToken cancellationToken)
    {
    try
    {

        if (request.PlanId < 1)
            return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
        if (request.ProcedureId < 1)
            return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));


        var plan = await _context.Plans
            .Include(p => p.PlanProcedureUsers)
            .FirstOrDefaultAsync(p => p.PlanId == request.PlanId, cancellationToken);
        if (plan is null)
            return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} not found"));

        var procedure = await _context.Procedures
            .Include(p => p.PlanProcedureUsers)
            .FirstOrDefaultAsync(p => p.ProcedureId == request.ProcedureId, cancellationToken);
        if (procedure is null)
            return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found"));


        if (request.UserIds == null || !request.UserIds.Any())
            return ApiResponse<Unit>.Fail(new BadRequestException("No UserIds provided"));

        var validUserIds = await _context.Users
            .Select(u => u.UserId)
            .ToListAsync(cancellationToken);
        if (!request.UserIds.All(id => validUserIds.Contains(id)))
            return ApiResponse<Unit>.Fail(new BadRequestException("One or more UserIds are invalid"));

        // Filtering users to delete
        var usersToRemove = procedure.PlanProcedureUsers
            .Where(ppu => ppu.PlanId == request.PlanId 
                          && ppu.ProcedureId == request.ProcedureId 
                          && request.UserIds.Contains(ppu.UserId))
            .ToList();

        if (usersToRemove.Any())
        {
            _context.PlanProcedureUsers.RemoveRange(usersToRemove);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<Unit>.Succeed(Unit.Value);
    }
    catch (Exception ex)
    {
        return ApiResponse<Unit>.Fail(ex);
    }
}

    }
}