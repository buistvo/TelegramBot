namespace TelegramBot.Types
{
    public class ChatMember
    {
        User user;
        string status;
        long until_date;
        bool can_be_edited;
        bool can_change_info;
        bool can_post_messages;
        bool can_edit_messages;
        bool can_delete_messages;
        bool can_invite_users;
        bool can_restrict_members;
        bool can_pin_messages;
        bool can_promote_members;
        bool can_send_messages;
        bool can_send_media_messages;
        bool can_send_other_messages;
        bool can_add_web_page_previews;
    }
}
