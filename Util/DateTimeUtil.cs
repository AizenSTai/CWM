using System.Globalization;

namespace Microsis.CWM.Util
{
    static public class DateTimeUtil
    {
        public static string MiladiSysStandardDateNow(bool HasSeperation = true)
        {
            if(HasSeperation)
                return DateTime.Now.ToString("yyyy/MM/dd");
            else
                return DateTime.Now.ToString("yyyyMMdd");
        }
        public static string MiladiSysStandardDateTimeNow(bool HasSeperation = true)
        {
            return DateTime.Now.ToString("yyyy/MM/ddTHH:mm:ss");
        }
        public static string SysStandardTimeNow(bool HasSeperation = true)
        {
            if (HasSeperation)
                return DateTime.Now.ToString("HH:mm:ss");
            else
                return DateTime.Now.ToString("HHmmss");
        }
        public static string ShamsiSysStandardDateNow(bool HasSeperation = true)
        {
            PersianCalendar pc = new PersianCalendar();
            string pd = $"{pc.GetYear(DateTime.Now).ToString()}/{pc.GetMonth(DateTime.Now).ToString().PadLeft(2, '0')}/{pc.GetDayOfMonth(DateTime.Now).ToString().PadLeft(2, '0')}";
            if (HasSeperation)
                return pd;
            else
                return pd.Replace("/", String.Empty);

        }
        public static string ShamsiSysStandardDateTimeNow(bool HasSeperation = true)
        {
            PersianCalendar pc = new PersianCalendar();
            string pd = $"{pc.GetYear(DateTime.Now)}/{pc.GetMonth(DateTime.Now).ToString().PadLeft(2, '0')}/{pc.GetDayOfMonth(DateTime.Now).ToString().PadLeft(2, '0')}/{DateTime.Now.ToString("HH:mm")}";
            if (HasSeperation)
                return pd;
            else
                return pd.Replace("/", String.Empty);

        }
        public static string ShamsiSysStandardDateYesterday(bool HasSeperation = true)
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime yesterday = DateTime.Now.AddDays(-1);
            string pd = $"{pc.GetYear(yesterday).ToString()}/{pc.GetMonth(yesterday).ToString().PadLeft(2, '0')}/{pc.GetDayOfMonth(yesterday).ToString().PadLeft(2, '0')}";
            return pd;
        }
        public static string MiladiToShamsi(string MiladiDate)
        {
            DateTime d = DateTime.Parse(MiladiDate);
            PersianCalendar pc = new PersianCalendar();
            return string.Format("{0}/{1}/{2}", pc.GetYear(d), pc.GetMonth(d).ToString().PadLeft(2, '0'), pc.GetDayOfMonth(d).ToString().PadLeft(2, '0'));
        }
        public static string ShamsiToMiladi(string ShamsiDate)
        {
            PersianCalendar pc = new PersianCalendar();
            string[] sd = ShamsiDate.Split('/');
            DateTime dt = new DateTime(int.Parse(sd[0]), int.Parse(sd[1]), int.Parse(sd[2]), pc);
            return dt.ToString("yyyy/MM/dd");
        }
        public static string GetDay(string ShamsiDate)
        {
            var miladi = ShamsiToMiladi(ShamsiDate).Split("/");

            DateTime dt = new DateTime(int.Parse(miladi[0]), int.Parse(miladi[1]), int.Parse(miladi[2]));

            var dd = DayOfWeekPersian(dt.DayOfWeek.ToString());

            return dd;
        }
        public static string DayOfWeekPersian(string DayOfWeek)
        {
            switch (DayOfWeek.ToLower())
            {
                case "saturday":
                    return "شنبه";
                    break;
                case "sunday":
                    return "یکشنبه";
                    break;
                case "monday":
                    return "دوشنبه";
                    break;
                case "tuesday":
                    return "سه شنبه";
                    break;
                case "wednesday":
                    return "چهارشنبه";
                    break;
                case "thursday":
                    return "پنج شنبه";
                    break;
                case "friday":
                    return "جمعه";
                    break;
                default:
                    return "مشخص نشده";
                    break;
            }
        }
        public static int[] ShamsiToMiladiInt(string ShamsiDate)
        {
            int[] res = new int[2];
            PersianCalendar pc = new PersianCalendar();
            string[] sd = ShamsiDate.Split('/');
            DateTime dt = new DateTime(int.Parse(sd[0]), int.Parse(sd[1]), int.Parse(sd[2]), pc);

            string s = ShamsiDate.Replace("/", string.Empty);
            res[0] = int.Parse(s); // آیتم اول تاریخ شمسی
            res[1] = int.Parse(dt.ToString("yyyyMMdd")); // آیتم دوم تاریخ میلادی
            return res;
        }
        public static string[] ThisWeekScope()
        {
            string[] res = new string[2];

            DateTime baseDate = DateTime.Today;
            var start = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            res[0] = start.ToString("yyyy/MM/dd");
            res[1] = start.AddDays(7).AddSeconds(-1).ToString("yyyy/MM/dd");

            return res;
        }
        public static string[] ThisWeekScopeShamsi()
        {
            string[] res = new string[2];

            DateTime baseDate = DateTime.Today;
            var start = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            res[0] = MiladiToShamsi(start.ToString("yyyy/MM/dd"));
            res[1] = MiladiToShamsi(start.AddDays(7).AddSeconds(-1).ToString("yyyy/MM/dd"));

            return res;
        }
        public static int[] ThisWeekScopeShamsiInt()
        {
            int[] res = new int[2];

            DateTime baseDate = DateTime.Today;
            var start = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            res[0] = int.Parse(MiladiToShamsi(start.ToString("yyyy/MM/dd")).Replace("/", string.Empty));
            res[1] = int.Parse(MiladiToShamsi(start.AddDays(7).AddSeconds(-1).ToString("yyyy/MM/dd")).Replace("/", string.Empty));

            return res;
        }

        public static string DateTimeToDashFormat(string DateTime)
        {
            try
            {
                if (string.IsNullOrEmpty(DateTime))
                    return System.DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss");

                if (DateTime.Length == 19 && DateTime.Contains("/") && DateTime.Contains(":"))
                    return DateTime;

                if (DateTime.Contains("-") && DateTime.Length == 15)
                {
                    string date = DateTime.Split('-')[0];
                    string time = DateTime.Split('-')[1];
                    return $"{date[0]}{date[1]}{date[2]}{date[3]}/{date[4]}{date[5]}/{date[6]}{date[7]}-{time[0]}{time[1]}:{time[2]}{time[3]}:{time[4]}{time[5]}";
                }
                return DateTime;
            }
            catch (Exception ex)
            {
                return System.DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss");
            }

        }
        public static string DateToDashFormat(int Date)
        {
            try
            {
                string date = Date.ToString();

                if (date.Length == 8)
                {
                    return $"{date[0]}{date[1]}{date[2]}{date[3]}/{date[4]}{date[5]}/{date[6]}{date[7]}";
                }
                return date;
            }
            catch (Exception ex)
            {
                return System.DateTime.Now.ToString("yyyy/MM/dd");
            }

        }
        public static string TimeToDotFormat(int Time)
        {
            try
            {
                string time = Time.ToString();

                if (time.Length == 6)
                {
                    return $"{time[0]}{time[1]}:{time[2]}{time[3]}:{time[4]}{time[5]}";
                }
                if (time.Length == 5)
                {
                    return $"0{time[1]}:{time[2]}{time[3]}:{time[4]}{time[5]}";
                }
                return time;
            }
            catch (Exception ex)
            {
                return System.DateTime.Now.ToString("HH:mm:ss");
            }

        }
        public static int DateTimeToInt(string DateTime)
        {
            if (string.IsNullOrEmpty(DateTime))
                return 0;

            var date = string.Empty;
            if (DateTime.Contains("/"))
                date = DateTime.Substring(0, 10);
            else
                date = DateTime.Substring(0, 8);


            var res = date.Replace("/", string.Empty);
            int ires = 1;
            int.TryParse(res, out ires);
            return ires;

        }
        public static string GetDate()
        {
            return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') +
                   DateTime.Now.Day.ToString().PadLeft(2, '0');
        }
        public static string GetTime()
        {
            return DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') +
                   DateTime.Now.Second.ToString().PadLeft(2, '0');
        }
        public static long GetEpochNow()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }
        public static long GetEpochNowFromShamsi(string ShamsiDate)
        {
            var MiladiDate = ShamsiToMiladi(ShamsiDate);

            var miladiDate = MiladiDate.Split('/');

            var dateTime = new DateTime(int.Parse(miladiDate[0]), int.Parse(miladiDate[1]), int.Parse(miladiDate[2]), 0, 0, 0, DateTimeKind.Utc);

            var secs = (long)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

            return secs;
        }
        public static long GetEpochNowFromMiladi(string MiladiDate)
        {
            var miladiDate = MiladiDate.Split('/');

            var dateTime = new DateTime(int.Parse(miladiDate[0]), int.Parse(miladiDate[1]), int.Parse(miladiDate[2]), 0, 0, 0, DateTimeKind.Utc);

            var secs = (long)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

            return secs;
        }


    }
}
