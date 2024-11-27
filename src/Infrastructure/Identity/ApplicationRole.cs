using Microsoft.AspNetCore.Identity;
using ProjectAPI.Domain.Entities;

namespace ProjectAPI.Infrastructure.Identity;

public class ApplicationRole : IdentityRole
{
    public virtual ICollection<CT_RolPermiso> RolPermisos { get; set; }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
}