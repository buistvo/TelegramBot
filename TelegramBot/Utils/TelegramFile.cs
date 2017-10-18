namespace TelegramBot.Utils
{
    public class TelegramFile
    {
        public object File { get; }
        public TelegramFileType Type { get; }
        public string FileName { get; }

        public TelegramFile(string fileId)
        {
            File = fileId;
            Type = TelegramFileType.String;
        }

        public TelegramFile(byte[] file, string fileName)
        {
            File = file;
            Type = TelegramFileType.Raw;
            FileName = fileName;
        }
    }
    
    public enum TelegramFileType
    {
        String,
        Raw
    }
}