using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectAPI.Application.Common.Interfaces;
using ProjectAPI.Application.Common.Models;
using ProjectAPI.Application.Security.Models;

namespace ProjectAPI.Application.Security.Commands;

public class LoginCommand : IRequest<AuthenticationWebModel>
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string NotificationToken { get; set; }
    public string Plattaform { get; set; }
    public string SO { get; set; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationWebModel>
{
    private readonly IIdentityService _identityService;
    private readonly IConfiguration _config;

    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IDateTime _dateTime;

    public LoginCommandHandler(IApplicationDbContext context, IDateTime dateTime, IIdentityService identityService, IConfiguration config, IMapper mapper)
    {
        _identityService = identityService;
        _config = config;
        _context = context;
        _mapper = mapper;
        _dateTime = dateTime;
    }

    public async Task<AuthenticationWebModel> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        ApplicationUserDto user = await _identityService.CheckPasswordAsync($"{request.UserName}", request.Password);

        var result = await GenerateJWT(user);

        return result;
    }

    public async Task<AuthenticationWebModel> GenerateJWT(ApplicationUserDto user)
    {
        var roles = await _identityService.GetRolesByUserAsync(user.Username);
        // Generamos un token según los claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name,user.Name),
            new Claim("Username",user.Username),
            new Claim("UserId",user.UserId),
            new Claim(ClaimTypes.Email, user.Email)
        };
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: _dateTime.Now.AddMinutes(60 * 24 * 1),
            signingCredentials: credentials);
        var jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        var res = new AuthenticationWebModel
        {
            Jwt = jwt,
            IsAuthenticated = true,
            ExpDate = _dateTime.Now.AddMinutes(60 * 24 * 1).ToString("s"),
            RefreshToken = Guid.NewGuid().ToString(),
            User = await _identityService.GetUsuarioWebModel(user.Username)
        };
        return res;
    }
}