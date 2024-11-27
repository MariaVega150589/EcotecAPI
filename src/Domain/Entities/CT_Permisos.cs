using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAPI.Domain.Entities;

public class CT_Permisos : BaseAuditableEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Key]
    public int PK_Permiso { get; set; }

    [Column(TypeName = "VARCHAR")]
    [StringLength(100)]
    public string Permiso { get; set; }
}