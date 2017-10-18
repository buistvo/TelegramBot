using System;

namespace TelegramBot.Types
{
    public class ChatAction
    {
        public string Action { get; }

        public new string ToString()
        {
            return Action;
        }
        public ChatAction(ChatActionType action)
        {
            switch (action)
            {
                case ChatActionType.Typing:
                    Action = "typing";
                    break;
                case ChatActionType.UploadPhoto:
                    Action = "upload_photo";
                    break;
                case ChatActionType.RecordVideo:
                    Action = "record_video";
                    break;
                case ChatActionType.UploadVideo:
                    Action = "upload_video";
                    break;
                case ChatActionType.RecordAudio:
                    Action = "record_audio";

                    break;
                case ChatActionType.UploadAudio:
                    Action = "upload_audio";

                    break;
                case ChatActionType.UploadDocument:
                    Action = "upload_document";

                    break;
                case ChatActionType.FindLocation:
                    Action = "find_location";

                    break;
                case ChatActionType.RecordVideoNote:
                    Action = "record_video_note";

                    break;
                case ChatActionType.UploadVideoNote:
                    Action = "upload_video_note";

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
    }

    public enum ChatActionType
    {
        Typing,
        UploadPhoto,
        RecordVideo,
        UploadVideo,
        RecordAudio,
        UploadAudio,
        UploadDocument,
        FindLocation,
        RecordVideoNote,
        UploadVideoNote
    }
}