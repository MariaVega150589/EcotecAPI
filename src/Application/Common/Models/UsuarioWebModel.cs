using ProjectAPI.Domain.Entities;

namespace ProjectAPI.Application.Common.Models;

public class UsuarioWebModel
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string Nombre { get; set; }
    public string Role { get; set; }

    public List<CT_PermisosUsuario> Permisos { get; set; }
    public DateTime CreateDate { get; set; }
}