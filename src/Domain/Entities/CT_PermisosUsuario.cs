using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAPI.Domain.Entities;

public class CT_PermisosUsuario : BaseAuditableEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int PK_PermisoUsuario { get; set; }

    public string FK_User { get; set; }
    public int FK_Permiso { get; set; }

    [ForeignKey("FK_Permiso")]
    public virtual CT_Permisos Permiso { get; set; }
}