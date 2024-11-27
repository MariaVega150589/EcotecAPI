using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ProjectAPI.Domain.Entities;

namespace ProjectAPI.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DatabaseFacade Database { get; }

    Task<int> ExecuteSqlCommandAsync(string sql);

    Task<List<T>> ExecuteSelectQueryAsync<T>(string sql, SqlParameter[] parameters);

    //Task<int> ExecuteSqlCommand2(FormattableString sql);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    DbSet<CT_PermisosUsuario> CT_PermisosUsuario { get; }
    DbSet<CT_Permisos> CT_Permisos { get; }
    DbSet<CT_RolPermiso> CT_RolPermiso { get; }
    DbSet<CT_Configuraciones> CT_Configuraciones { get; }
    DbSet<Tbl_Project> CT_Proyectos { get; }
}