
namespace PaymentSystem.Shared.Extensions
{
    public static class TimeSpanExtension
    {
        public static string ToTurkishDuration(this int? totalSeconds)
        {
            if (!totalSeconds.HasValue || totalSeconds <= 0)
                return "—";

            return ((TimeSpan)TimeSpan.FromSeconds(totalSeconds.Value)).ToTurkishDuration();
        }

        public static string ToTurkishDuration(this TimeSpan ts)
        {
            if (ts <= TimeSpan.Zero)
                return "0 second";

            var parts = new List<string>();

            if (ts.Days > 0) parts.Add($"{ts.Days} day");
            if (ts.Hours > 0) parts.Add($"{ts.Hours} hour");
            if (ts.Minutes > 0) parts.Add($"{ts.Minutes} minute");
            if (ts.Seconds > 0 || parts.Count == 0) parts.Add($"{ts.Seconds} second");

            return string.Join(" ", parts);
        }

        public static string ToLiveTurkishDuration(this DateTime loginDate)
        {
            return (DateTime.UtcNow - loginDate).ToTurkishDuration();
        }
    }
}
