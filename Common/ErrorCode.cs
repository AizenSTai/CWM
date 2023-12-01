namespace Microsis.CWM.Common
{
    public class ErrorCode
    {
        public const string Ok = "عملیات موفق";
        public const string Nok = "عملیات نا موفق";        
        public const string Exc = "خطای سیستمی.";
        public const string SomethingWereWrong = "مشکلی رخ داده است ، مجدد تلاش کنید.";
        public const string RecordNotFound = "رکوردی یافت نشد.";
        public const string ObjectNotFound = "آبجکت پیدا نشد.";
        public const string EmptyData = "داده خالی. ";
        public static string ItterateWholesaleGuildNo = "شناسه صنف فروشگاه تکراری است.";
        public const string MultiRecordsFound = "چندین رکورد یافت شد.";
        public const string MaximumRecordsReached = "خطا به علت محمدودیت تعداد رکوردها";
        public const string TokenExpired = "لطفا دوباره لاگین نمایید";
    }

    // note : using from 5001 to 5499
    public enum NErrorCode
    {
        Exc = -1, // خطای سیستمی 
        Ok = 0, // موفق
        Nok = 1 ,
        SomethingWereWrong = 5001,
        RecordNotFound = 5002,
        FileNotFound = 5003,
        FileMaximumSize = 5004,
        InvalidFileLenght = 5005,
        EmptyData = 5006 ,
        ObjectNotFound = 5007, // مورد درخواستی پیدا نشد
        ItterateWholesaleGuildNo = 5008 ,
        MultiRecordsFound = 5009 ,
        MaximumRecordsReached = 5010 ,
        TokenExpired = 5011,
        

    }
}
