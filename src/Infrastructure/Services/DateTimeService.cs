using ProjectAPI.Application.Common.Interfaces;

namespace ProjectAPI.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    private TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
    public DateTime Now => TimeZoneInfo.ConvertTime(DateTime.Now, cstZone);
}