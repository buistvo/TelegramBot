using TelegramBot.Types;

namespace TelegramBot
{
    public partial class TelegramBot
    {
        public class OnMessageEventArgs
        {
            public OnMessageEventArgs(Update u)
            {
                Update = u;
            }

            public Update Update { get;}
        }
    }
}