using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramShopAsp.DB;
using TelegramShopAsp.Responses;

namespace TelegramShopAsp
{
    interface IFoo { }

    class foo:IFoo
    {
        public foo() { }
    }
    /// <summary>
    /// Эта часть класса описывает общее поведение юзеров
    /// Первые 32 байта приходящего сообщения в inline-режиме - это UID текущего метода
    /// вторые 32 байта - 
    /// </summary>
    partial class Behavior       // вторая часть класса с клавиатурами находится в папке Keyboards, файл KeyboardsForBehavior
    {
        //private static DB_A566BC_xenichContext _context;
        public static void Init()
        {
            //загружаем список поведений - общий, покупателя, продавца
            using (DB_A566BC_xenichContext __context = new DB_A566BC_xenichContext())
            {
                Generate_UIDToState_Dic(__context.States.Where(s => s.UserType == 0).ToDictionary(s => s.UID, s => s.StateName));
                Generate_UIDToState_Dic_Buyer(__context.States.Where(s => s.UserType == 1).ToDictionary(s => s.UID, s => s.StateName));
                Generate_UIDToState_Dic_Seller(__context.States.Where(s => s.UserType == 2).ToDictionary(s => s.UID, s => s.StateName));
            }
        }

        // словарь: имя метода -> метод (Общий случай)
        private static readonly Dictionary<string, StateDelegate> Name_State_Dic = new Dictionary<string, StateDelegate>()
        {
                //  Общее поведение
            { "Hello", Hello },
            { "ChooseUserType", ChooseUserType },
            { "ChooseCountry", ChooseCountry },
            { "ChooseCity", ChooseCity },
            { "EditName", EditName},
            { "EditAdress", EditAdress},
            { "EditPhone", EditPhone},
            { "ChangeCity", ChangeCity},
            { "Help", Help},
        };

            // словарь: имя метода -> метод (покупатель)
        private static readonly Dictionary<string, StateDelegateBuyer> Name_State_Dic_Buyer = new Dictionary<string, StateDelegateBuyer>()
        { 
                // поведение покупателя
            //{ "ChooseUserTypeBuyer", ChooseUserTypeBuyer},
            //{ "ChooseCountryBuyer", ChooseCountryBuyer },
            //{ "ChooseCityBuyer", ChooseCityBuyer },
            { "GotoBuy", GotoBuy },
            { "ChooseProducts", ChooseProducts },
            //{ "EditInfoBuyer", EditInfoBuyer },
            { "ViewCart", ViewCart },
            { "RemooveFromCart", RemooveFromCart },
            { "Checkout", Checkout},
            { "MainMenuBuyer", MainMenuBuyer },
            { "ViewInfoBuyer", ViewInfoBuyer},
            { "MakeCheckout" , MakeCheckout},
            { "EditQuantityBuyer", EditQuantityBuyer},
            { "InputQuantityBuyer", InputQuantityBuyer}
            //{ "EditName" , EditName},
            //{ "MainMenu", MainMenu },
            //{ "AddToCart", AddToCart },
        };

            // словарь: имя метода -> метод (продавец)
        private static readonly Dictionary<string, StateDelegateSeller> Name_State_Dic_Seller = new Dictionary<string, StateDelegateSeller>()
        {
                //поведение продавца
            //{ "ChooseUserTypeSeller", ChooseUserTypeSeller}, 
            //{ "ChooseCountrySeller", ChooseCountrySeller },
            //{ "ChooseCitySeller", ChooseCitySeller },
            { "MainMenuSeller", MainMenuSeller},                            // главное меню продавца
            { "ViewInfoSeller", ViewInfoSeller},                            // даные о компании
            { "GotoStoreFilling", GotoStoreFilling},                        // 
            { "EditCompanyPhones", EditCompanyPhones},                      // 
            { "AddCompanyPhone" , AddCompanyPhone},                         // добавить телефон
            { "InputCompanyPhone", InputCompanyPhone},                      // ввести телефон
            { "DeleteCompany", DeleteCompany},                              // удалить компанию из системы
            { "StopCompany", StopCompany},                                  // приостановить работу компании
            { "CreateGoodDescription" , CreateGoodDescription},             // 
            { "ViewGoodsList", ViewGoodsList},                              // просмотр списка товаров
            { "ChooseProductsSeller", ChooseProductsSeller},                // 
            { "SetGoodActivity", SetGoodActivity},                          // де/активизация товара 
            { "DeleteGood", DeleteGood},                                    // удалить товар
            { "ChooseGoodSeller", ChooseGoodSeller},                        // 
            { "ViewGoodPhotoSeller", ViewGoodPhotoSeller },                 // просмотр фото
            { "AddGoodPhoto", AddGoodPhoto},                                // добавить фото
            { "EditGoodName", EditGoodName},                                // редактирование наименования продукта
            { "EditCompanyDescription", EditCompanyDescription}             // редактировать описание компании
        };

        //private static Dictionary<string, StateDelegate> Name_State_Dic_Seller = new Dictionary<string, StateDelegate>(){};

        public static Dictionary<string, StateDelegate> UIDToState_Dic = new Dictionary<string, StateDelegate>();                // словарь: UID метода -> метод
        public static Dictionary<string, StateDelegateSeller> UIDToState_Seller_Dic = new Dictionary<string, StateDelegateSeller>();         // словарь: UID метода -> метод для продавца
        public static Dictionary<string, StateDelegateBuyer> UIDToState_Buyer_Dic = new Dictionary<string, StateDelegateBuyer>();          // словарь: UID метода -> метод для покупателя

        public static Dictionary<StateDelegate, string> StateToUID_Dic = new Dictionary<StateDelegate, string>();                // словарь: метод-> UID метода
        public static Dictionary<StateDelegateSeller, string> StateToUID_Seller_Dic = new Dictionary<StateDelegateSeller, string>();         // словарь: метод-> UID метода для продавца
        public static Dictionary<StateDelegateBuyer, string> StateToUID_Buyer_Dic = new Dictionary<StateDelegateBuyer, string>();          // словарь: метод-> UID метода для покупателя

            // генерация словарей
        public static void Generate_UIDToState_Dic(Dictionary<string, string> UID_stateDic)       // UID_stateDic - UID метода -> название метода
        {
            //GenerateDic(UID_stateDic, UIDToState_Dic, StateToUID_Dic);

            foreach (string key in UID_stateDic.Keys)
            {
                UIDToState_Dic.Add(key, Name_State_Dic[UID_stateDic[key]]);
                StateToUID_Dic.Add(UIDToState_Dic[key], key);
            }
        }
        public static void Generate_UIDToState_Dic_Seller(Dictionary<string, string> UID_stateDic)       
        {
            foreach (string key in UID_stateDic.Keys)
            {
                UIDToState_Seller_Dic.Add(key, Name_State_Dic_Seller[UID_stateDic[key]]);
                StateToUID_Seller_Dic.Add(UIDToState_Seller_Dic[key], key);
            }
                // добавляем общие методы 
            //foreach(StateDelegate deleg in StateToUID_Dic.Keys) 
            //{
            //    UIDToState_Seller_Dic.Add(StateToUID_Dic[deleg], deleg);
            //}
            UIDToState_Seller_Dic.Add(StateToUID_Dic[ChooseUserType], ChooseUserType);            // UID chooseUserType -> метод chooseUserTypeSeller
            UIDToState_Seller_Dic.Add(StateToUID_Dic[ChooseCountry], ChooseCountry);
            UIDToState_Seller_Dic.Add(StateToUID_Dic[ChooseCity], ChooseCity);
            UIDToState_Seller_Dic.Add(StateToUID_Dic[EditName], EditName);
            UIDToState_Seller_Dic.Add(StateToUID_Dic[EditAdress], EditAdress);
            UIDToState_Seller_Dic.Add(StateToUID_Dic[EditPhone], EditPhone);
            UIDToState_Seller_Dic.Add(StateToUID_Dic[ChangeCity], ChangeCity);
            UIDToState_Seller_Dic.Add(StateToUID_Dic[Help], Help);

        }
        public static void Generate_UIDToState_Dic_Buyer(Dictionary<string, string> UID_stateDic)
        {
            //GenerateDic(UID_stateDic, UIDToState_Buyer_Dic, StateToUID_Buyer_Dic);
            foreach (string key in UID_stateDic.Keys)
            {
                UIDToState_Buyer_Dic.Add(key, Name_State_Dic_Buyer[UID_stateDic[key]]);
                StateToUID_Buyer_Dic.Add(UIDToState_Buyer_Dic[key], key);
            }
                // добавляем общие методы
            UIDToState_Buyer_Dic.Add(StateToUID_Dic[ChooseUserType], ChooseUserType);            // UID chooseUserType -> метод chooseUserTypeSeller
            UIDToState_Buyer_Dic.Add(StateToUID_Dic[ChooseCountry], ChooseCountry);
            UIDToState_Buyer_Dic.Add(StateToUID_Dic[ChooseCity], ChooseCity);
            UIDToState_Buyer_Dic.Add(StateToUID_Dic[EditName], EditName);
            UIDToState_Buyer_Dic.Add(StateToUID_Dic[EditAdress], EditAdress);
            UIDToState_Buyer_Dic.Add(StateToUID_Dic[EditPhone], EditPhone);
            UIDToState_Buyer_Dic.Add(StateToUID_Dic[ChangeCity], ChangeCity);
            UIDToState_Buyer_Dic.Add(StateToUID_Dic[Help], Help);
        }
        private static void GenerateDic(Dictionary<string, string> UID_stateDic,  Dictionary<string, StateDelegate> dic_UID_State_ToGenerate, Dictionary<StateDelegate, string> State_UID_Dic_ToGenerate)
        {
            foreach (string key in UID_stateDic.Keys)
            {
                dic_UID_State_ToGenerate.Add(key, Name_State_Dic[UID_stateDic[key]]);
                State_UID_Dic_ToGenerate.Add(dic_UID_State_ToGenerate[key], key);
            }
        }

//**********************************************************************************************************************************************************************
//**********************************************************************************************************************************************************************

        public static void Hello(string s, User user)
        {
            SendKeyboardFor_ChooseUserType(StateToUID_Dic[Hello], user);
            user.CurrentState = ChooseUserType;
        }

        public static void Help(string returnUID, User user)
        {
            user.CurrentState = Help;


            if (user is Buyer)
            {
                SendKeyboardFor_HelpBuyer(returnUID, user as Buyer);
            }
            if (user is Seller)
            {
                SendKeyboardFor_HelpSeller(returnUID, user as Seller);
            }
        }

        public static void ChooseUserType(string chooseUserType, User user)
        {
            user.ClearMessagesWithKeyboardsList();      // делаем это сейчас, потому что потом юзера удалим
            User newUser = null;
            switch (chooseUserType)
            {
                case ("buyer"):
                    if (BOT.ContainsBuyer(user.telegramId))
                        newUser = BOT.GetBuyer(user.telegramId);
                    else
                    {
                        try
                        {
                            using (DB_A566BC_xenichContext _context = new DB_A566BC_xenichContext())
                            {
                                DB.Buyers buyer = _context.Buyers.FirstOrDefault(b => b.TelegramId == user.telegramId);
                                if (buyer != null)
                                    newUser = Buyer.GetBuyer(buyer);
                                else
                                {
                                    newUser = new Buyer(user.telegramId);
                                    buyer = new DB.Buyers()
                                    {
                                        isActive = true,
                                        TelegramId = user.telegramId,
                                        StateUID = StateToUID_Dic[ChooseCountry]
                                    };
                                    _context.Buyers.Add(buyer);
                                    _context.SaveChanges();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.Handle(ex.Message);
                        }
                        BOT.AddBuyer(newUser.telegramId, newUser as Buyer);
                    }
                    if (BOT.ContainsSeller(newUser.telegramId))
                        BOT.RemoveSeller(newUser.telegramId);
                    break;
                case "seller":
                    if (BOT.ContainsSeller(user.telegramId))
                        newUser = BOT.GetSeller(user.telegramId);
                    else
                    {
                        try
                        {
                            using (DB_A566BC_xenichContext _context = new DB_A566BC_xenichContext())
                            {
                                DB.Sellers seller = _context.Sellers.FirstOrDefault(b => b.TelegramId == user.telegramId);
                                if (seller != null)
                                    newUser = Seller.GetSeller(seller);
                                else
                                {
                                    newUser = new Seller(user.telegramId);
                                    seller = new DB.Sellers()
                                    {
                                        TelegramId = user.telegramId,
                                        StateUID = StateToUID_Dic[ChooseCountry]
                                    };
                                    _context.Sellers.Add(seller);

                                    DB.Buyers buyer = _context.Buyers.FirstOrDefault(b => b.TelegramId == user.telegramId);     // делаем неактивным покупателя с таким же id (если он существует)
                                    if (buyer != null)
                                        buyer.isActive = false;

                                    _context.SaveChanges();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.Handle(ex.Message);
                        }
                        BOT.AddSeller(newUser.telegramId, newUser as Seller);
                    }
                    if (BOT.ContainsBuyer(newUser.telegramId))
                    {
                        BOT.RemoveBuyer(newUser.telegramId);
                    }
                    break;
                default:                                     
                    SendKeyboardFor_ChooseUserType(StateToUID_Dic[Hello],user );
                    return;
            }

            if (string.IsNullOrEmpty(newUser.cityUID) || (!(newUser is Buyer) && !(newUser is Seller)))
            {
                SendKeyboardFor_ChooseCountry(StateToUID_Dic[ChooseUserType], newUser);
                newUser.CurrentState = ChooseCountry;
            }
            else
            {
                if (newUser is Buyer)
                {
                    SendKeyboardFor_MainMenuBuyer("none", newUser as Buyer);
                    (newUser as Buyer).CurrentState = MainMenuBuyer;
                }
                if (newUser is Seller)
                {
                    SendKeyboardFor_MainMenuSeller("none", newUser as Seller);
                    (newUser as Seller).CurrentState = MainMenuSeller;

                }               
            }
        }

        public static void ChooseCountry(string chooseCountry, User user)
        {
            if (user.currentCallback_query_id != "" && BOT.ContainsCountry(chooseCountry))
            {                   
                SendKeyboardFor_ChooseCity(user, StateToUID_Dic[ChooseCountry], chooseCountry);
                user.CurrentState = ChooseCity;
            }
            else
            {                   
                SendKeyboardFor_ChooseCountry(StateToUID_Dic[ChooseUserType], user );
            }             
        }

        public static void ChooseCity(string choosedCityUID, User user)
        {
            if (BOT.ContainsCity(choosedCityUID))       // юзер выбрал свой город, переходим в главное меню
            {                  
                user.SetCity(choosedCityUID);
                if (user is Buyer)
                {
                    SendKeyboardFor_MainMenuBuyer(StateToUID_Dic[ChooseCity], user as Buyer);// as User );
                    (user as Buyer).CurrentState = MainMenuBuyer;
                }
                if (user is Seller)
                {
                    SendKeyboardFor_MainMenuSeller(StateToUID_Dic[ChooseCity], user as Seller);
                    (user as Seller).CurrentState = MainMenuSeller;
                }
            }
            else
            {
                SendKeyboardFor_ChooseCountry(StateToUID_Dic[ChooseUserType], user );
                user.CurrentState = ChooseCountry;
            }
        }


        public static void EditName(string none, User user)
        {
            if (user is Buyer)
            {
                (user as Buyer).CurrentState = NameEdited;  
                SendSimpleMessage("Введите ваше имя: ", user);
            }
            if (user is Seller)
            {
                (user as Seller).CurrentState = NameEdited;   
                SendSimpleMessage("Введите наименование компании: ", user);
            }
        }

            // метода нет в словаре!
        public static void NameEdited(string name, User user)
        {
            user.SetName(name);
            if (user is Buyer)
            {
                (user as Buyer).CurrentState = ViewInfoBuyer;
                SendKeyboardFor_EditInfoBuyer("", user as Buyer);
            }
            if (user is Seller)
            {
                (user as Seller).CurrentState = ViewInfoSeller;
                SendKeyboardFor_EditInfoSeller("", user as Seller);
            }
        }


        public static void EditPhone(string none, User user)
        {
            if (user is Buyer)
            {
                (user as Buyer).CurrentState = PhoneEdited;             
                SendSimpleMessage("Введите контактный телефон: ", user);
            }
            if (user is Seller)
            {
                SendKeyboardFor_EditCompanyPhones(StateToUID_Seller_Dic[ViewInfoSeller], user as Seller);
            }              
        }
            // метода нет в словаре!
        public static void PhoneEdited(string phone, User user)
        {
            if (user is Buyer)
            {
                (user as Buyer).SetPhone(phone);
                (user as Buyer).CurrentState = ViewInfoBuyer;
                SendKeyboardFor_EditInfoBuyer("", user as Buyer);
            }
            if (user is Seller)
            {
                // у продавца своя логика
            }
        }

            // метода нет в словаре!
        public static void EditAdress(string none, User user)
        {
            if (user is Buyer)
            {
                (user as Buyer).CurrentState = AddressEdited;
                SendSimpleMessage("Введите адрес доставки:", user);
            }
            if (user is Seller)
            {
                (user as Seller).CurrentState = AddressEdited;
                SendSimpleMessage("Введите адрес компании:", user);
            }
        }

            // метода нет в словаре!
        public static void AddressEdited(string adress, User user)
        {
            user.SetAdress(adress);
            if (user is Buyer)
            {
                (user as Buyer).CurrentState = ViewInfoBuyer;
                SendKeyboardFor_EditInfoBuyer("", user as Buyer);
            }
            if (user is Seller)                                                       
            {
                (user as Seller).CurrentState = ViewInfoSeller;
                SendKeyboardFor_EditInfoSeller("", user as Seller);
            }
        }

        public static void ChangeCity(string none, User user)
        {
            user.CurrentState = ChooseCountry; 
            if (user is Buyer)
                SendKeyboardFor_ChooseCountry(StateToUID_Buyer_Dic[ViewInfoBuyer], user);
            if (user is Seller)
                SendKeyboardFor_ChooseCountry(StateToUID_Seller_Dic[ViewInfoSeller], user);
        }
    }
}
