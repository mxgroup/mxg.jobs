namespace Mxg.Jobs
{
    public static class CronHelper
    {
        public static string Minutes(int periodMinutes, int hourStartMinutes = 0)
        {
            return $"0 {hourStartMinutes}/{periodMinutes} * 1/1 * ? *";
        }

        public static string Seconds(int periodSeconds, int minuteStartSeconds = 0)
        {
            return $"{minuteStartSeconds}/{periodSeconds} * * 1/1 * ? *";
        }
    }
}
