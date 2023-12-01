using Microsis.CWM.Common;
using System.Text.Json.Serialization;

namespace Microsis.CWM.Dto.Basic
{

    public class ResponseBase<T> where T : class
    {
        public ResponseBase(bool operationResult, string message, List<Error> error)
        {
            OperationResult = operationResult;
            Message = message;
            Error = error;
            
        }

        [JsonConstructor]
        public ResponseBase(T data, string message = null, int totalRecords = 0, int currentIndex = 0, int recordsPerPage = 0)
        {

            OperationResult = true;

            if (string.IsNullOrEmpty(message))
                Message = "عملیات با موفقیت انجام شد.";
            else
                Message = message;

            Data = data;

            Error = new List<Error>();
            Error.Add(new Error() { ErrorCode = (int)NErrorCode.Ok });

            TotalRecords = totalRecords;
            CurrentIndex = currentIndex;
            RecordsPerPage = recordsPerPage;
        }

        public ResponseBase()
        {

        }
        
        [JsonPropertyName("operationResult")]
        public bool OperationResult { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("error")]
        public List<Error> Error { get; set; }

        [JsonPropertyName("totalrecords")]
        public int TotalRecords { get; set; }

        [JsonPropertyName("currentindex")]
        public int CurrentIndex { get; set; }

        [JsonPropertyName("recordsperpage")]
        public int RecordsPerPage { get; set; }

    }

    public class Error
    {
        public int ErrorCode { get; set; }
        public string? ErrorDesc { get; set; }

    }
}
