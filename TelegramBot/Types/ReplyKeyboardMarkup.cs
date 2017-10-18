using Newtonsoft.Json;

namespace TelegramBot.Types
{
    public class ReplyKeyboardMarkup:InlineKeyboard
    {
        public KeyboardButton[][] keyboard;
        public bool resize_keyboard;
        public bool one_time_keyboard;
        public bool selective;
        public override string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}