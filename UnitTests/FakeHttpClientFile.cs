using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TelegramBot;
using TelegramBot.Types;
namespace UnitTests
{
    public class FakeHttpClientFile : HttpClient
    {
        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var respone = new HttpResponseMessage(HttpStatusCode.OK);
                respone.Content = new StringContent("{\"ok\":true,\"result\":{\"message_id\":169,\"from\":{\"id\":411255176,\"is_bot\":true,\"first_name\":\"My bot\",\"username\":\"test671bot\"},\"chat\":{\"id\":225803684,\"first_name\":\"Lev\",\"username\":\"buistvo671\",\"type\":\"private\"},\"date\":1505576872,\"photo\":[{\"file_id\":\"AgADBAADVGw5GzoaZAc6zr0FaeREmODy-RkABKhE7UDLvIn0SYEAAgI\",\"file_size\":1097,\"width\":90,\"height\":67},{\"file_id\":\"AgADBAADVGw5GzoaZAc6zr0FaeREmODy-RkABGraHKb12tr7SIEAAgI\",\"file_size\":1795,\"width\":120,\"height\":90}]}}");
                return respone;
            });
        }
    }
}