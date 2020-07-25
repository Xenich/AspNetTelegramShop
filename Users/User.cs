using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TelegramShopAsp.Classes;
using TelegramShopAsp.Responses;

namespace TelegramShopAsp
{
    public class User
    {
        //public static Dictionary<int, StateDelegate> statesDic = new Dictionary<int, StateDelegate>();

        static User()
        {

        }
        public DB.DB_A566BC_xenichContext _context = new DB.DB_A566BC_xenichContext();

        public StateDelegate CurrentState;          // указатель на текущее состояние конечного автомата
        public string currentItemEdit;              // например редактируется кол-во заказа - тут будет UID продукта
        public bool isLastMessageKeyboard = false;          // показывает было ли последне сообщение отправлено с inline-клавиатуры (нужно, чтоб знать, делать ли удаление предыдущего сообщения или замену)
        //public bool isLastMessagePhotoKeyboard = false;          // показывает было ли последне сообщение отправлено с фото inline-клавиатуры (нужно, чтоб знать, делать ли удаление предыдущего сообщения или замену)        

        public int telegramId;
        //public int chatId;
        public string name;
        public string adress;
        public string cityUID = "";

        public string currentCallback_query_id;         // Unique identifier for this query - используется для всплывающих сообщений
        public bool WaitForUserInput = false;
        public int message_id = 0;                      // id пришедшего сообщения - по нему потом делаем подмену клавиатуры

        public User(int telegramId)//, int chatId)
        {
            this.telegramId = telegramId;
            //CurrentState = GetState();
        }

        /// <summary>
        /// Обработка сообщения, пришедшего от Inline-клавиатуры. Сообщения приходят в виде: UID_текущего состояния - 32 байта + данные
        /// </summary>
        /// <param name="callbackmessage">колбек от inline клавиатуры</param>
        /// <param name="massageId">по этому id происходит замена сообщения с клавиатурой</param>
        /// <param name="currentCallbackId">id для всплывающего сообщения</param>
        /// <param name="isNotPhotoMessage">показывает, что это не сообщение с фотографией</param>
        public virtual void HandleMessage(string callbackmessage, int massageId, string currentCallbackId, bool isNotPhotoMessage)
        {
            message_id = massageId;             
            this.currentCallback_query_id = currentCallbackId;
            string currentStateUID;
            isLastMessageKeyboard = isNotPhotoMessage;      // делаем проверку на то, что это прислано не из фотосообщения
            if (callbackmessage.Length > 31)
            {
                currentStateUID = callbackmessage.Substring(0, 32);     // UID текущего состояния согласно callbackmessage - по идее должен совпадать с UID CurrentState
                string callbackData = callbackmessage.Split(new string[] { currentStateUID }, StringSplitOptions.RemoveEmptyEntries)[0];
                    // делаем предварительную проверку, являются ли вторые 32 байта уидом какого-то метода
                if (Behavior.UIDToState_Dic.Keys.Contains(callbackData))     // если да, то вызываем соответствующий метод
                {
                    CurrentState = Behavior.UIDToState_Dic[callbackData];
                    CurrentState(currentStateUID, this);     // у методов, которые не ожидают данных первый принимаемый аргумент - UID метода для возврата
                }
                else                             // если нет - нам пришли данные для текущего CurrentState - нам их надо обработать в текущем CurrentState
                {
                    
                    if (Behavior.UIDToState_Dic.Keys.Contains(currentStateUID))
                    {
                        CurrentState = Behavior.UIDToState_Dic[currentStateUID];
                        CurrentState(callbackData, this);
                        //Behavior.UID_State_Dic[currentStateUID](callbackData, this);
                    }
                }
            }
        }


            //обработка сообщения, введённого пользователем вручную или посылка фото
        public virtual void HandleMessage(Message mes)
        {
            isLastMessageKeyboard = false;
            string message = mes.text;
            if (message == "/start")
                Behavior.Hello("", this);
            else 
            {
                if (CurrentState != null)
                {                                   
                    CurrentState(message, this);
                }                   
            }
        }
            // для многопоточности
        public void HandleMessage(Object o)
        {
            HandleMessageInputObject inputs = (HandleMessageInputObject)o;
            this.currentCallback_query_id = inputs.currentCallbackId;
            CurrentState(inputs.message, this);
        }

        /// <summary>
        /// Получение состояния из базы данных
        /// </summary>
        public StateDelegate GetState()
        {

            return Behavior.Hello;
        }

        public void ClearMessagesWithKeyboardsList()
        {
            string s = TelegramRequest.DeleteMessage(message_id.ToString(), telegramId.ToString());
            message_id = 0;
            //while (messagesWithKeyboardId.Count > 0)
            //{
            //    TelegramRequest.DeleteMessage(messagesWithKeyboardId.Pop(), chatId.ToString());
            //}
        }

        public virtual void MakeOffer()
        { }

        public void SetName(string name)
        {
            this.name = name;
            if (this is Buyer)
            {
                _context.Buyers.FirstOrDefault(b => b.TelegramId == this.telegramId).name = name;
                _context.SaveChanges();
            }
            if (this is Seller)
            {
                _context.Sellers.FirstOrDefault(b => b.TelegramId == this.telegramId).Name = name;
                _context.SaveChanges();
            }
        }

        public void SetCity(string cityUID)
        {
            this.cityUID = cityUID;
            if (this is Buyer)
            {
                _context.Buyers.FirstOrDefault(b=>b.TelegramId==this.telegramId).CityUID= cityUID;
                _context.SaveChanges();
            }
            if (this is Seller)
            {
                _context.Sellers.FirstOrDefault(b => b.TelegramId == this.telegramId).CityUID = cityUID;
                _context.SaveChanges();
            }
        }

        public void SetAdress(string adress)
        {
            this.adress = adress;
            if (this is Buyer)
            {
                _context.Buyers.FirstOrDefault(b => b.TelegramId == this.telegramId).adress = adress;
                _context.SaveChanges();
            }
            if (this is Seller)
            {
                _context.Sellers.FirstOrDefault(b => b.TelegramId == this.telegramId).Adress = adress;
                _context.SaveChanges();
            }
        }
    }

    class HandleMessageInputObject
    {
        public string message;
        public int massageId;
        public string currentCallbackId;
        public HandleMessageInputObject(string message, int massageId, string currentCallbackId)
        {
            this.message = message;
            this.massageId = massageId;
            this.currentCallbackId = currentCallbackId;
        }
    }  
}
