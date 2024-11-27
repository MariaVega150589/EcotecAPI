namespace ProjectAPI.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime CreateDate { get; set; }
    public DateTime? LastUpdateDate { get; set; }
    public string? UpdatedByUserId { get; set; }
    public string? CreatedBy { get; set; }
    public bool IsActive { get; set; }
}