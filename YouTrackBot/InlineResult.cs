using Newtonsoft.Json;
using Telegram.Bot.Types.InlineQueryResults;

namespace YouTrackBot
{
    public class InlineResult : InlineQueryResultBase
    {
        [JsonProperty(Required = Required.Always)]
        public string Message { get; set; }
        public string StickerFileId { get; set; }

        public InlineResult() : base(InlineQueryResultType.Sticker)
        {

        }

        public InlineResult(string id, string message) : base(InlineQueryResultType.Sticker, id)
        {
            Message = message;
            StickerFileId = "";
        }
    }
}
