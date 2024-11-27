using MediatR;
using ProjectAPI.Application.Common.Interfaces;
using ProjectAPI.Application.Common.Models;
using ProjectAPI.Application.Common.Security;
using ProjectAPI.Application.Security.Models;
using ProjectAPI.Infrastructure.Common;

namespace ProjectAPI.Application.Security.Queries;

[Authorize(Roles = $"{Roles.Master},{Roles.Usuario}")]
public class GetUsuariosQuery : IRequest<PaginatedList<ApplicationUserDto>>
{
    public int PageSize { get; set; } = 20;
    public int PageNumber { get; set; } = 0;
}

public class GetUsuariosQueryHandler : IRequestHandler<GetUsuariosQuery, PaginatedList<ApplicationUserDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;

    public GetUsuariosQueryHandler(IIdentityService identityService, IApplicationDbContext context)
    {
        _identityService = identityService;
        _context = context;
    }

    public async Task<PaginatedList<ApplicationUserDto>> Handle(GetUsuariosQuery request, CancellationToken cancellationToken)
    {
        var res = await _identityService.GetUsersAsync(request.PageSize, request.PageNumber);
        return res;
    }
}