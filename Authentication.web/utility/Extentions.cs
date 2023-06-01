using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text;

namespace Authentication.web.utility
{
    public static class Extentions
    {
        public static async Task<HttpResponseMessage> PostAsJsonAsync(this HttpClient client,string Api,object obj,bool IgnoreNulls)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                IgnoreNullValues = IgnoreNulls
            };
            var json = JsonSerializer.Serialize(obj, jsonSerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await client.PostAsync("/api/Question/AddQuestion",content);
            return httpResponseMessage;
        }
    }
}
