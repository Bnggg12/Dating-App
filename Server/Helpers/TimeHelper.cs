namespace Server.Helpers;
public static class TimeHelper
{
    public static DateTime NowVN()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
        );
    }
}