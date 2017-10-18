using Newtonsoft.Json;

namespace TelegramBot.Types
{
    public class ReplyKeyboardRemove:InlineKeyboard
    {
        public const bool remove_keyboard = true;
        public bool selective;
        public override string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}