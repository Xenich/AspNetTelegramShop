using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramShopAsp.Classes;
using TelegramShopAsp.Responses;

namespace TelegramShopAsp
{
        // набор клавиатур ПРОДАВЦА
    partial class Behavior                // вторая часть класса с поведениями находится в папке Behaviors
    {
        private static void SendKeyboardFor_MainMenuSeller(string returnUID, Seller seller)
        {
            try
            {
                string token = StateToUID_Seller_Dic[MainMenuSeller];
                string message = "\U0001F3E0 *Главное меню*" + Environment.NewLine + Environment.NewLine;
                message += "Наименование магазина: " + (string.IsNullOrEmpty(seller.name)? "-" : seller.name) + Environment.NewLine;
                message += "Товаров: " + seller.sellerGoods.Count.ToString() + " шт." + Environment.NewLine;
                InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
                if (!seller.isActive)
                {
                    message += Environment.NewLine + "\U00002757 Внимание! Работа магазина приостановлена. Покупатели не видят ваш товар.";
                    ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U00002757 Возобновить работу магазина", token + StateToUID_Seller_Dic[StopCompany]) });
                }
                else
                    ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("Приостановить работу магазина", token + StateToUID_Seller_Dic[StopCompany]) });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U0000270F Редактировать информацию о компании", token + StateToUID_Seller_Dic[ViewInfoSeller]) });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("Перейти к наполнению магазина товарами", token + StateToUID_Seller_Dic[GotoStoreFilling]) });
                if(seller.sellerGoods.Count>0)
                    ikbm.Addrow(new List<InlineKbrdBtn>() { ViewGoodsListButton() });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U0000274C Удалить компанию из системы", token + StateToUID_Seller_Dic[DeleteCompany]) });  
                if(returnUID!="none")
                    ikbm.Addrow(new List<InlineKbrdBtn>() { BackButton(token, returnUID) });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("Сброс и переход в начало", token + StateToUID_Dic[ChooseUserType]) });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("Помощь", token + StateToUID_Dic[Help]) });
                SendMessageWithInlineKeyBoard(seller, message, ikbm);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }

        private static void SendKeyboardFor_HelpSeller(string returnUID, Seller seller)
        {
            string message = "Задать вопрос созателям: https://www.facebook.com/dmitry.leechman";
            InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
            ikbm.Addrow(new List<InlineKbrdBtn>() { MainMenuSellerButton(StateToUID_Dic[Help]) });     // в гл. меню
            SendMessageWithInlineKeyBoard(seller, message, ikbm);
        }

            // редактирование данных о компании
        private static void SendKeyboardFor_EditInfoSeller(string none, Seller seller)
        {
            try
            {
                string message = "*Даные о компании:*" + Environment.NewLine + Environment.NewLine;
                message += "Наименование: " + seller.name + Environment.NewLine + Environment.NewLine;
                message += "Контактные телефоны:" + Environment.NewLine;
                foreach (string phone in seller.telephones)
                {
                    message += phone + Environment.NewLine;
                }
                message += Environment.NewLine + "Адрес: " + "г. " + BOT.GetCityName(seller.cityUID) + ", " +  seller.adress + Environment.NewLine + Environment.NewLine;
                message += "Описание: " + seller.description + Environment.NewLine + Environment.NewLine;
                InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();

                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U0000270F Изменить наименование", StateToUID_Dic[EditName] + "none") });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U0000270F Изменить контактные телефоны", StateToUID_Dic[EditPhone] + "none") });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U0000270F Изменить город", StateToUID_Dic[ChangeCity] + "none") }); 
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U0000270F Изменить адрес", StateToUID_Dic[EditAdress] + "none") });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U0000270F Изменить описание", StateToUID_Seller_Dic[EditCompanyDescription] + "none") }); 
                ikbm.Addrow(new List<InlineKbrdBtn>() { MainMenuSellerButton(StateToUID_Seller_Dic[ViewInfoSeller]) });     // в гл. меню
                SendMessageWithInlineKeyBoard(seller, message, ikbm);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }
            
            // редактирование списка телефонов
        private static void SendKeyboardFor_EditCompanyPhones(string returnUID, Seller seller)
        {
            try
            {
                string token = StateToUID_Seller_Dic[EditCompanyPhones];
                string message;
                if (seller.telephones.Count>0)
                    message = "Удалите телефон или добавьте новый" + Environment.NewLine + Environment.NewLine;
                else
                    message = "Добавьте контактный телефон" + Environment.NewLine + Environment.NewLine;
                InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
                foreach (string phone in seller.telephones)
                {
                    ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn(phone + "   \U0000274C удалить", token + phone) });
                }
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("Добавить телефон", token + StateToUID_Seller_Dic[InputCompanyPhone]) });
                ikbm.Addrow(new List<InlineKbrdBtn>() { MainMenuSellerButton(StateToUID_Seller_Dic[ViewInfoSeller]) });    // в гл. меню
                BackButton(token, returnUID);
                SendMessageWithInlineKeyBoard(seller, message, ikbm);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }

        private static void SendKeyboardFor_CompanyDescriptionEdited(string returnUID, Seller seller)
        {
            try
            {
                string message;
                message = "Опишите свою компанию :" + Environment.NewLine + Environment.NewLine;
                InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
                ikbm.Addrow(new List<InlineKbrdBtn>() { BackButton(StateToUID_Seller_Dic[ViewInfoSeller], "none") });
                SendMessageWithInlineKeyBoard(seller, message, ikbm);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }

        // Выбор продукции в категории для добавления в магазин
        private static void SendKeyboardFor_ChooseProductsSeller(string goodUID, Seller seller)
        {
            try
            {
                string token = StateToUID_Seller_Dic[ChooseProductsSeller];
                string message = "*Добавление товара*" + Environment.NewLine + Environment.NewLine;
                if (goodUID == "0                               ")
                    message += "Выберите категорию, в которую вы хотите добавить товар:";
                else
                    message += "Категория: " + BOT.GetBasicProduct(goodUID).Name;
                InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();

                if (goodUID != "0                               ")
                {
                    string parentUID = BOT.GetBasicProduct(goodUID).ParentUID;
                    if (parentUID != "0                               ")    // если это не прямой потомок корневой категории
                    {
                        ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U00002B06 Вернуться к категории " + BOT.GetBasicProduct(goodUID).Name, token + parentUID) });
                    }
                }

                foreach (DB.Goods good in BOT.GetProductsByParent(goodUID))        // перебираем все дочерние элементы если categoryUID их имеет (если это категория)
                {
                    if (BOT.IsParent(good.UID))
                        ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn(good.Name + " \U000027A1", token + good.UID) });              // если это категория
                    else                   
                        ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn(good.Name + " \U000027A1", token + good.UID) });              // если это товар
                }

                if (goodUID == "0                               ")                       // если тут токен корневого элемента, то мы в самом начале выбора категорий
                {
                    if (seller.sellerGoods.Count > 0)           // если список товаров продавца не пуст - добавляем кнопку просмотра товаров
                        ikbm.Addrow(new List<InlineKbrdBtn>() { ViewGoodsListButton() });
                    ikbm.Addrow(new List<InlineKbrdBtn>() { MainMenuSellerButton(StateToUID_Seller_Dic[GotoStoreFilling]) });
                }
                else                                                                    // иначе - мы в какой-то дочерней категории
                {
                    ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U00002B05 К началу выбора категорий", token + "0                               ") });
                    if (seller.sellerGoods.Count > 0)           // если список товаров продавца не пуст - добавляем кнопку просмотра товаров
                        ikbm.Addrow(new List<InlineKbrdBtn>() { ViewGoodsListButton() });
                }

                SendMessageWithInlineKeyBoard(seller, message, ikbm);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }

            // Редактирование карточки товара
        private static void SendKeyboardFor_EditGoodCard(DB.SellerGoods good, Seller seller)
        {
            seller.CurrentState = CreateGoodDescription;
            seller.currentItemEdit = good.GoodUID;          
            string message = "*Карточка товара*" + Environment.NewLine + Environment.NewLine + "*Наименование:* ";
            if (string.IsNullOrEmpty(good.ShopName))
                message += BOT.GetBasicLeave(good.BasicGoodUID).Name + Environment.NewLine;
            else
                message += good.ShopName + Environment.NewLine;
            message += "*Категория:* " + BOT.GetBasicLeave(good.BasicGoodUID).Name + Environment.NewLine;
            message += "*Описание:* " + (string.IsNullOrEmpty(good.Description)?"-": good.Description) + Environment.NewLine;
            if (!good.isActive)
                message += "\U00002757 Товар не активен. " + Environment.NewLine;
            message += Environment.NewLine + "Напечатайте краткое описание товара \U0000270F или выполните следующие действия:" + Environment.NewLine;

            InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
            ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("Изменить наименование", StateToUID_Seller_Dic[EditGoodName] + good.GoodUID) }); 
            if(string.IsNullOrEmpty(good.MainPhotoUID))
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("Добавить фотографию", StateToUID_Seller_Dic[AddGoodPhoto] + good.GoodUID) });
            else
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("Изменить фото", StateToUID_Seller_Dic[AddGoodPhoto] + good.GoodUID) , new InlineKbrdBtn("Просмотр фото", StateToUID_Seller_Dic[ViewGoodPhotoSeller] + good.GoodUID) });
            if (!good.isActive)
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U00002757 Активировать товар", StateToUID_Seller_Dic[SetGoodActivity] + good.GoodUID) });
            else
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U00002796 Деактивировать товар", StateToUID_Seller_Dic[SetGoodActivity] + good.GoodUID) });
            ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U0000274C Удалить товар из магазина", StateToUID_Seller_Dic[DeleteGood] + good.GoodUID) });
            ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U00002B06 К просмотру в: "  + BOT.GetBasicProduct(BOT.GetBasicProduct(good.BasicGoodUID).ParentUID).Name, StateToUID_Seller_Dic[ViewGoodsList]        + BOT.GetBasicProduct(good.BasicGoodUID).ParentUID) });               // перескакиваем через одну категорию
            ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U00002B05 К добавлению в: " + BOT.GetBasicProduct(BOT.GetBasicProduct(good.BasicGoodUID).ParentUID).Name, StateToUID_Seller_Dic[ChooseProductsSeller] + BOT.GetBasicProduct(good.BasicGoodUID).ParentUID) });       // перескакиваем через одну категорию
            ikbm.Addrow(new List<InlineKbrdBtn>() { ViewGoodsListButton() });
            SendMessageWithInlineKeyBoard(seller, message, ikbm);
        }

        private static void SendKeyboardFor_ViewPhoto(string goodUID, Seller seller)
        {
            DB.SellerGoods good = seller.sellerGoods[goodUID];
            seller.CurrentState = ChooseGoodSeller;
            seller.currentItemEdit = good.GoodUID;
            string message = "*Фото*. " + good.ShopName;
            InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
            ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U00002B05 Вернуться к товару", StateToUID_Seller_Dic[ChooseGoodSeller] + good.GoodUID) });
            SendMessageWithPhotoAndInlineKeyBoard(seller, message, good.MainPhotoUID, ikbm);
        }

            // Выбор товаров для просмотра карточки;  parentUID - 100% какой-то базовый товар
        private static void SendKeyboardFor_ViewGoodsList(string parentUID, Seller seller)
        {
            string token = StateToUID_Seller_Dic[ViewGoodsList];
            string message = "*Просмотр товаров магазина*" + Environment.NewLine ;
            if (parentUID != "0                               ")
            {
                message += "*Категория:* " + BOT.GetBasicProduct(parentUID).Name + Environment.NewLine + Environment.NewLine;
                message += "Выберите категорию для просмотра карточки товара или вернитесь на категорию выше" + Environment.NewLine;
            }
            else
                message += "Выберите категорию для просмотра карточки товара";
            InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
            foreach (string categoryUID in seller.parentProductsDic[parentUID])        // перебираем все дочерние элементы если categoryUID их имеет (если это категория)
            {
                if (seller.parentProductsDic.ContainsKey(categoryUID))                              // если это какая-то базовая продукция
                    ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn(BOT.GetBasicProduct(categoryUID).Name + " (" + seller.GetGoodCountByCategory(categoryUID).ToString() +" шт.)"+ " \U000027A1", token + categoryUID) });
                else                                                                                // если это товар магазина - переходим к просмотру карточки
                {
                    string name = string.IsNullOrEmpty(seller.sellerGoods[categoryUID].ShopName) ? BOT.GetBasicProduct(seller.sellerGoods[categoryUID].BasicGoodUID).Name : seller.sellerGoods[categoryUID].ShopName;  
                    ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn((seller.sellerGoods[categoryUID].isActive ? "" : "\U00002757 ") + name, StateToUID_Seller_Dic[ChooseGoodSeller] + categoryUID) });
                }
            }
            if (parentUID != "0                               " && BOT.IsBasicProduct(BOT.GetBasicProduct(parentUID).ParentUID))
            {
                DB.Goods parentParent = BOT.GetBasicProduct(BOT.GetBasicProduct(parentUID).ParentUID);
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U00002B06 Вернуться к категории " + parentParent.Name, token + parentParent.UID) });
            }
            if (parentUID != "0                               ")
            {
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U00002B05 Вернуться в начальную категорию", token + "0                               ") });
            }
            ikbm.Addrow(new List<InlineKbrdBtn>() { MainMenuSellerButton(token) });
            SendMessageWithInlineKeyBoard(seller, message, ikbm);
        }

        private static void SendNotimplementedKeyboardSeller(string returnUID, Seller seller)
        {
            string message = "Не реализовано";
            InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
            ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("В главное меню", StateToUID_Seller_Dic[MainMenuSeller] + returnUID) });
            SendMessageWithInlineKeyBoard(seller, message, ikbm);
        }

// **************************************************************************************************************
// *******************      Готовые кнопки      ************************



            // Кнопка возвращения в главное меню продавца
        private static InlineKbrdBtn MainMenuSellerButton(string currentToken)
        {
            return new InlineKbrdBtn("\U0001F3E0 В главное меню", currentToken + StateToUID_Seller_Dic[MainMenuSeller]);
        }

            // кнопка просмотра товаров
        private static InlineKbrdBtn ViewGoodsListButton()
        {
            return new InlineKbrdBtn("\U0001F440 \U00002B05 К просмотру товаров магазина", StateToUID_Seller_Dic[ViewGoodsList] + "0                               ");
        }

    }
}