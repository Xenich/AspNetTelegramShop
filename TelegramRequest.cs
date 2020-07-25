using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;
using TelegramShopAsp.Responses;

using RestSharp;

namespace TelegramShopAsp
{

    public static class TelegramRequest
    {
        static int lastUpdate = 0;   // текущий номер сообщения
        //static string token = "717822946:AAHtT8L2D2P6afhTBdB_AgGe1VwviBiqW8U";      // eFreelance_bot
        static string token = "1120463837:AAHEvmnejgfiH7CvnEts9M5TliR-SQdigpc";       //   @MarketTele_bot
        public static bool notAborted = true;
        //IBot _bot;

        //public TelegramRequest()//IBot _bot)
        //{
        //    //this._bot = _bot;
        //}

        public static void Start()
        {
           // GetUpdates();
        }

        public static void GetUpdates()
        {
            while (notAborted)
            {
                string respText = "";
                try
                {
                    Uri url = new Uri(@"https://api.telegram.org/bot" + token + @"/getUpdates?offset=" + lastUpdate.ToString());

                    WebRequest request = WebRequest.Create(url);
                    WebResponse response = request.GetResponse();

                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    respText = reader.ReadToEnd().TrimStart(new char[] { (char)0xEF, (char)0xBB, (char)0xBF });

                    //Stream stream = response.GetResponseStream();
                    //byte[] buf = new byte[stream.Length];
                    //response.GetResponseStream().Read(buf,0,(int)stream.Length);
                    //if (buf.Length > 2 && buf[0] == 0xEF && buf[1] == 0xBB && buf[2] == 0xBF)
                    //{
                    //    buf = buf.Skip(3).ToArray();                        // Формируем выходной массив, убирая маркер
                    //}
                    //string OutputString = Encoding.UTF8.GetString(buf).Trim();

                    Response resp = null;
                    try
                    {
                        resp = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(respText);
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Handle(ex.Message);
                    }

                    if (resp.ok && resp.result.Count > 0)
                    {
                        Console.WriteLine(Environment.NewLine);
                        for (int i = 0; i < resp.result.Count; i++)
                        {
                            int telegramId;
                            //int chatId;
                            int messageId;
                            string messageText="";
                            string currentCallbackId = "";

                            if (resp.result[i].message != null)             // пришло сообщение, введенное пользователем
                            {
                                Console.WriteLine(resp.result[i].message.from.first_name + ": " + resp.result[i].message.text + Environment.NewLine);
                                telegramId = resp.result[i].message.from.id;
                                //chatId = resp.result[i].message.chat.id;        // chatId=UserId
                                messageId = resp.result[i].message.message_id;  // порядковый номер собщения в чате
                                //messageText = resp.result[i].message.text;      // текст сообщения введённый юзером
                                //Message message = resp.result[i].message;       // сообщение юзера (текст или фото)
                            }
                            else            // if (resp.result[i].callback_query != null)   -  пришло сообщение от inline-клавиатуры
                            {
                                Console.WriteLine(resp.result[i].callback_query.from.first_name + ": " + resp.result[i].callback_query.data.ToString() + Environment.NewLine);
                                telegramId = resp.result[i].callback_query.from.id;                     // UserId=chatId
                                //chatId = resp.result[i].callback_query.message.chat.id;         // chatId=UserId
                                messageId = resp.result[i].callback_query.message.message_id;   // порядковый номер собщения в чате
                                messageText = resp.result[i].callback_query.data;               // данные забитые от клавиатуры
                                currentCallbackId = resp.result[i].callback_query.id;           // Unique identifier for this query
                            }
                            User user = BOT.GetUser(telegramId);
                            if(user == null)        // если это совершенно новый юзер
                            {
                                user = new User(telegramId);
                                user.CurrentState = Behavior.Hello;
                            }

                            lastUpdate = resp.result[i].update_id;
                                                        
                            if (resp.result[i].callback_query != null)       // пришло сообщение от клавиатуры
                                user.HandleMessage(messageText, messageId, currentCallbackId, resp.result[i].callback_query.message.photo==null);
                            if (resp.result[i].message != null)             // пришло сообщение, введенно пользователем
                                user.HandleMessage(resp.result[i].message);
                        }
                        lastUpdate++;
                    }
                    Console.Write(".");
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + Environment.NewLine + respText);
                }

            }
        }

            // сообщение, пришедшее из вебхука
        public static void UpdateFromWebHook(Update result)
        {
            int telegramId;
            int messageId;
            string currentCallbackId = "";
            string messageText = "";

            try
            {
                if (result.message != null)             // пришло сообщение, введенное пользователем
                {
                    Console.WriteLine(result.message.from.first_name + ": " + result.message.text + Environment.NewLine);
                    telegramId = result.message.from.id;
                    //chatId = result.message.chat.id;        // chatId=UserId
                    messageId = result.message.message_id;  // порядковый номер собщения в чате
                                                            //messageText = result.message.text;      // текст сообщения введённый юзером
                                                            //Message message = result.message;       // сообщение юзера (текст или фото)
                }
                else            // т.е. if (result.callback_query != null)   -  пришло сообщение от inline-клавиатуры
                {
                    Console.WriteLine(result.callback_query.from.first_name + ": " + result.callback_query.data.ToString() + Environment.NewLine);
                    telegramId = result.callback_query.from.id;                     // UserId=chatId
                                                                                    //chatId = result.callback_query.message.chat.id;         // chatId=UserId
                    messageId = result.callback_query.message.message_id;   // порядковый номер собщения в чате
                    messageText = result.callback_query.data;               // данные забитые от клавиатуры
                    currentCallbackId = result.callback_query.id;           // Unique identifier for this query
                }
                User user = BOT.GetUser(telegramId);
                if (user == null)        // если это совершенно новый юзер
                {
                    user = new User(telegramId);
                    user.CurrentState = Behavior.Hello;
                }

                lastUpdate = result.update_id;
                if (result.callback_query != null)       // пришло сообщение от клавиатуры
                    user.HandleMessage(messageText, messageId, currentCallbackId, result.callback_query.message.photo == null);
                if (result.message != null)             // пришло сообщение, введенно пользователем
                    user.HandleMessage(result.message);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }

        // отправление сообщения в чат без клавиатуры
        public static void SendSimpleMessage(string message, int chatID)
        {
            Dictionary<string, string> messageDict = new Dictionary<string, string>()
            {
                { "text", message },
                { "chat_id", chatID.ToString() }
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(messageDict);
            Send("sendMessage", json);
            //return SendWithResponse("sendMessage", json);
        }

            // отправление сообщение в чат с клавиатурой без response
        public static void SendMessage(string message, int chatID, IKeyboard keyboard)
        {
            Dictionary<string, string> messageDict = new Dictionary<string, string>();
            try
            {
                messageDict.Add("text", message);
                messageDict.Add("chat_id", chatID.ToString());
                messageDict.Add("parse_mode", "Markdown");
                messageDict.Add("reply_markup", keyboard.Generate());
                //messageDict.Add("reply_markup", kbrd.Generate());
                // messageDict.Add("reply_markup", "{\"keyboard\": [[{\"text\":\"Показать первое меню\", \"request_contact\": true }, \"показать второе меню\"], [ \"Показать третье меню\"]] , \"one_time_keyboard\" : false , \"resize_keyboard\" : true }");
                // messageDict.Add("one_time_keyboard", "true");
                // messageDict.Add("parse_mode", "html");
                // messageDict.Add("disable_web_page_preview", "true");
                // messageDict.Add("disable_notification", "true");

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(messageDict);
                Send("sendMessage", json);
            }

            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }

            // отправление сообщение в чат с клавиатурой и получение response, чтоб узнать id сообщения
        public static string SendMessageWithResponse(string message, int chatID, IKeyboard keyboard)
        {
            Dictionary<string, string> messageDict = new Dictionary<string, string>()
            {
                { "text", message },
                { "chat_id", chatID.ToString() },
                { "parse_mode", "Markdown" },
                { "reply_markup", keyboard.Generate() }
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(messageDict);
            return SendWithResponse("sendMessage", json);
        }

            // отправление сообщениz с фотографией в чат с клавиатурой и получение response, чтоб узнать id сообщения
        public static string SendMessageWithPhoto(string message, string photoId, int chatID, IKeyboard keyboard)
        {
            Dictionary<string, string> messageDict = new Dictionary<string, string>()
            {
                { "caption", message },
                //messageDict.Add("text", message);
                { "photo", photoId },
                { "chat_id", chatID.ToString() },
                { "parse_mode", "Markdown" },
                { "reply_markup", keyboard.Generate() }
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(messageDict);
            //Send("sendPhoto", json);
            return SendWithResponse("sendPhoto", json);
        }

        public static void ReplyKeyboardRemove(string message, string chat_id)
        {
            Dictionary<string, string> messageDict = new Dictionary<string, string>()
            {
                { "text", message },
                { "chat_id", chat_id },
                { "reply_markup", "{ \"remove_keyboard\":\"true\"}" }
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(messageDict);
            Send("sendMessage", json);
        }

        public static void EditMessageText(string message, int chatID, int message_id)//, string inlineKeyboard)
        {
            Dictionary<string, string> messageDict = new Dictionary<string, string>()
            {
                { "text", message },
                { "chat_id", chatID.ToString() },
                { "parse_mode", "Markdown" },
                { "message_id", message_id.ToString() }
            };
            //messageDict.Add("reply_markup", inlineKeyboard);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(messageDict);
            Send("editMessageText", json);
        }

        public static void EditMessageText(string message, int chatID, string message_id, InlineKeyboardMarkup ikbm)
        {
            Dictionary<string, string> messageDict = new Dictionary<string, string>()
            {
                { "text", message },
                { "chat_id", chatID.ToString() },
                { "message_id", message_id },
                { "parse_mode", "Markdown" },
                { "reply_markup", ikbm.Generate() }
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(messageDict);
            //string resp = SendWithResponse("editMessageText", json);
            Send("editMessageText", json);            // вернуть
        }


        /// <summary>
        /// Всплывающее сообщение при нажатии пользователем книпки inline-клавиатуры
        /// </summary>
        /// <param name="message">сообщение, которое будет всплывать у пользователя</param>
        /// <param name="callback_query_id">callback_query_id который пришёл в запросе после нажатия пользователем кнопки inline-клавиатуры</param>
        public static void AnswerCallbackQuery(string message, string callback_query_id)
        {
            Dictionary<string, string> messageDict = new Dictionary<string, string>()
            {
                { "text", message },
                { "callback_query_id", callback_query_id.ToString() }
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(messageDict);
            Send("answerCallbackQuery", json);
        }

        public static string DeleteMessage(string message_id, string chat_id)
        {
            Dictionary<string, string> messageDict = new Dictionary<string, string>()
            {
                { "message_id", message_id.ToString() },
                { "chat_id", chat_id }
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(messageDict);
            string s = SendWithResponse("deleteMessage", json);
            return s;
            int i = 5;
            i++;
        }

        private static void Send(string method, string messageBody)
        {
            try
            {
                string uri = "https://api.telegram.org/bot" + token + "/" + method;
                var client = new RestClient(uri);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", messageBody, ParameterType.RequestBody);
                client.Execute(request);
                

            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);               
            }
        }

        private static string SendWithResponse(string method, string messageBody)
        {
            try
            {
                string uri = "https://api.telegram.org/bot" + token + "/" + method;
                var client = new RestClient(uri);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", messageBody, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                string ret = response.Content;              
                return ret;
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle("Exception with return" + Environment.NewLine + ex);
                
                return "BADSTATUS";
            }
        }
    }
}
