using System;
using System.Text.RegularExpressions;

namespace TelegramBot.Utils
{
    public class ChatId
    {
        public string Id { get; }
        public ChatId(long id)
        {
            Id = id.ToString();
        }
        public ChatId (string username)
        {
            var regex = new Regex ("^@");
            if (regex.IsMatch(username))
            {
                Id = username;
            }
            else throw new ArgumentException("String should be username!");


        }

    }
}
