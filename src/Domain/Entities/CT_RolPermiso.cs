using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAPI.Domain.Entities;

public class CT_RolPermiso : BaseAuditableEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Key]
    public int PK_RolPermiso { get; set; }

    public string FK_Role { get; set; }
    public int FK_Permiso { get; set; }

    [ForeignKey("FK_Permiso")]
    public virtual CT_Permisos Permiso { get; set; }
}