using MediatR;
using ProjectAPI.Application.Common.Interfaces;
using ProjectAPI.Application.Common.Models;
using ProjectAPI.Application.Common.Security;
using ProjectAPI.Infrastructure.Common;

namespace ProjectAPI.Application.CatalogosPredefinidos.Permiso.Queries;

[Authorize(Roles = $"{Roles.Master},{Roles.Usuario}")]
public class GetPermisosQuery : IRequest<List<ItemDto>>
{
}

public class GetPermisosQueryHandler : IRequestHandler<GetPermisosQuery, List<ItemDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;

    public GetPermisosQueryHandler(IIdentityService identityService, IApplicationDbContext context)
    {
        _identityService = identityService;
        _context = context;
    }

    public async Task<List<ItemDto>> Handle(GetPermisosQuery request, CancellationToken cancellationToken)
    {
        var entities = _context.CT_Permisos.OrderBy(x => x.Permiso).ToList();
        var res = entities.Select(x => new ItemDto() { id = x.PK_Permiso, Name = x.Permiso }).ToList();

        return res;
    }
}