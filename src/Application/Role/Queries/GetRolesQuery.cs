using MediatR;
using ProjectAPI.Application.Common.Interfaces;
using ProjectAPI.Application.Common.Security;
using ProjectAPI.Application.Role.Models;
using ProjectAPI.Infrastructure.Common;

namespace ProjectAPI.Application.Role.Queries;

[Authorize(Roles = $"{Roles.Master},{Roles.Usuario}")]
public class GetRolesQuery : IRequest<List<RoleDto>>
{
}

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RoleDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;

    public GetRolesQueryHandler(IIdentityService identityService, IApplicationDbContext context)
    {
        _identityService = identityService;
        _context = context;
    }

    public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var res = await _identityService.GetRolesAsync();

        return res;
    }
}