namespace ProjectAPI.Infrastructure.Common;

public static class Roles
{
    public const string Master = "Master";
    public const string Usuario = "Usuario";
    public const string Robot = "Robot";

    public static bool CheckRoleExists(string roleName)
    {
        // Get all the public static fields in the Roles class.
        var fields = typeof(Roles).GetFields();

        foreach (var field in fields)
        {
            // Check if the field's value matches the roleName.
            if (field.GetValue(null) as string == roleName)
            {
                return true;
            }
        }

        return false;
    }
}