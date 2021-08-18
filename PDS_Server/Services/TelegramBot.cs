using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace PDS_Server.Services
{
    public interface IBot
    {
        Task<Message> SendMessage(string message, BuiltInChatId chat);
    }

    public enum BuiltInChatId
    {
        Channel_Errors
    }

    public class TelegramBot : IBot
    {
        private TelegramBotClient Client;
        private Dictionary<BuiltInChatId, ChatId> ChatCollection = new Dictionary<BuiltInChatId, ChatId>();
        public TelegramBot()
        {
            Client = new TelegramBotClient("1897793531:AAH4uAtf6UVHl3WruSZOgrd4LjbMHX_L86A");
            ChatCollection.Add(BuiltInChatId.Channel_Errors, new ChatId(-1001317262375));
        }
        public async Task<Message> SendMessage(string message, BuiltInChatId chat = BuiltInChatId.Channel_Errors)
        {
            if (ChatCollection.TryGetValue(chat, out ChatId id))
            { 
                return await Client.SendTextMessageAsync(id, message); 
            }
            return null;
        }
    }
}
