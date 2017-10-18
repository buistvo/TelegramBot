namespace TelegramBot.Types
{
    public class Message
    {
        public long message_id;
        public User from;
        public int date;
        public Chat chat;
        public User forward_from;
        public Chat forward_from_chat;
        public long forward_from_message_id;
        public string forward_signature;
        public int forward_date;
        public Message reply_to_message;
        public long edit_date;
        public string author_signature;
        public string text;
        public MessageEntity[] entities;
        public Audio audio;
        public Document document;
        public Game game;
        public PhotoSize[] photo;
        public Sticker sticker;
        public Video video;
        public Voice voice;
        public VideoNote video_note;
        public User[] new_chat_members;
        public string caption;
        public Contact contact;
        public Location location;
        public Venue venue;
        public User new_chat_member;
        public User left_chat_member;
        public string new_chat_title;
        public PhotoSize[] new_chat_photo;
        public bool delete_chat_photo;
        public bool group_chat_created;
        public bool supergroup_chat_created;
        public bool channed_chat_created;
        public long migrate_to_chat_id;
        public long migrate_from_chat_id;
        public Message pinned_message;
        public Invoice invoice;
        public SuccessfulPayment successfulPayment;
    }
}