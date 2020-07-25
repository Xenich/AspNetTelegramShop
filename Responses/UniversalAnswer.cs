using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramShopAsp.Responses
{     
    public class Response
    {
        public bool ok { get; set; }
        public List<Update> result { get; set; }
    }

    public class Update
    {
        public int update_id { get; set; }
        //public string message_id { get; set; }
        public Message message { get; set; }
        public CallbackQuery callback_query { get; set; }
    }

    public class CallbackQuery
    {
        public string id { get; set; }
        public From from { get; set; }
        public Message message { get; set; }
        public string chat_instance { get; set; }
        public string data { get; set; }
    }
    public class Message
    {
        public int message_id { get; set; }
        public From from { get; set; }
        public Chat chat { get; set; }
        public int date { get; set; }
        public string text { get; set; }
        public Photo[] photo { get; set; }
        //public string reply_markup { get; set; }
    }

    public class From
    {
        public int id { get; set; }
        //public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string language_code { get; set; }

        public string username { get; set; }
    }

    public class Chat
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string type { get; set; }
    }

    public class Photo
    {
        public string file_id { get; set; }
        public string file_unique_id { get; set; }
        public long file_size { get; set; }
        public long width { get; set; }
        public long height { get; set; }
    }


    //public class From2
    //{
    //    public int id { get; set; }
    //    public bool is_bot { get; set; }
    //    public string first_name { get; set; }
    //    public string language_code { get; set; }
    //}

    //public class From3
    //{
    //    public int id { get; set; }
    //    public bool is_bot { get; set; }
    //    public string first_name { get; set; }
    //    public string username { get; set; }
    //}

    //public class Chat2
    //{
    //    public int id { get; set; }
    //    public string first_name { get; set; }
    //    public string type { get; set; }
    //}

    //public class Entity
    //{
    //    public int offset { get; set; }
    //    public int length { get; set; }
    //    public string type { get; set; }
    //}

    //public class ReplyMarkup
    //{
    //    //public List<List<>> inline_keyboard { get; set; }
    //}


    //public class Message2
    //{
    //    public int message_id { get; set; }
    //    public From from { get; set; }
    //    public Chat chat { get; set; }
    //    public int date { get; set; }
    //    public string text { get; set; }
    //    public List<Entity> entities { get; set; }
    //   // public ReplyMarkup reply_markup { get; set; }
    //}


}
