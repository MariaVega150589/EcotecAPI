using ProjectAPI.Application.BankResume.Models;
using ProjectAPI.Application.Common.Interfaces;
using ProjectAPI.Application.Common.Models;
using ProjectAPI.Application.Common.Security;
using ProjectAPI.Infrastructure.Common;
using MediatR;

namespace ProjectAPI.Application.CatalogosPredefinidos.Queries;

[Authorize(Roles = $"{Roles.Master},{Roles.Usuario}")]
public class GetCatalogosPredefinidosQuery : IRequest<CatalogosPredefinidosDto>
{
}

public class GetCatalogosPredefinidosQueryHandler : IRequestHandler<GetCatalogosPredefinidosQuery, CatalogosPredefinidosDto>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;

    public GetCatalogosPredefinidosQueryHandler(IIdentityService identityService, IApplicationDbContext context)
    {
        _identityService = identityService;
        _context = context;
    }

    public async Task<CatalogosPredefinidosDto> Handle(GetCatalogosPredefinidosQuery request, CancellationToken cancellationToken)
    {
        CatalogosPredefinidosDto res = new()
        {
            Permiso = _context.CT_Permisos.OrderBy(x => x.Permiso).Select(x => new ItemDto() { id = x.PK_Permiso, Name = x.Permiso }).ToList(),
        };

        return res;
    }
}