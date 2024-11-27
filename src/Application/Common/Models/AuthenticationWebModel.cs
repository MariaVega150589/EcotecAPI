namespace ProjectAPI.Application.Common.Models;

public class AuthenticationWebModel
{
    public bool IsAuthenticated { get; set; }
    public string Jwt { get; set; }
    public string RefreshToken { get; set; }
    public string ExpDate { get; set; }
    public UsuarioWebModel User { get; set; }
}