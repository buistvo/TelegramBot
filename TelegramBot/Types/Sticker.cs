using System;

namespace TelegramBot.Types
{
    public class Sticker
    {
        public string file_id;
        public int width;
        public int height;
        public PhotoSize thumb;
        public string emoji;
        public String set_name;
        public MaskPosition mask_position;
        public int file_size;
    }
}