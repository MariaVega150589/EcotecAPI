using MediatR;
using Microsoft.Extensions.Configuration;
using ProjectAPI.Application.Common.Interfaces;
using ProjectAPI.Application.Common.Security;
using ProjectAPI.Domain.Entities;
using ProjectAPI.Infrastructure.Common;

namespace ProjectAPI.Application.Security.Commands;

[Authorize(Roles = $"{Roles.Master},{Roles.Usuario}")]
public class NewUserWebCommand : IRequest<bool>
{
    public string Email { get; set; }
    public string Nombre { get; set; }
    public string? Role { get; set; }

    public string Password { get; set; }
    public List<int> Permisos { get; set; }
}

public class NewUserWebCommandHandler : IRequestHandler<NewUserWebCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public NewUserWebCommandHandler(IIdentityService identityService, IApplicationDbContext context, IConfiguration configuration)
    {
        _identityService = identityService;
        _context = context;
        _configuration = configuration;
    }

    public async Task<bool> Handle(NewUserWebCommand request, CancellationToken cancellationToken)
    {
        request.Role = Roles.Usuario;
        string userId = await _identityService.CreateWebUser(request);
        var tablaPermisos = _context.CT_Permisos.Where(tp => request.Permisos.Any(p => p == tp.PK_Permiso));
        var permisos = tablaPermisos.Select(p => new CT_PermisosUsuario
        {
            FK_Permiso = p.PK_Permiso,
            FK_User = userId
        });
        await _context.CT_PermisosUsuario.AddRangeAsync(permisos);
        await _context.SaveChangesAsync(cancellationToken);

        string token = await _identityService.GenerateResetToken(request.Email, false);
        string urlRecoveryPassword = $"{_configuration["WebDomain"]}/reset-password?Token={token}&Email={request.Email}";
        string HtmlBody = $"\r\n Genere una contraseña en la siguiente <a style=\"font-size: 1.5rem\" href=\"{urlRecoveryPassword}\">liga</a>.";
        return true;
    }
}