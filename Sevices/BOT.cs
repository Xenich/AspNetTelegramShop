using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramShopAsp.DB;

namespace TelegramShopAsp
{
    public delegate void StateDelegate(string request, User user);
    public delegate void StateDelegateSeller(string request, Seller seller);
    public delegate void StateDelegateBuyer(string request, Buyer buyer);

    public delegate void SendKeyboardDelegate(User user);

    class BOT                                              // todo:  сделать singleton
    {
        public static bool initialized = false;
       // private readonly DB_A566BC_xenichContext _context;

        //public static Dictionary<int, User> usersDic = new Dictionary<int, User>();                         // TelegramId -> user

        static readonly Dictionary<int, Buyer> buyerDic = new Dictionary<int, Buyer>();                       // TelegramId -> buyer
        static readonly Dictionary<int, Seller> sellerDic = new Dictionary<int, Seller>();                    // TelegramId -> seller

        static Dictionary<string, string> countriesDic = new Dictionary<string, string>();           // UID -> country
        static Dictionary<string, string> citiesDic = new Dictionary<string, string>();              // UID -> city

        static readonly Dictionary<string, List<Goods>> parentToProductsDic = new Dictionary<string, List<Goods>>();      // иерархический словарь с продукцией: parent UID (категория) -> список дочерних категорий или товаров
        static readonly Dictionary<string, Goods> basicLeavesDic = new Dictionary<string, Goods>();                   // словарь с базовой продукцией (только листьями): UID -> good
        static readonly Dictionary<string, Goods> allBasicProductsDic = new Dictionary<string, Goods>();                // словарь со всей базовой продукцией (листья + узлы): UID -> good (or category)

        static Dictionary<int, Unit> UnitDic = new Dictionary<int, Unit>();                                    // словарь Id - единица измерения продукции


        public static void Init()
        {
           using (DB_A566BC_xenichContext _context = new DB_A566BC_xenichContext())
            {
                //this._context = _context;
                citiesDic = _context.Cities.OrderBy(c => c.Name).ToDictionary(c => c.UID, c => c.Name);                     // загружаем города
                countriesDic = _context.Countries.OrderBy(c => c.Name).ToDictionary(c => c.UID, c => c.Name);                 // загружаем страны
                                                                                                                              //usersDic = _context.Users.ToDictionary(u => u.TelegramId, u=>new User(u.TelegramId, u.chatId) {name = u.name });            // загружаем всех юзеров
                UnitDic = _context.Unit.ToDictionary(u => u.Id, u => u);                              // загружаем единицы измерения
                                                                                                      //загружаем список поведений - общий, покупателя, продавца

                // загружаем базовую продукцию              
                List<Goods> basicGoodsList = _context.Goods.OrderBy(c => c.Name).Where(g => g.IsBasic && !g.Disabled).ToList();
                foreach (Goods good in basicGoodsList)
                {
                    allBasicProductsDic.Add(good.UID, good);
                }
                Goods nullGood = new Goods() { UID = "0                               " };
                Recurs(basicGoodsList, nullGood);
                Dictionary<string, StateDelegate> UID_State_Dic = new Dictionary<string, StateDelegate>();

                initialized = true;
            }
        }
            // заполнение basicProductsDic и parentProductsDic
        private static void Recurs(List<Goods> allProductList,  Goods currentGood)
        {
            List<Goods> listToDic = new List<Goods>();
            List<Goods> tempList = new List<Goods>();
            foreach (Goods good in allProductList)
            {
                if (good.ParentUID == currentGood.UID)
                {
                    listToDic.Add(good);
                }
                else
                    tempList.Add(good);
            }
            if (listToDic.Count > 0)
                parentToProductsDic.Add(currentGood.UID, listToDic);
            else
                basicLeavesDic.Add(currentGood.UID, currentGood);
            foreach (Goods good in listToDic)
                Recurs(tempList, good);
        }

        public static User GetUser(int telegramId)//, int chatId)
        {
            if (buyerDic.Keys.Contains(telegramId))
                return buyerDic[telegramId];
            if (sellerDic.Keys.Contains(telegramId))
            {
                return sellerDic[telegramId];
            }

            using (DB_A566BC_xenichContext _context = new DB_A566BC_xenichContext())
            {
                User user;
                Buyers buyer = _context.Buyers.FirstOrDefault(b => b.TelegramId == telegramId);
                if (buyer != null && buyer.isActive)            // выбираем тех юзеров, которые в данный момент являются покупателями (хотя могут быть и продавцами)
                {
                    user = Buyer.GetBuyer(buyer);
                    AddUserToDic(telegramId, user);
                    return user;
                }

                Sellers seller = _context.Sellers.FirstOrDefault(s => s.TelegramId == telegramId);
                if (seller != null)
                {
                    user = Seller.GetSeller(seller);
                    AddUserToDic(telegramId, user);
                    return user;
                }

                if (buyer != null)
                {
                    buyer.isActive = true;
                    user = Buyer.GetBuyer(buyer);
                    AddUserToDic(telegramId, user);
                    return user;
                }
                return null;
            }
        }

        private static void AddUserToDic(int telegramId, User user)
        {
            if (user is Buyer)
            {
                buyerDic.Add(telegramId, (Buyer)user);
                return;
            }
            if (user is Seller)
            {
                sellerDic.Add(telegramId, (Seller)user);
                return;
            }
            //usersDic.Add(telegramId, user);
        }

        public static bool ContainsSeller(int telegramId)
        {
            return sellerDic.ContainsKey(telegramId);
        }

        public static bool ContainsBuyer(int telegramId)
        {
            return buyerDic.ContainsKey(telegramId);
        }

        public static void RemoveBuyer(int telegramId)
        {
            buyerDic.Remove(telegramId);
        }

        public static void RemoveSeller(int telegramId)
        {
            sellerDic.Remove(telegramId);
        }

        public static void AddBuyer(int telegramId, Buyer newBuyer)
        {
            buyerDic.Add(telegramId, newBuyer);
        }

        public static void AddSeller(int telegramId, Seller newSeller)
        {
            sellerDic.Add(telegramId, newSeller);
        }

        public static Seller GetSeller(int telegramId)
        {
            return sellerDic[telegramId];
        }

        public static Buyer GetBuyer(int telegramId)
        {
            return buyerDic[telegramId];
        }

        public static bool ContainsCity(string UID)
        {
            return citiesDic.ContainsKey(UID);
        }

        public static bool ContainsCountry(string UID)
        {
            return countriesDic.ContainsKey(UID);
        }

        public static string GetCountryName(string UID)
        {
            return countriesDic[UID];
        }

        public static string[] GetAllCountryUIDs()
        {
            return countriesDic.Keys.ToArray();
        }

        public static string GetCityName(string UID)
        {
            return citiesDic[UID];
        }

        // проверка, является ли UID базовой продукцией (листом)
        public static bool IsBasicLeave(string UID)
        {
            return basicLeavesDic.ContainsKey(UID);
        }

        public static Goods GetBasicLeave(string UID)
        {
            return basicLeavesDic[UID];
        }

        // проверка, является ли productUID категорией, а не товаром
        public static bool IsParent(string UID)
        {
            return parentToProductsDic.ContainsKey(UID);
        }

        public static List<Goods> GetProductsByParent(string UID)
        {
            return parentToProductsDic[UID];
        }

        // проверка на то, является ли productUID базовым продуктом
        public static bool IsBasicProduct(string UID)
        {
            return allBasicProductsDic.ContainsKey(UID);
        }
        public static Goods GetBasicProduct(string UID)
        {
            return allBasicProductsDic[UID];
        }
        // все дочерние элементы категории
        //public Goods[] GetAllBasicProductUIDs()
        //{
        //    return parentToProductsDic.Keys.ToArray();
        //}

        public static string GetUnitShortName(int unitId)
        {
            return UnitDic[unitId].ShortName;
        }



        public static string GetUnitName5(int unitId)
        {
            return UnitDic[unitId].Name5;
        }




    }
}

