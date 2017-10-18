using Newtonsoft.Json;

namespace TelegramBot.Types
{
    public class InlineKeyboardButton: InlineKeyboard
    {
        public string text;
        public string url;
        public string callback_data;
        public string switch_inline_query;
        public string switch_inline_query_current_chat;
        public CallbackGame callback_game;
        public bool pay;
        public override string GetJson()
        {
           return JsonConvert.SerializeObject(this);
        }
    }
    


}