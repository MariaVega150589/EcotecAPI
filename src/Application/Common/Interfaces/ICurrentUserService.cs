namespace ProjectAPI.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Name { get; }
    string? Username { get; }
    string? Email { get; }
}