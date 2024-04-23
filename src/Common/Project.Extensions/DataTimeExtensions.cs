namespace Project.Extensions
{
    public static class DataTimeExtensions
    {
        private static readonly System.Globalization.CultureInfo CultureInfo = new System.Globalization.CultureInfo("tr-TR");

        public static DateTime AddWorkDay(this DateTime datetime, int day, bool includeSaturday)
        {
            while (day > 0)
            {
                datetime = datetime.AddDays(1);

                if ((includeSaturday && datetime.DayOfWeek == DayOfWeek.Saturday) || datetime.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                day--;
            }

            return datetime;
        }

        /// <summary>
        /// 2 Aralık Cuma
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToTurkishString(this DateTime datetime)
        {
            return $"{datetime.Day} {CultureInfo.DateTimeFormat.GetMonthName(datetime.Month)} {CultureInfo.DateTimeFormat.GetDayName(datetime.DayOfWeek)}";
        }

        public static DateTime ToLocalDateTime(this DateTime main)
        {
            return TimeZoneInfo.ConvertTime(main, TimeZoneInfo.Local, TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"));
        }

        public static DateTime ToDateTimeByZoneId(this DateTime main, string zoneId)
        {
            return TimeZoneInfo.ConvertTime(main, TimeZoneInfo.Local, TimeZoneInfo.FindSystemTimeZoneById(zoneId));
        }
        public static int TwoDateDifferenceInDay(this DateTime main, DateTime secondDate)
        {
            return (main - secondDate).Days;
        }
    }
}
