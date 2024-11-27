using Microsoft.AspNetCore.Identity;
using ProjectAPI.Application.Common.Models;
using ProjectAPI.Application.Role.Models;
using ProjectAPI.Application.Security.Commands;
using ProjectAPI.Application.Security.Models;

namespace ProjectAPI.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string> GenerateResetToken(string userId, bool validateDelete = false);

    Task<string> CreateWebUser(NewUserWebCommand user);

    Task DeleteUserByUserId(string userId);

    Task<UsuarioWebModel> GetUsuarioWebModel(string username);

    Task DeleteUserLogic(string? userId);

    Task<string> ReactiveUserLogic(string userName);

    string GetUserNameAsync(string userId);

    Dictionary<string, string> GetUsersNameAliasAsync();

    string GetNameByUsername(string username);

    Task<ApplicationUserDto> CheckPasswordAsync(string userName, string password);

    Task<IList<string>> GetRolesByUserAsync(string userName);

    Task<bool> ValidateUserToken(string? username);

    Task<bool> UsernameExist(string? username);

    Task<bool> IsValidPassword(string password);

    Task<IdentityResult> ResetPassword(string username, string password, string token);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<PaginatedList<ApplicationUserDto>> GetUsersAsync(int pagezise, int pagenumber);

    Task<List<RoleDto>> GetRolesAsync();

    Task<bool> ChangeNameUser(string userId, string? name);

    Task<bool> TienePermiso(string userId, int permissionId);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    ////Task<(Result Result, UserDto User)> CreateUserAsync(CreateUserCommand command);
    ////Task<(Result Result, UserDto User)> UpdateUserAsync(UpdateUserCommand command);
    //Task<bool> RoleExist(string roleName);

    //Task<(string? username, string? userId, string? name)> FindByNameAsync(string userName);
    ////Task<List<UserDto>> GetUsers();
    ////Task<UserDto> GetUser(string username);
    //IQueryable<IdentityRole> GetRoles();
}