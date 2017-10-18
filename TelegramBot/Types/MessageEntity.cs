namespace TelegramBot.Types
{
    public class MessageEntity
    {
        public string type;
        public int offset;
        public int length;
        public string url;
        public User user;
    }
}