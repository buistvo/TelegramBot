namespace TelegramBot.Types
{
    public class InlineQuery
    {
        public string id;
        public User from;
        public Location location;
        public string query;
        public string offset;
    }
}