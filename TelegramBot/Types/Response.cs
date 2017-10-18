

// ReSharper disable InconsistentNaming

namespace TelegramBot.Types
{

    public class Response <T>
    {
        public bool ok;
        public T result;
    }
}