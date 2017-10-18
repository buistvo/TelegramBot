using System;
using TelegramBot.Utils;
namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            // fake token, insert your bot's token here
            var token = "123456789:ABCDFGHJKQWERTYUIOPLKJGFGGGRT56HF3G";
            try
            {
                var bot = new TelegramBot.TelegramBot(token);
                bot.OnMessage += async (sender, e) =>
                {
                    if(e.Update.channel_post!=null)
                    {
                        var chatId = new ChatId(e.Update.channel_post.chat.id);
                        await bot.SendMessageAsync(chatId, e.Update.channel_post.text);
                    }
                    if (e.Update.message!=null)
                    {
                        var chatId = new ChatId(e.Update.message.chat.id);
                        var ss = await bot.SendMessageAsync(chatId, e.Update.message.text);
                    }

                };
                bot.StartReceiving();
                Console.ReadLine();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}