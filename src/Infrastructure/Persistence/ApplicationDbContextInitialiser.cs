using System;
using System.ComponentModel;
using System.Reflection;
using ProjectAPI.Application.Common.Interfaces;
using ProjectAPI.Domain.Entities;
using ProjectAPI.Domain.Enums;
using ProjectAPI.Infrastructure.Common;
using ProjectAPI.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace ProjectAPI.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMediator _mediator;
    private readonly IDateTime _dateTime;

    public ApplicationDbContextInitialiser(IMediator mediator, IDateTime dateTime, ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _mediator = mediator;
        _dateTime = dateTime;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        //Create Roles
        if (!_roleManager.Roles.Any())
        {
            List<ApplicationRole> roles = new()
            {
                new ApplicationRole { Name = Roles.Master},
            };

            foreach (var role in roles)
            {
                await _roleManager.CreateAsync(role);
            }
        }

        //CREATE USER
        if (!_context.CT_Permisos.Any())
        {
            foreach (PermisosEnum permisoEnum in Enum.GetValues(typeof(PermisosEnum)))
            {
                var permiso = new CT_Permisos
                {
                    CreateDate = _dateTime.Now,
                    Permiso = GetEnumDescription(permisoEnum),
                    PK_Permiso = (int)permisoEnum
                };

                _context.Add(permiso);
            }
            _ = await _context.SaveChangesAsync();


        }


       
        //CREATE USER
        if (!_userManager.Users.Any())
        {
            const string password = "xZiYfns8799EXu7F3Zu9";
            ApplicationUser master = new() { CreateDate = _dateTime.Now, UserName = "master@proyectosApi.com.mx", Email = "master@proyectosAPI.com.mx", Nombre = "Master" };

            var res = await _userManager.CreateAsync(master, password);
            if (res.Succeeded)
                GuardarPermisos(master);
            
            _ = await _context.SaveChangesAsync();
            var r = await _context.ExecuteSqlCommandAsync("SELECT * FROM [dbo].[AspNetUserRoles]");
            if(r != null)
            _ = await _context.ExecuteSqlCommandAsync("INSERT INTO [dbo].[AspNetUserRoles] ( UserId,RoleId ) SELECT  (SELECT top 1 UserId  FROM AspNetUsers) as UserId, (SELECT top 1 Id  FROM AspNetRoles) as RoleId");

        }


    }

    private async Task GuardarPermisos(ApplicationUser user)
    {
        var permisos = _context.CT_Permisos.ToList();
      
            await _userManager.AddToRoleAsync(user, Roles.Master);
        _ = await _context.SaveChangesAsync();

        var allPermisos = (permisos.Where(x => x.PK_Permiso == (int)PermisosEnum.UpdateProject)
                .Select(a => new CT_PermisosUsuario
                {
                    FK_Permiso = a.PK_Permiso,
                    FK_User = user.Id
                }).ToList());

            _context.AddRange(allPermisos);
            _ = await _context.SaveChangesAsync();
    }

    private static string GetEnumDescription<T>(T value) where T : Enum
    {
        FieldInfo field = value.GetType().GetField(value.ToString());

        if (field == null)
            return null;

        DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

        return attribute == null ? value.ToString() : attribute.Description;
    }
}