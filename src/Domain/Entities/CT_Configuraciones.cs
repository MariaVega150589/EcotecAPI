using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAPI.Domain.Entities;
public class CT_Configuraciones : BaseAuditableEntity
{
    [DatabaseGenerated(  DatabaseGeneratedOption.None)]
    [Key]
    public int PK_Configuracion { get; set; }
    public string Valor { get; set; }


    [Column(TypeName = "VARCHAR")]
    [StringLength(100)]
    public string Descripcion { get; set; }
}