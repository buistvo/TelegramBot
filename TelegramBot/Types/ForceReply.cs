using Newtonsoft.Json;

namespace TelegramBot.Types
{
    public class ForceReply:InlineKeyboard
    {
        public const bool force_reply = true;
        public bool selective;
        public override string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}