using ProjectAPI.Domain.Enums;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class RequiresPermissionAttribute : Attribute
{
    public int Permission { get; }

    public RequiresPermissionAttribute(PermisosEnum permissionName)
    {
        Permission = (int)permissionName;
    }
}