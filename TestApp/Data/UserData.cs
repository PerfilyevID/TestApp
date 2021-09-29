using Newtonsoft.Json;
using System;

namespace TestApp.Data
{
    /// <summary>
    /// Данные с бота.
    /// </summary>
    public class UserData
    {
        public UserData() { }

        [JsonIgnore]
        public DateTime DataTime { get; set; }
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Время в голосовом канале.
        /// </summary>
        [JsonProperty("voice_time")]
        public int Voicetime { get; set; }

        /// <summary>
        /// Время с включенным микрофоном.
        /// </summary>
        [JsonProperty("unmute_time")]
        public int UnmuteTime { get; set; }

        /// <summary>
        /// время с включенной видеокамерой.
        /// </summary>
        [JsonProperty("video_time")]
        public int VideoTime { get; set; }

        /// <summary>
        /// время присутствия с 12:00 До 12:15.
        /// </summary>
        [JsonProperty("mid_day_activity")]
        public int MidDayActivity { get; set; }

        /// <summary>
        /// Количество времени в афк канале.
        /// </summary>
        [JsonProperty("afk_time")]
        public int AfkTime { get; set; }

        /// <summary>
        /// Присутствие в канале от 2 до 4 человек. Когда в канале 1 человек, время пишется только в voicetime .
        /// </summary>
        [JsonProperty("users_15time")]
        public int Users15time { get; set; }

        /// <summary>
        /// Присутствие в канале от 5 человек.
        /// </summary>
        [JsonProperty("users_4time")]
        public int Users4time { get; set; }

        /// <summary>
        /// Кол-во реакций под сообщениями за день.
        /// </summary>
        [JsonProperty("reactions")]
        public int Reactions { get; set; }

        /// <summary>
        /// Количество сообщений, написанных юзером.
        /// </summary>
        [JsonProperty("messages")]
        public Message[] Messages { get; set; }

        /// <summary>
        /// Время первого входа.
        /// </summary>
        [JsonProperty("first_connection")]
        [JsonConverter(typeof(WeirdDateSerializer))]
        public DateTime FirstConnection { get; set; }

        /// <summary>
        /// Время до следующего сброса в миллисекундах.
        /// </summary>
        [JsonProperty("last_reset")]
        public long LastReset { get; set; }

    }

    [Obsolete]
    public class Message
    {

    }
}
