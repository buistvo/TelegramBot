using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Linq;
using TelegramBot.Types;
using TelegramBot.Utils;
using File = TelegramBot.Types.File;

namespace TelegramBot
{
    public partial class TelegramBot
    { 
        private const string Address = "https://api.telegram.org/bot";
        readonly HttpClient _client;
        public readonly string Token;
        public bool IsReceiving { get; private set; }
        public long MessageOffset { get; private set; }

        public delegate void OnMessageEventHandler(object sender, OnMessageEventArgs e);

        public event OnMessageEventHandler OnMessage;

        public TelegramBot(string token)
        {
            Token = token;
            _client = new HttpClient();
        }

        public TelegramBot(string token, HttpClient client)
        {
            Token = token;
            _client = client;
        }

        private async Task<T> SendRequest<T>(Dictionary<string, object> input, string url)
        {
            try
            {
                if (input.Any(x => x.Value is TelegramFile))
                {
                    var form = new MultipartFormDataContent();
                    foreach (var pair in input)
                    {
                        if (pair.Value == null)
                        {
                            form.Add(new StringContent(""), pair.Key);
                        }
                        else if (pair.Value is TelegramFile)
                        {
                            var file = (TelegramFile) pair.Value;
                            if (file.Type == TelegramFileType.String)
                            {
                                form.Add(new StringContent((string) file.File), pair.Key);
                            }
                            if (file.Type == TelegramFileType.Raw)
                            {
                                form.Add(new StreamContent(new MemoryStream((byte[]) file.File)), pair.Key,
                                    file.FileName);
                            }
                        }
                        else if (pair.Value is int || pair.Value is long || pair.Value is bool)
                        {
                            form.Add(new StringContent(pair.Value.ToString()), pair.Key);
                        }
                        else if (pair.Value is string)
                        {
                            form.Add(new StringContent((string) pair.Value), pair.Key);
                        }
                        else
                            throw new ArgumentException("Value type" + pair.Value.GetType() +
                                                        " is not supported");
                    }

                    var response = await _client.PostAsync(url, form);
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    response.EnsureSuccessStatusCode();
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }
                else
                {
                    var json = JsonConvert.SerializeObject(input);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _client.PostAsync(url, content);
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    response.EnsureSuccessStatusCode();
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return default(T);
            }
        }

        public void StartReceiving(params string[] allowedUpdates)
        {
            if (IsReceiving) return;
            IsReceiving = true;
            Task.Run(async () =>
            {
                while (IsReceiving)
                {
                    var updates = await GetUpdatesAsync(MessageOffset, allowedUpdates: allowedUpdates);
                    if (updates.result.Length > 0)
                    {
                        MessageOffset = updates.result[updates.result.Length - 1].update_id + 1;
                        foreach (var update in updates.result)
                        {
                            OnMessage?.Invoke(this, new OnMessageEventArgs(update));
                        }
                    }
                    Task.Delay(1000).Wait();
                }
            });
        }

        #region TelegramMethods

        public async Task<string> GetMeAsync()
        {
            string result = "";
            var url = Address + Token + "/getMe";
            var request = (HttpWebRequest) WebRequest.Create(url);
            var response = (HttpWebResponse) request.GetResponse();

            var responseStream = response.GetResponseStream();
            if (responseStream == null) return result;
            using (var s = new StreamReader(responseStream))
            {
                await s.ReadToEndAsync().ContinueWith((x) => { result = x.Result; });
            }
            return result;
        }

        public async Task<Response<Message>> SendMessageAsync(ChatId chatId, string text, string parseMode = "",
            bool disableWebPagePreview = false, bool disableNotification = false, int replyToMessageId = 0,
            InlineKeyboard replyMarkup = null)
        {
            var markup = "";
            if (replyMarkup != null)
            {
                markup = replyMarkup.GetJson();
            }
            var url = Address + Token + "/sendMessage";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"text", text},
                {"parse_mode", parseMode},
                {"disable_web_page_preview", disableWebPagePreview},
                {"disable_notification", disableNotification},
                {"reply_to_message_id", replyToMessageId},
                {"reply_markup", markup},
            }, url);
        }

        public async Task<Response<Update[]>> GetUpdatesAsync(long offset = 0, int limit = 100, int timeout = 0,
            string[] allowedUpdates = null)
        {
            var url = Address + Token + "/getUpdates";
            return await SendRequest<Response<Update[]>>(new Dictionary<string, object>
            {
                {"offset", offset},
                {"limit", limit},
                {"timeout", timeout},
                {"allowed_updates", allowedUpdates},
            }, url);
        }


        public async Task<Response<Message>> SendPhotoAsync(ChatId chatId, TelegramFile photo, string caption = "",
            bool disableNotification = false,
            int replyToMessageId = 0, InlineKeyboard replyMarkup = null)
        {
            var markup = "";
            if (replyMarkup != null)
            {
                markup = replyMarkup.GetJson();
            }
            var url = Address + Token + "/sendPhoto";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"photo", photo},
                {"caption", caption},
                {"disable_notification", disableNotification},
                {"reply_to_message_id", replyToMessageId},
                {"reply_markup", markup},
            }, url);
        }

        public async Task<Response<Message>> SendVoiceAsync(ChatId chatId, TelegramFile audio, string caption = "",
            int duration = 0,
            bool disableNotification = false, int replyToMessageId = 0,
            InlineKeyboard replyMarkup = null)
        {
            var markup = "";
            if (replyMarkup != null)
            {
                markup = replyMarkup.GetJson();
            }
            var url = Address + Token + "/sendVoice";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"audio", audio},
                {"caption", caption},
                {"duration", duration},
                {"disable_notification", disableNotification},
                {"reply_to_message_id", replyToMessageId},
                {"reply_markup", markup},
            }, url);
        }

        public async Task<Response<Message>> SendAudioAsync(ChatId chatId, TelegramFile audio, string caption = "",
            int duration = 0,
            string performer = "", string title = "", bool disableNotification = false, int replyToMessageId = 0,
            InlineKeyboard replyMarkup = null)
        {
            var markup = "";
            if (replyMarkup != null)
            {
                markup = replyMarkup.GetJson();
            }
            var url = Address + Token + "/sendAudio";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"voice", audio},
                {"caption", caption},
                {"performer", performer},
                {"title", title},
                {"duration", duration},
                {"disable_notification", disableNotification},
                {"reply_to_message_id", replyToMessageId},
                {"reply_markup", markup},
            }, url);
        }

        public async Task<Response<Message>> SendDocumentAsync(ChatId chatId, TelegramFile document,
            string caption = "",
            bool disableNotification = false, int replyToMessageId = 0,
            InlineKeyboard replyMarkup = null)
        {
            var markup = "";
            if (replyMarkup != null)
            {
                markup = replyMarkup.GetJson();
            }
            var url = Address + Token + "/sendDocument";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"document", document},
                {"caption", caption},
                {"disable_notification", disableNotification},
                {"reply_to_message_id", replyToMessageId},
                {"reply_markup", markup},
            }, url);
        }

        public async Task<Response<Message>> SendVideoAsync(ChatId chatId, TelegramFile video, int duration = 0,
            int width = 0,
            int height = 0, string caption = "",
            bool disableNotification = false, int replyToMessageId = 0,
            InlineKeyboard replyMarkup = null)
        {
            var markup = "";
            if (replyMarkup != null)
            {
                markup = replyMarkup.GetJson();
            }
            var url = Address + Token + "/sendVideo";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"video", video},
                {"duration", duration},
                {"width", width},
                {"height", height},
                {"caption", caption},
                {"disable_notification", disableNotification},
                {"reply_to_message_id", replyToMessageId},
                {"reply_markup", markup},
            }, url);
        }

        public async Task<Response<Message>> SendVideoNoteAsync(ChatId chatId, TelegramFile videoNote, int duration = 0,
            int length = 0,
            bool disableNotification = false, int replyToMessageId = 0,
            InlineKeyboard replyMarkup = null)
        {
            var markup = "";
            if (replyMarkup != null)
            {
                markup = replyMarkup.GetJson();
            }
            var url = Address + Token + "/sendVideoNote";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"video", videoNote},
                {"duration", duration},
                {"disable_notification", disableNotification},
                {"reply_to_message_id", replyToMessageId},
                {"reply_markup", markup},
            }, url);
        }

        public async Task<Response<Message>> SendLocationAsync(ChatId chatId, float latitude, float longitude,
            bool disableNotification = false, int replyToMessageId = 0,
            InlineKeyboard replyMarkup = null)
        {
            var markup = "";
            if (replyMarkup != null)
            {
                markup = replyMarkup.GetJson();
            }
            var url = Address + Token + "/sendLocation";
            if (latitude < -90 || latitude > 90) throw new ArgumentException("latitude must be between -90 and 90");
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("longitude must be between -180 and 180");

            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"latitude", latitude},
                {"longitude", longitude},
                {"disable_notification", disableNotification},
                {"reply_to_message_id", replyToMessageId},
                 {"reply_markup", markup},
            }, url);
        }

        public async Task<Response<Message>> SendVenueAsync(ChatId chatId, float latitude, float longitude,
            string title, string address,
            bool disableNotification = false, int replyToMessageId = 0,
            InlineKeyboard replyMarkup = null)
        {
            var markup = "";
            if (replyMarkup != null)
            {
                markup = replyMarkup.GetJson();
            }
            var url = Address + Token + "/sendVenue";
            if (latitude < -90 || latitude > 90) throw new ArgumentException("latitude must be between -90 and 90");
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("longitude must be between -180 and 180");

            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"latitude", latitude},
                {"longitude", longitude},
                {"title", title},
                {"address", address},
                {"disable_notification", disableNotification},
                {"reply_to_message_id", replyToMessageId},
                {"reply_markup", markup},
            }, url);
        }

        public async Task<Response<Message>> SendContactAsync(ChatId chatId, string phoneNumber, string firstName,
            string lastName,
            bool disableNotification = false, int replyToMessageId = 0,
            InlineKeyboard replyMarkup = null)
        {
            var markup = "";
            if (replyMarkup != null)
            {
                markup = replyMarkup.GetJson();
            }
            var url = Address + Token + "/sendContact";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"phoneNumber", phoneNumber},
                {"firstName", firstName},
                {"lastName", lastName},
                {"disable_notification", disableNotification},
                {"reply_to_message_id", replyToMessageId},
                {"reply_markup", markup},
            }, url);
        }

        public void StopRevieving()
        {
            IsReceiving = false;
        }

        public async Task<Response<bool>> SendChatActionAsync(ChatId chatId, ChatAction action)
        {
            var url = Address + Token + "/sendChatAction";
            return await SendRequest<Response<bool>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"action", action.Action},
                //   {"reply_markup", replyMarkup},
            }, url);
        }

        public async Task<Response<Message>> ForwardMessageAsync(ChatId chatId, long fromChatId, long messageId,
            bool disableNotification = false)
        {
            var url = Address + Token + "/forwardMessage";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"from_chat_id", fromChatId},
                {"message_id", messageId},
                {"disable_notification", disableNotification},
            }, url);
        }

        public async Task<Response<Message>> ForwardMessageAsync(ChatId chatId, string fromChatId, long messageId,
            bool disableNotification = false)
        {
            var url = Address + Token + "/forwardMessage";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"from_chat_id", fromChatId},
                {"message_id", messageId},
                {"disable_notification", disableNotification},
            }, url);
        }

        public async Task<Response<UserProfilePhotos>> GetUserProfilePhotosAsync(long userId, int offset = 0,
            int limit = 100)
        {
            var url = Address + Token + "/getUserProfilePhotos";
            return await SendRequest<Response<UserProfilePhotos>>(new Dictionary<string, object>
            {
                {"user_id", userId},
                {"offset", offset},
                {"limit", limit}
            }, url);
        }

        public async Task<Response<File>> GetFileAsync(string fileId)
        {
            var url = Address + Token + "/getFile";
            return await SendRequest<Response<File>>(new Dictionary<string, object>
            {
                {"file_id", fileId},
            }, url);
        }

        public async Task<Response<bool>> KickChatMemberAsync(ChatId chatId, long userId, long untilDate = 0)
        {
            var url = Address + Token + "/kickChatMember";
            return await SendRequest<Response<bool>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"user_id", userId},
                {"until_date", untilDate}
            }, url);
        }

        public async Task<Response<bool>> UnbanChatMemberAsync(ChatId chatId, long userId)
        {
            var url = Address + Token + "/unbanChatMember";
            return await SendRequest<Response<bool>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"user_id", userId},
            }, url);
        }

        public async Task<Response<bool>> RestrictChatMemberAsync(ChatId chatId, long userId, int untilDate = 0,
            bool canSendMessages = false, bool canSendMediaMessages = false, bool canSendOtherMessages = false,
            bool canAddWebPagePreviews = false)
        {
            var url = Address + Token + "/restrictChatMember";
            return await SendRequest<Response<bool>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"user_id", userId},
                {"until_date", untilDate},
                {"can_send_messages", canSendMessages},
                {"can_send_media_messages", canSendMediaMessages},
                {"can_send_other_messages", canSendOtherMessages},
                {"can_add_web_page_previews", canAddWebPagePreviews}
            }, url);
        }

        public async Task<Response<bool>> PromoteChatMemberAsync(ChatId chatId, long userId, bool canChangeInfo = false,
            bool canPostMessages = false, bool canEditMessages = false, bool canDeleteMessages = false,
            bool canInviteUsers = false, bool canRestrictMembers = false, bool canPinMessages = false,
            bool canPromoteMembers = false)
        {
            var url = Address + Token + "/promoteChatMember";
            return await SendRequest<Response<bool>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"user_id", userId},
                {"can_change_info", canChangeInfo},
                {"can_post_messages", canPostMessages},
                {"can_edit_messages", canEditMessages},
                {"can_delete_messages", canDeleteMessages},
                {"can_invite_users", canInviteUsers},
                {"can_restrict_members", canRestrictMembers},
                {"can_pin_messages", canPinMessages},
                {"can_promote_members", canPromoteMembers}
            }, url);
        }

        public async Task<Response<string>> ExportChatInviteLink(ChatId chatId)
        {
            var url = Address + Token + "/exportChatInviteLink";
            return await SendRequest<Response<string>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
            }, url);
        }

        public async Task<Response<bool>> SetChatPhotoAsync(ChatId chatId, TelegramFile photo)
        {
            var url = Address + Token + "/setChatPhoto";
            if (photo.Type == TelegramFileType.Raw)
            {
                return await SendRequest<Response<bool>>(new Dictionary<string, object>
                {
                    {"chat_id", chatId.Id},
                    {"photo", photo},
                }, url);
            }
            throw new ArgumentException("Photo must be new chat photo, uploaded using multipart/form-data");
        }

        public async Task<Response<bool>> DeleteChatPhotoAsync(ChatId chatId)
        {
            var url = Address + Token + "/deleteChatPhoto";
            return await SendRequest<Response<bool>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
            }, url);
        }

        public async Task<Response<bool>> SetChatTitleAsync(ChatId chatId, string title)
        {
            if (title.Length > 0 && title.Length < 256)
            {
                var url = Address + Token + "/setChatTitle";
                return await SendRequest<Response<bool>>(new Dictionary<string, object>
                {
                    {"chat_id", chatId.Id},
                    {"title", title},
                }, url);
            }
            throw new ArgumentException("title lenght must be between 1 and 255 characters");
        }

        public async Task<Response<bool>> SetChatDescriptionAsync(ChatId chatId, string description)
        {
            var url = Address + Token + "/setChatDescription";
            return await SendRequest<Response<bool>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"description", description},
            }, url);
        }

        public async Task<Response<bool>> PinChatMessageAsync(ChatId chatId, long messageId,
            bool disableNotification = false)
        {
            var url = Address + Token + "/pinChatMessage";
            return await SendRequest<Response<bool>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"message_id", messageId},
                {"disable_notification", disableNotification},
            }, url);
        }

        public async Task<Response<bool>> UnpinChatMessageAsync(ChatId chatId)
        {
            var url = Address + Token + "/unpinChatMessage";
            return await SendRequest<Response<bool>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id}
            }, url);
        }

        public async Task<Response<bool>> LeaveChatAsync(ChatId chatId)
        {
            var url = Address + Token + "/leaveChat";
            return await SendRequest<Response<bool>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id}
            }, url);
        }

        public async Task<Response<Chat>> GetChatAsync(ChatId chatId)
        {
            var url = Address + Token + "/getChat";
            return await SendRequest<Response<Chat>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id}
            }, url);
        }

        public async Task<Response<ChatMember[]>> GetChatAdministratorsAsync(ChatId chatId)
        {
            var url = Address + Token + "/getChatAdministrators";
            return await SendRequest<Response<ChatMember[]>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id}
            }, url);
        }

        public async Task<Response<int>> GetChatMembersCountAsync(ChatId chatId)
        {
            var url = Address + Token + "/getChatMembersCount";
            return await SendRequest<Response<int>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id}
            }, url);
        }

        public async Task<Response<ChatMember>> GetChatMemberAsync(ChatId chatId, long userId)
        {
            var url = Address + Token + "/getChatMember";
            return await SendRequest<Response<ChatMember>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"user_id", userId}
            }, url);
        }

        public async Task<Response<bool>> AnswerCallbackQueryAsync(string callbackQueryId, string text = "",
            bool showAlert = false, string url = "", int cacheTime = 0)
        {
            if (text.Length >= 200) throw new ArgumentException("Text length is between 0 and 200");
            var methodUrl = Address + Token + "/answerCallbackQuery";
            return await SendRequest<Response<bool>>(new Dictionary<string, object>
            {
                {"callback_query_id", callbackQueryId},
                {"text", text},
                {"show_alert", showAlert},
                {"url", url},
                {"cache_time", cacheTime}
            }, methodUrl);
        }

        #endregion

        #region UpdatingMessages

        public async Task<Response<Message>> EditMessageTextAsync(string text, ChatId chatId, long messageId,
            string parseMode = "", bool disableWebPagePreview = false)
        {
            var methodUrl = Address + Token + "/editMessageText";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"text", text},
                {"chat_id", chatId.Id},
                {"message_id", messageId},
                {"parse_mode", parseMode},
                {"disable_web_page_preview", disableWebPagePreview}
            }, methodUrl);
        }

        public async Task<Response<Message>> EditMessageTextAsync(string text, string inlineMessageId,
            InlineKeyboard replyMarkup = null,
            string parseMode = "", bool disableWebPagePreview = false)
        {
            var markup = "";
            if (replyMarkup != null)
            {
                markup = replyMarkup.GetJson();
            }
            var methodUrl = Address + Token + "/editMessageText";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"text", text},
                {"inline_message_id", inlineMessageId},
                {"parse_mode", parseMode},
                {"disable_web_page_preview", disableWebPagePreview},
                {"reply_markup", markup}
            }, methodUrl);
        }

        public async Task<Response<Message>> EditMessageCaptionAsync(string caption, ChatId chatId, long messageId,
            string parseMode = "", bool disableWebPagePreview = false, InlineKeyboard replyMarkup = null)
        {
            var markup = "";
            if (replyMarkup != null)
            {
                markup = replyMarkup.GetJson();
            }
            var methodUrl = Address + Token + "/editMessageCaption";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"caption", caption},
                {"chat_id", chatId.Id},
                {"message_id", messageId},
                {"parse_mode", parseMode},
                {"disable_web_page_preview", disableWebPagePreview},
                {"reply_markup", markup}

            }, methodUrl);
        }

        public async Task<Response<Message>> EditMessageCaptionTextAsync(string text, string inlineMessageId,
            InlineKeyboard replyMarkup = null, string parseMode = "", bool disableWebPagePreview = false)
        {
            var markup = "";
            if (replyMarkup != null)
            {
                markup = replyMarkup.GetJson();
            }
            var methodUrl = Address + Token + "/editMessageCaption";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"text", text},
                {"inline_message_id", inlineMessageId},
                {"reply_markup", replyMarkup},
                {"parse_mode", parseMode},
                {"disable_web_page_preview", disableWebPagePreview}
            }, methodUrl);
        }

        public async Task<Response<Message>> EditMessageReplyMarkupAsync(ChatId chatId, long messageId,
            InlineKeyboard replyMarkup = null)
        {
            var markup = "";
            if (replyMarkup != null)
            {
                markup = replyMarkup.GetJson();
            }
            var methodUrl = Address + Token + "/editMessageReplyMarkup";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"chat_id", chatId},
                {"message_id", messageId},
                {"reply_markup", markup},
            }, methodUrl);
        }

        public async Task<Response<Message>> EditMessageReplyMarkupAsync(long inlineMessageId,
            InlineKeyboard replyMarkup = null)
        {
            var markup = "";
            if (replyMarkup != null)
            {
                markup = replyMarkup.GetJson();
            }
            var methodUrl = Address + Token + "/editMessageReplyMarkup";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"inline_message_id", inlineMessageId},
                {"reply_markup", markup},
            }, methodUrl);
        }

        public async Task<Response<Message>> DeleteMessageAsync(ChatId chatId,
            long messageId)
        {
            var methodUrl = Address + Token + "/deleteMessage";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"inline_message_id", chatId.Id},
                {"message_id", messageId},
            }, methodUrl);
        }

        #endregion

        public async Task<Response<Message>> SendStickerAsync(ChatId chatId, TelegramFile sticker,
            bool disableNotification = false, int replyToMessageId = 0, InlineKeyboard replyMarkup = null)
        {
            var markup = "";
            if (replyMarkup != null)
            {
                markup = replyMarkup.GetJson();
            }
            var methodUrl = Address + Token + "/sendSticker";
            return await SendRequest<Response<Message>>(new Dictionary<string, object>
            {
                {"chat_id", chatId.Id},
                {"sticker", sticker},
                {"disable_notification", disableNotification},
                {"reply_to_message_id", replyToMessageId},
               {"reply_markup", markup}
            }, methodUrl);
        }

        public async Task<Response<StickerSet>> GetStickerSetAsync(string name)
        {
            var methodUrl = Address + Token + "/getStickerSet";
            return await SendRequest<Response<StickerSet>>(new Dictionary<string, object>
            {
                {"name", name},
            }, methodUrl);
        }
        // TODO: Inline mode
        // TODO: Payments
        // TODO: Games
    }
}