
using System.Text;
using Newtonsoft.Json;
using static Microsis.CWM.Util.LoggerProxy;

namespace Microsis.CWM.Util
{
    public interface IApiClient
    {
        Task<TOut> CallRestService<TOut, TInput>(TInput input, string url, HttpMethod httpMethod = null, List<KeyValuePair<string, string>> headerValues = null);
    }

    public class ApiClient : IApiClient
    {
        public async Task<TOut> CallRestService<TOut, TInput>(TInput input, string url, HttpMethod httpMethod, List<KeyValuePair<string, string>> headerValues = null)
        {
            var jsonInString = JsonConvert.SerializeObject(input);
            using var client = new HttpClient();
            try
            {
                if (httpMethod == null)
                {
                    httpMethod = HttpMethod.Get;
                }
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                var content = new StringContent(jsonInString, Encoding.UTF8, "application/json");
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = httpMethod,
                    RequestUri = new Uri(url),
                    Content = content,

                };
                if (headerValues != null && headerValues.Any())
                {
                    foreach (var headerValue in headerValues)
                    {
                        request.Headers.Add(headerValue.Key, headerValue.Value);
                    }
                }
                HttpResponseMessage result = await client.SendAsync(request);

                var returnData = await result.Content.ReadAsStringAsync();

                Log(LogLevels.Info, returnData);

                //var deserializeOptions = new JsonSerializerOptions();
                //deserializeOptions.Converters.Add(new JsonDateConverter());

                //var output = JsonSerializer.Deserialize<TOut>(returnData, deserializeOptions);
                var output = JsonConvert.DeserializeObject<TOut>(returnData);
                return output;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
                throw;
            }
        }
    }

    //public class JsonDateConverter : Newtonsoft.Json.JsonConverter<DateTime?>
    //{
    //    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    //    {
    //        var value = reader.GetString();
    //        if (!string.IsNullOrEmpty(value))
    //        {
    //            var dt = DateTime.ParseExact(value, "yyyy/MM/dd", CultureInfo.InvariantCulture);
    //            return dt;

    //        }
    //        return null;
    //    }

    //    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    //    {
    //        if (value.HasValue)
    //        {
    //            writer.WriteStringValue(value.Value.ToString(
    //                "yyyy/MM/dd", CultureInfo.InvariantCulture));
    //        }
    //    }
    //}
}
