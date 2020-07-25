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
    public class Seller : User
    {
        public new StateDelegateSeller CurrentState;
        public List<string> telephones = new List<string>();
        public string description;
        public bool isActive = true;
        //public new bool isLastMessageKeyboard = false;                                                              // показывает было ли последне сообщение отправлено с inline-клавиатуры (нужно, чтоб знать, делать ли удаление предыдущего сообщения или замену)
        public Dictionary<string, DB.SellerGoods> sellerGoods = new Dictionary<string, DB.SellerGoods>();       // словарь UID -> товар продавца 
        public Dictionary<string, List<string>> parentProductsDic = new Dictionary<string, List<string>>();     // словарь UID категории -> список подкатегорий продавца
        DB.Sellers DBseller;

        public Seller(int telegramId): base(telegramId)
        {
            //CurrentState = state;
        }
            // Сконструировать продавца из БД. если такого в базе данных нет, то создать нового
        public static Seller GetSeller(int telegramId)
        {
            Seller seller = new Seller(telegramId);
            DB.Sellers DBseller;

            DBseller = seller._context.Sellers.FirstOrDefault(u => u.TelegramId == telegramId);
            if (DBseller == null)                   // Если в БД такого нет - создаём новую сущность в БД и привязываем сущность из БД к сущности продавца
            {
                DBseller = new DB.Sellers
                {
                    TelegramId = telegramId,
                    StateUID = Behavior.StateToUID_Dic[Behavior.ChooseUserType]
                };
                seller._context.Sellers.Add(DBseller);
                seller._context.SaveChanges();
                seller.DBseller = DBseller;
            }
            else                                    // Если в БД такой есть - привязываем сущность из БД к сущности продавца
                seller = GetSeller(DBseller);
            return seller;
        }
            
            // получение продавца, который существует в базе данных и привязка сущности из БД к модели
        public static Seller GetSeller(DB.Sellers DBseller)
        {
            Seller seller = new Seller(DBseller.TelegramId)
            {
                adress = DBseller.Adress,
                cityUID = DBseller.CityUID,
                CurrentState = Behavior.MainMenuSeller,// Behavior.UID_State_Dic[DBseller.StateUID];
                description = DBseller.Description,
                name = DBseller.Name,
                isActive = DBseller.IsActive
            };

            if (DBseller.Phone != null)
            {
                seller.telephones = DBseller.Phone.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            seller.sellerGoods = seller._context.SellerGoods
                                    .Where(g => g.SellerTelegramId == seller.telegramId)
                                    .ToDictionary(g => g.GoodUID, g => g);

            seller.GenerateDic();
            seller.DBseller = DBseller;
            return seller;
        }

        private DB.Sellers GetDBSeller()
        {
            return _context.Sellers.FirstOrDefault(u => u.TelegramId == telegramId);
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
                if (Behavior.UIDToState_Seller_Dic.Keys.Contains(callbackData))     // если да, то вызываем соответствующий метод
                {
                    CurrentState = Behavior.UIDToState_Seller_Dic[callbackData];
                    CurrentState(currentStateUID, this);     // у методов, которые не ожидают данных первый принимаемый аргумент - UID метода для возврата
                }
                else                             // если нет - нам пришли данные для текущего CurrentState - нам их надо обработать в текущем CurrentState
                {

                    if (Behavior.UIDToState_Seller_Dic.Keys.Contains(currentStateUID))
                    {
                        CurrentState = Behavior.UIDToState_Seller_Dic[currentStateUID];
                        CurrentState(callbackData, this);
                    }
                }
            }
        }

        public override void HandleMessage(Message mes)
        {
            ClearMessagesWithKeyboardsList();
            isLastMessageKeyboard = false;
            string message = mes.text;
            if (message == "/start")
                Behavior.Hello("", this);
            else
            {
                if (CurrentState != null)
                {
                    if (CurrentState == Behavior.AddedGoodPhoto)       // добавление фото продавцом
                    {
                        if (mes.photo != null)
                        {
                            string photo = mes.photo.First().file_id + "*" + mes.photo.Last().file_id;
                            CurrentState(photo, this);
                        }
                        else
                            CurrentState("ERROR", this);
                    }
                    else
                        CurrentState(message, this);
                }
            }
        }

        public void DeletePhone(string phone)
        {
            if (telephones.Contains(phone))
            {
                telephones.Remove(phone);
                string phonesToDB = "";
                foreach (string telehpone in telephones)
                {
                    phonesToDB += telehpone + ";";
                }

                _context.Sellers.FirstOrDefault(s => s.TelegramId == telegramId).Phone = phonesToDB;
                _context.SaveChanges();
            }
        }

        public void AddPhone(string phone)
        {
            telephones.Add(phone);
            string phonesToDB = "";
            foreach (string telehpone in telephones)
            {
                phonesToDB += telehpone + ";";
            }
            if (phonesToDB.Length < 50)
            {
                _context.Sellers.FirstOrDefault(s => s.TelegramId == telegramId).Phone = phonesToDB;
                _context.SaveChanges();
            }
        }

        public void SetDescription(string description)
        {
            this.description = description;
            _context.Sellers.FirstOrDefault(s => s.TelegramId == telegramId).Description = description;
            _context.SaveChanges();
        }

        public void SetActivity()
        {
            isActive = !isActive;
            DB.Sellers sellers = _context.Sellers.FirstOrDefault(s => s.TelegramId == telegramId);
            sellers.IsActive = !sellers.IsActive;
            _context.SaveChanges();
        }

        public DB.SellerGoods CreateGood(string baseGoodUID)
        {
            DB.SellerGoods newGood = new DB.SellerGoods();
            try
            {
                string goodUID = Guid.NewGuid().ToString("N");
                //DB.SellerGoods newGood = new DB.SellerGoods();
                newGood.BasicGoodUID = baseGoodUID;
                newGood.GoodUID = goodUID;
                newGood.isActive = false;
                newGood.SellerTelegramId = telegramId;
                _context.SellerGoods.Add(newGood);
                _context.SaveChanges();
                sellerGoods.Add(goodUID, newGood);
                GenerateDic();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
            return newGood;
        }




        public bool DeleteGood(string GoodUID)
        {
            try
            {
                DB.SellerGoods good = _context.SellerGoods.FirstOrDefault(g => g.GoodUID == GoodUID);
                if (good != null)
                {
                    _context.SellerGoods.Remove(good);
                    _context.SaveChanges();
                }
                if (sellerGoods.ContainsKey(GoodUID))
                    sellerGoods.Remove(GoodUID);
                GenerateDic();
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
                return false;
            }
        }

        public DB.SellerGoods SetGoodActivity(string GoodUID)
        {
            try
            {
                if (sellerGoods.ContainsKey(GoodUID))
                {
                    DB.SellerGoods good = sellerGoods[GoodUID];
                    good.isActive = !good.isActive;
                    _context.SellerGoods.Update(good);
                    _context.SaveChanges();
                    return good;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
                return null;
            }
        }

        public DB.SellerGoods SetGoodDescription(string description)
        {
            try
            {
                if (string.IsNullOrEmpty(currentItemEdit))
                    return null;
                if (sellerGoods.ContainsKey(currentItemEdit))
                {
                    DB.SellerGoods good = sellerGoods[currentItemEdit];
                    good.Description = description;
                    _context.SellerGoods.Update(good);
                    _context.SaveChanges();
                    return good;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
                return null;
            }
        }

        public DB.SellerGoods SetGoodName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(currentItemEdit))
                    return null;
                if (sellerGoods.ContainsKey(currentItemEdit))
                {
                    DB.SellerGoods good = sellerGoods[currentItemEdit];
                    good.ShopName = name;
                    _context.SellerGoods.Update(good);
                    _context.SaveChanges();
                    return good;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
                return null;
            }
        }

        public DB.SellerGoods SetGoodPhoto(string UID)
        {
            try
            {
                if (string.IsNullOrEmpty(currentItemEdit))
                    return null;
                if (sellerGoods.ContainsKey(currentItemEdit))
                {
                    DB.SellerGoods good = sellerGoods[currentItemEdit];
                    good.MainPhotoUID = UID;
                    _context.SellerGoods.Update(good);
                    _context.SaveChanges();
                    return good;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
                return null;
            }
        }

        private void GenerateDic()           // генерация словаря: UID категории -> список подкатегорий продавца
        {
            parentProductsDic = new Dictionary<string, List<string>>();
            foreach (DB.SellerGoods sellerGood in sellerGoods.Values)       // каждый товар продавца запихиваем в свою категорию
            {
                if (!parentProductsDic.ContainsKey(sellerGood.BasicGoodUID))
                {
                    parentProductsDic.Add(sellerGood.BasicGoodUID, new List<string>() { sellerGood.GoodUID });
                    GenerateDicRecurs(sellerGood.BasicGoodUID);
                }
                else
                    parentProductsDic[sellerGood.BasicGoodUID].Add(sellerGood.GoodUID);
            }
        }
            // рекурсивная часть метода 
        private void GenerateDicRecurs(string categoryUID)
        {
            if (categoryUID == "0                               ")         // базовая категория не имеет родителей
                return;
            string parentUID =BOT.GetBasicProduct(categoryUID).ParentUID;

            if (!parentProductsDic.ContainsKey(parentUID))
            {
                parentProductsDic.Add(parentUID, new List<string>() { categoryUID });
                GenerateDicRecurs(parentUID);
            }
            else
                parentProductsDic[parentUID].Add(categoryUID);
        }

            // подсчёт количества товаров по интересующей категории
        public int GetGoodCountByCategory(string categoryUID)
        {
            int count = 0;
            if (parentProductsDic.Keys.Contains(categoryUID))       // если это категория
            {
                foreach (string UID in parentProductsDic[categoryUID])
                {
                    count += GetGoodCountByCategory(UID);
                }
                return count;
            }
            else return 1;
        }

        //public void AddGood()
        //{
        //    sellerGoods.Add(new DB.SellerGoods() {ui })
        //}
    }
}
