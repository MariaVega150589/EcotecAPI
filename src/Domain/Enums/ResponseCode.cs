namespace ProjectAPI.Domain.Enums;

public static class ResponseCode
{
    public static string Success { get; } = "0000";
    public static string Fail { get; } = "9500";
    public static string BadRequest { get; } = "9400";
    public static string NotFound { get; } = "9404";
}