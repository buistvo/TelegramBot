namespace TelegramBot.Types
{
    public class Chat
    {
        public long id;
        public string type;
        public string title;
        public string username;
        public string first_name;
        public string last_name;
        public bool all_members_are_administrators;
        public ChatPhoto photo;
        public string description;
        public string invite_link;
        public Message pinnedMessage;
    }
}