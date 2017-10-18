using System;
using System.Net.Http;
using TelegramBot.Utils;
using Xunit;

namespace UnitTests
{

    public class UnitTest1
    {
        private const string Token = "123456789:ABCDFGHJKQWERTYUIOPLKJGFGGGRT56HF3G";

        [Fact]
        public void TelegramBot_SendDocumentAsync_ReturnsFileMessage()
        {
            var fakeClient = FakeHttpClientFactory.GetClient(ClientType.File);
            var bot = new TelegramBot.TelegramBot(Token, fakeClient);
            var message = bot.SendDocumentAsync(new ChatId(12345), new TelegramFile("fileId")).Result;
            Assert.True(message.ok);
        }

        [Fact]
        public void TelegramBot_SendVideoAsync_ReturnsFileMessage()
        {
            var fakeClient = FakeHttpClientFactory.GetClient(ClientType.File);
            var bot = new TelegramBot.TelegramBot(Token, fakeClient);
            var message = bot.SendVideoAsync(new ChatId(12345), new TelegramFile("fileId")).Result;
            Assert.True(message.ok);
        }

        [Fact]
        public void TelegramBot_SendMessageAsync_ResurnsTextMessage()
        {
            var fakeClient = FakeHttpClientFactory.GetClient(ClientType.Text);
            var bot = new TelegramBot.TelegramBot(Token, fakeClient);
            var message = bot.SendMessageAsync(new ChatId(12345), "HAI").Result;
            Assert.True(message.ok);
        }
    }
    public static class FakeHttpClientFactory
    {
        public static HttpClient GetClient(ClientType type)
        {
            if(type == ClientType.Text)
            return new FakeHttpClientText();
            if (type == ClientType.File)
                return new FakeHttpClientFile();
            return null;
        }
    }

    public enum ClientType
    {
        Text, 
        File
    }
}