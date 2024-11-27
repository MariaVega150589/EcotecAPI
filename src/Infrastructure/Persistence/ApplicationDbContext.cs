using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.Application.Common.Interfaces;
using ProjectAPI.Domain.Entities;
using ProjectAPI.Infrastructure.Common;
using ProjectAPI.Infrastructure.Identity;
using ProjectAPI.Infrastructure.Persistence.Interceptors;

namespace ProjectAPI.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser, ApplicationRole, string,
        IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>, IApplicationDbContext
{
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IMediator mediator,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
        : base(options)
    {
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    public DbSet<CT_PermisosUsuario> CT_PermisosUsuario => Set<CT_PermisosUsuario>();
    public DbSet<CT_Permisos> CT_Permisos => Set<CT_Permisos>();
    public DbSet<CT_RolPermiso> CT_RolPermiso => Set<CT_RolPermiso>();
    public DbSet<CT_Configuraciones> CT_Configuraciones => Set<CT_Configuraciones>();
    public DbSet<Tbl_Project> CT_Proyectos => Set<Tbl_Project>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }

        builder.Entity<ApplicationUser>(b =>
        {
            b.Property(p => p.Id)
            .HasColumnName("UserId");
            b.HasMany(user => user.PermisosUsuario)
            .WithOne()
            .HasForeignKey(th => th.FK_User);
            b.HasMany(e => e.UserRoles).WithOne(e => e.User).HasForeignKey(ur => ur.UserId).IsRequired();
            b.HasIndex(e => e.Nombre).IsUnique();
            b.HasIndex(e => e.Email).IsUnique();
            b.Property(p => p.IsActive)
             .HasDefaultValue(true);
            b.Property(p => p.IsDeleted)
             .HasDefaultValue(false);
        });

        builder.Entity<ApplicationRole>(b =>
        {
            b.HasMany(role => role.RolPermisos)
            .WithOne()
            .HasForeignKey(th => th.FK_Role);
            b.HasMany(e => e.UserRoles).WithOne(e => e.Role).HasForeignKey(ur => ur.RoleId).IsRequired();
        });
        builder.Entity<Tbl_Project>(b =>
        {
            b.Property(p => p.Id).ValueGeneratedOnAdd();
        });
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public async Task<int> ExecuteSqlCommandAsync(string sql)
    {
        return await base.Database.ExecuteSqlRawAsync(sql);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEvents(this);

        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<T>> ExecuteSelectQueryAsync<T>(string sql, SqlParameter[] parameters)
    {
        var result = new List<T>();

        using (var command = base.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = sql;
            command.Parameters.AddRange(parameters);
            base.Database.OpenConnection();

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var item = Activator.CreateInstance<T>();
                    var properties = typeof(T).GetProperties();

                    foreach (var property in properties)
                    {
                        var ordinal = reader.GetOrdinal(property.Name);
                        if (!reader.IsDBNull(ordinal))
                        {
                            var value = reader.GetValue(ordinal);
                            property.SetValue(item, value);
                        }
                    }

                    result.Add(item);
                }
            }
        }

        return result;
    }
}