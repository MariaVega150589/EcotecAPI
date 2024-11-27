using System.ComponentModel;

namespace ProjectAPI.Domain.Enums;

public enum PermisosEnum
{
    [Description("Dashboard")]
    Dashboard = 1,

    [Description("Project")]
    Project = 2,

    [Description("UpdateProject")]
    UpdateProject = 3,

    [Description("DeleteProject")]
    DeleteProject = 4,

}