using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.Application.Common.Exceptions;
using ProjectAPI.Application.Common.Interfaces;
using ProjectAPI.Application.Common.Models;
using ProjectAPI.Application.Role.Models;
using ProjectAPI.Application.Security.Commands;
using ProjectAPI.Application.Security.Models;
using ProjectAPI.Domain.Enums;

namespace ProjectAPI.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IApplicationDbContext _context;
    private readonly IDateTime _dateTime;
    private readonly IMapper _mapper;

    public IdentityService(IApplicationDbContext context, IDateTime dateTime, IMapper mapper, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _dateTime = dateTime;
    }

    public async Task<UsuarioWebModel> GetUsuarioWebModel(string username)
    {
        ApplicationUser user = _userManager.Users.Include(u => u.PermisosUsuario).FirstOrDefault(u => u.NormalizedUserName == username.ToUpper()) ?? throw new ValidationException("Usuario no encontrado");
        return new UsuarioWebModel
        {
            CreateDate = user.CreateDate,
            UserId = user.Id,
            Nombre = user.Nombre,
            Email = user.UserName,
            Role = (await _userManager.GetRolesAsync(user)).First(),
            Permisos = user.PermisosUsuario.ToList()
        };
    }

    public string GetUserNameAsync(string userId)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
        return user == null ? throw new ValidationException("Usuario no encontrado") : user.UserName;
    }

    public Dictionary<string, string> GetUsersNameAliasAsync()
    {
        var userDictionary = _userManager.Users
        .ToDictionary(user => user.Id, user => user.Nombre);

        return userDictionary;
    }

    public string GetNameByUsername(string username)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.NormalizedUserName == username.ToUpper());
        if (user == null)
            throw new ValidationException("Usuario no encontrado");
        return user.Nombre;
    }

    public async Task<List<RoleDto>> GetRolesAsync()
    {
        int count = _roleManager.Roles.Count();
        if (count < 1)
            throw new ValidationException("No hay roles");

        var roles = _roleManager.Roles.OrderBy(x => x.Name)
                       .Select(r => new RoleDto
                       {
                           id = r.Id,
                           Name = r.Name,
                       })
                       .ToList();
        return roles;
    }

    public async Task<PaginatedList<ApplicationUserDto>> GetUsersAsync(int pagezise, int pagenumber)
    {
        int count = _userManager.Users.Where(x => x.UserName != "master@multas.com.mx" && x.UserName != "robot_multas" && x.UserName != "Master@MFS.com" && x.IsActive == true).Count();
        if (count < 1)
            throw new ValidationException("No hay usuarios");

        var users = _userManager.Users.Include(u => u.UserRoles).Include(u => u.PermisosUsuario).Where(x => x.UserName != "master@multas.com.mx" && x.UserName != "robot_multas" && x.UserName != "Master@MFS.com" && x.IsActive == true)
                       .Select(u => new ApplicationUserDto
                       {
                           UserId = u.Id,
                           Role = u.UserRoles.First().Role.Name,
                           Email = u.Email,
                           Username = u.UserName,
                           Name = u.Nombre,
                           IsActive = u.IsActive,
                           Permisos = u.PermisosUsuario.ToList()
                       })
                       .Skip(pagezise * pagenumber)
                       .Take(pagezise)
                       .ToList();

        PaginatedList<ApplicationUserDto> res = new(users, count, pagenumber, pagezise);
        return res;
    }

    public async Task<ApplicationUserDto> CheckPasswordAsync(string userName, string password)
    {
        var userApp = await _userManager.FindByNameAsync(userName) ?? throw new ValidationException("Usuario no encontrado");

        if (!userApp.IsActive)
        {
            throw new ValidationException("El usuario está bloqueado");
        }

        // Verificar si la contraseña es correcta
        bool isPasswordCorrect = await _userManager.CheckPasswordAsync(userApp, password);
        if (!isPasswordCorrect)
        {
            throw new ConflictException("Usuario o contraseña incorrectos");
        }

        // Crear y devolver el DTO de usuario si todas las validaciones pasan
        return new ApplicationUserDto
        {
            Email = userApp.Email,
            Name = userApp.Nombre,
            UserId = userApp.Id,
            Username = userApp.UserName
        };
    }

    public async Task<IList<string>> GetRolesByUserAsync(string userName)
    {
        var userApp = await _userManager.FindByNameAsync(userName) ?? throw new ValidationException("Usuario no encontrado");
        var roles = await _userManager.GetRolesAsync(userApp);

        return roles;
    }

    public async Task DeleteUserLogic(string? userId)
    {
        var user = await _userManager.FindByIdAsync(userId) ?? throw new ValidationException("Usuario no encontrado");
        if (!user.IsActive)
            throw new ValidationException("Usuario previamente borrado");
        user.IsActive = false;
        await _userManager.UpdateAsync(user);
    }

    public async Task<bool> ValidateUserToken(string? userId)
    {
        var user = (await _userManager.FindByIdAsync(userId));
        return user != null && user.IsActive;
    }

    public async Task<bool> ChangeNameUser(string userId, string? name)
    {
        var user = (await _userManager.FindByIdAsync(userId));
        user.Nombre = name ?? user.Nombre;
        await _userManager.UpdateAsync(user);
        return true;
    }

    public async Task<bool> TienePermiso(string userId, int permiso)
    {
        return await _context.CT_PermisosUsuario.AnyAsync(x => x.FK_Permiso == permiso && x.FK_User == userId);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        PermisosEnum permissionEnumValue;
        int permisoId = 0;
        if (Enum.TryParse(policyName, out permissionEnumValue))
        {
            permisoId = (int)permissionEnumValue;
        }
        return await _context.CT_PermisosUsuario.AnyAsync(x => x.FK_Permiso == permisoId && x.FK_User == userId);
    }

    public async Task<bool> UsernameExist(string? username)
    {
        var x = (await _userManager.FindByEmailAsync(username));
        return x?.IsActive ?? false;
    }

    public async Task<bool> IsValidPassword(string password)
    {
        var validators = _userManager.PasswordValidators;
        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(_userManager, null, password);

            if (!result.Succeeded)
            {
                return false;
            }
        }
        return true;
    }

    public async Task<string> CreateWebUser(NewUserWebCommand user)
    {
        var newUser = new ApplicationUser { CreateDate = _dateTime.Now, UserName = user.Email, Email = user.Email, Nombre = user.Nombre };
        var res = await _userManager.CreateAsync(newUser, user.Password);
        if (res.Succeeded)
            await _userManager.AddToRoleAsync(newUser, user.Role);
        else
            throw new ValidationException(res.Errors.First().Description);
        return newUser.Id;
    }

    public async Task DeleteUserByUserId(string userId)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
        if (user != null)
            await _userManager.DeleteAsync(user);
    }

    public async Task<string> GenerateResetToken(string username, bool validateDelete = false)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(username) ?? throw new ValidationException("Usuario no encontrado");
            if (validateDelete && user.IsActive)
                throw new ValidationException("Usuario no encontrado");
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IdentityResult> ResetPassword(string username, string password, string token)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(username) ?? throw new ValidationException("Usuario no encontrado");
            IdentityResult res = await _userManager.ResetPasswordAsync(user, token, password);
            return res;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId) ?? throw new ValidationException("Usuario no encontrado");
           var r =  await _userManager.IsInRoleAsync(user, role);
        return r;
    }

    public async Task<string> ReactiveUserLogic(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
            return "";
        if (user.IsActive)
            throw new ValidationException("Nombre de usuario existente");
        user.IsActive = true;
        await _userManager.UpdateAsync(user);

        return user.Id;
    }
}