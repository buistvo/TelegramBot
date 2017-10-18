using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TelegramBot;
using TelegramBot.Types;

namespace UnitTests
{
    public class FakeHttpClientText : HttpClient
    {
        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var x = request.Content.ReadAsStringAsync().Result;
            return Task.Run(() =>
            {
                var content = JsonConvert.DeserializeObject<Message>(request.Content.ReadAsStringAsync().Result);
                var responseMessage = new Response<Message>
                {
                    ok = true,
                    result = content
                };
                var stringResponse = JsonConvert.SerializeObject(responseMessage);
                var response = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(stringResponse)};
                return response;
            });
        }
    }
}