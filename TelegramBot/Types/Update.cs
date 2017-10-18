namespace TelegramBot.Types
{
    public class Update
    {

        public long update_id;
        public Message message;
        public Message edited_message;
        public Message channel_post;
        public Message edited_channel_post;
        public InlineQuery inline_query;
    }
}