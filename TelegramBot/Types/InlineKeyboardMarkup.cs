using Newtonsoft.Json;

namespace TelegramBot.Types
{
    public class InlineKeyboardMarkup : InlineKeyboard
    {
        public InlineKeyboardMarkup[][] inline_keyboard;

        public override string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
