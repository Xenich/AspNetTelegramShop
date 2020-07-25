using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TelegramShopAsp.DB;
using TelegramShopAsp.Responses;
using StoredProcedureEFCore;
using TelegramShopAsp.DB.prOutputs;

namespace TelegramShopAsp
{
    public class Buyer : User
    {
        public new StateDelegateBuyer CurrentState;
        public string telephone;
        public Dictionary<DB.Goods, string> cart = new Dictionary<DB.Goods, string>();              // корзина покупателя: товар - количество
        public DB.Buyers DbBuyer;

        public Buyer(int telegramId) : base(telegramId)
        {
        }

            // получение покупателя, который существует в базе данных и привязка сущности из БД к модели
        public static Buyer GetBuyer(DB.Buyers DBbuyer)
        {
            Buyer buyer = new Buyer(DBbuyer.TelegramId);
            buyer.telephone = DBbuyer.phone;
            buyer.name = DBbuyer.name;
            buyer.adress = DBbuyer.adress;
            buyer.cityUID = DBbuyer.CityUID;
            buyer.CurrentState = Behavior.UIDToState_Buyer_Dic[DBbuyer.StateUID];
            buyer.currentItemEdit = DBbuyer.currentItemEdit;


            Dictionary<Goods,string> dic;
            buyer._context.LoadStoredProc("prGetBuyerCart")
                .AddParam("telegramId", DBbuyer.TelegramId)
                .Exec(r => dic = r.ToList<CartGoodFromStoredPr>().ToDictionary(c => BOT.GetBasicLeave(c.UID), c => c.Quantity));

            //SqlParameter[] @params =
            //    {
            //        new SqlParameter("@telegramId", DBbuyer.TelegramId) {Direction = System.Data.ParameterDirection.Input},                         // входной параметр в хранимке @id                    
            //    };
            //string query = $"EXEC [prGetBuyerCart] @telegramId";
            //buyer.cart = buyer._context.CartGoodFromStoredPr.SqlQuery(query, @params)
                                                            //.ToDictionary(c=> BOT.basicLeavesDic[c.UID], c=>c.Quantity);
            buyer.DbBuyer = DBbuyer;
            return buyer;
        }

        public override void HandleMessage(string callbackmessage, int massageId, string currentCallbackId, bool isNotPhotoMessage)
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
                if (Behavior.UIDToState_Buyer_Dic.Keys.Contains(callbackData))     // если да, то вызываем соответствующий метод
                {
                    CurrentState = Behavior.UIDToState_Buyer_Dic[callbackData];
                    CurrentState(currentStateUID, this);     // у методов, которые не ожидают данных первый принимаемый аргумент - UID метода для возврата
                }
                else                             // если нет - нам пришли данные для текущего CurrentState - нам их надо обработать в текущем CurrentState
                {

                    if (Behavior.UIDToState_Buyer_Dic.Keys.Contains(currentStateUID))
                    {
                        CurrentState = Behavior.UIDToState_Buyer_Dic[currentStateUID];
                        CurrentState(callbackData, this);
                    }
                }
            }
        }

        public override void HandleMessage(Message mes)
        {
            //ClearMessagesWithKeyboardsList();
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

        public void SetPhone(string phone)
        {
            this.telephone = phone;
            _context.Buyers.FirstOrDefault(b => b.TelegramId == this.telegramId).phone = phone;
            _context.SaveChanges();
        }

        public void SetActivity()
        {           
           _context.Buyers.FirstOrDefault(s => s.TelegramId == telegramId).isActive=true;
            _context.SaveChanges();
        }

        public override void MakeOffer()
        {
            cart.Clear();
            //TODO      реализовать
        }

    }
}
