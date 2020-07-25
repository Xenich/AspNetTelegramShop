using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramShopAsp.Classes;
using TelegramShopAsp.DB;
using TelegramShopAsp.Responses;

namespace TelegramShopAsp
{
        // набор клавиатур ПОКУПАТЕЛЯ
    partial class Behavior                // вторая часть класса с поведениями находится в папке Behaviors
    {
            // Главное меню
        private static void SendKeyboardFor_MainMenuBuyer(string returnUID, Buyer buyer)
        {
            try
            {
                string token = StateToUID_Buyer_Dic[MainMenuBuyer];
                string message = "\U0001F3E0 *Главное меню*";
                InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("Перейти к покупкам", token + StateToUID_Buyer_Dic[GotoBuy]) });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U0000270F Редактировать информацию о себе", token + StateToUID_Buyer_Dic[ViewInfoBuyer]) });
                if(buyer.cart.Count>0)
                    ikbm.Addrow(new List<InlineKbrdBtn>() { CartButton(buyer, token) });
                if (returnUID != "none")
                    ikbm.Addrow(new List<InlineKbrdBtn>() { BackButton(token, returnUID) });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("Сброс и переход в начало", token + StateToUID_Dic[ChooseUserType]) });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("Помощь", token + StateToUID_Dic[Help]) });
                SendMessageWithInlineKeyBoard(buyer, message, ikbm);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }

        private static void SendKeyboardFor_HelpBuyer(string returnUID, Buyer buyer)
        {
            string message = "Задать вопрос созателям: https://www.facebook.com/dmitry.leechman";
            InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
            ikbm.Addrow(new List<InlineKbrdBtn>() { MainMenuBuyerButton(StateToUID_Dic[Help]) });     // в гл. меню
            SendMessageWithInlineKeyBoard(buyer, message, ikbm);
        }

        // Выбор продукции в категории 
        private static void SendKeyboardFor_ChooseProducts(string categoryUID,  Buyer buyer)
        {
            try
            {
                string token = StateToUID_Buyer_Dic[ChooseProducts];
                string message = Environment.NewLine + "Выберите товары из списка";
                if (categoryUID != "0                               ")
                    message += " или вернитесь на категорию выше:";

                InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
                foreach (DB.Goods good in BOT.GetProductsByParent(categoryUID))        // перебираем все дочерние элементы если productUID их имеет (если это категория)
                {
                    if (BOT.IsParent(good.UID))      // если это категория
                        ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn(good.Name + " \U000027A1", token + good.UID) });
                    else                   // если это товар
                    {
                        if (buyer.cart.Keys.Contains(good))
                            ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn(good.Name + " \U00002705", token + good.UID) });
                        else
                            ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn(good.Name + " \U00002795", token + good.UID) });
                    }
                }

                if (categoryUID == "0                               ")                       // если тут токен корневого элемента, то мы в самом начале выбора категорий
                    ikbm.Addrow(new List<InlineKbrdBtn>() { MainMenuBuyerButton(StateToUID_Buyer_Dic[GotoBuy]) });
                else        // иначе - мы в какой-то дочерней категории
                {
                    string parentUID = BOT.GetBasicProduct(categoryUID).ParentUID;
                    if (parentUID != "0                               ")    // если это не прямой потомок корневой категории
                    {
                        ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U00002B06 Вернуться к категории " + BOT.GetBasicProduct(parentUID).Name , token + parentUID) });
                    }
                    ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U00002B05 К началу выбора категорий", token + "0                               ") });
                }
                if (buyer.cart.Count > 0)           // если корзина не пуста - добавляем кнопку просмотр корзины
                    ikbm.Addrow(new List<InlineKbrdBtn>() { CartButton(buyer, token) });
                SendMessageWithInlineKeyBoard(buyer, message, ikbm);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }

            // Просмотр корзины
        private static void SendKeyboardFor_RemooveFromCart(string none,  Buyer buyer)
        {
            try
            {
                string token = StateToUID_Buyer_Dic[RemooveFromCart];
                string message;
                if (buyer.cart.Count == 0)
                    message = Environment.NewLine + "Ваша корзина пуста" + Environment.NewLine;
                else
                    message = Environment.NewLine + "Ваш список покупок:" + Environment.NewLine;
                InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
                foreach (DB.Goods good in buyer.cart.Keys)
                {
                    ikbm.Addrow(new List<InlineKbrdBtn>() { CartGoodButton(buyer, good), new InlineKbrdBtn("\U0000274C Убрать", token + good.UID) });
                }
                if (buyer.cart.Count > 0)
                    ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("Перейти к оформлению заказа", token + StateToUID_Buyer_Dic[Checkout]) });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U00002B05 К выбору категорий", StateToUID_Buyer_Dic[ChooseProducts] + "0                               ") });
                ikbm.Addrow(new List<InlineKbrdBtn>() { MainMenuBuyerButton(StateToUID_Buyer_Dic[ViewCart]) });
                SendMessageWithInlineKeyBoard(buyer, message, ikbm);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }

            // Просмотр и редактирование информации о покупателе
        private static void SendKeyboardFor_EditInfoBuyer(string returnUID, Buyer buyer)
        {
            try
            {
                string token = StateToUID_Buyer_Dic[ViewInfoBuyer];
                string message = "*Личные данные*" + Environment.NewLine + Environment.NewLine;
                message += "Имя: " + buyer.name + Environment.NewLine + Environment.NewLine;
                message += "Контактный телефон: " + (buyer as Buyer).telephone + Environment.NewLine + Environment.NewLine;
                message += "Адрес доставки: " + "г. " + BOT.GetCityName(buyer.cityUID) +", "+ buyer.adress + Environment.NewLine + Environment.NewLine;
                InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U0000270F Изменить имя", StateToUID_Dic[EditName] + "none") });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U0000270F Изменить контактный телефон", StateToUID_Dic[EditPhone] + "none") });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U0000270F Изменить адрес доставки", StateToUID_Dic[EditAdress] + "none") });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U0000270F Изменить город", StateToUID_Dic[ChangeCity] + "none") });
                if ((buyer as Buyer).cart.Count > 0)
                {
                    ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("Перейти к оформлению заказа", token + StateToUID_Buyer_Dic[Checkout]) });
                }
                ikbm.Addrow(new List<InlineKbrdBtn>() { MainMenuBuyerButton(token) });
                SendMessageWithInlineKeyBoard(buyer, message, ikbm);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }


            // Перейти к оформлению
        private static void SendKeyboardFor_MakeCheckout(string none,  Buyer buyer)
        {
            try
            {
                string token = StateToUID_Buyer_Dic[Checkout];
                InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
                StringBuilder sb = new StringBuilder();
                sb.Append("*Оформление заказа*" + Environment.NewLine +Environment.NewLine);
                sb.Append("*Адрес доставки: *" + buyer.adress + Environment.NewLine);
                sb.Append("*Контактный телефон: *" + (buyer as Buyer).telephone + Environment.NewLine + Environment.NewLine);
                sb.Append("*Корзина (" + buyer.cart.Count.ToString() + " шт.):*" + Environment.NewLine + Environment.NewLine);
                int counter = 1;
                foreach (DB.Goods good in buyer.cart.Keys)
                {
                    sb.Append(counter.ToString() + ") " + good.Name + ((buyer.cart[good] == "") ? "" : " - " + buyer.cart[good] + " " + BOT.GetUnitShortName(good.UnitId)) + Environment.NewLine);
                    counter++;
                }
                sb.Append(Environment.NewLine + "Указать количество можно сейчас или при обсуждении заказа с продавцом. Для изменения количества нажмите соответствующую кнопку с названием товара" + Environment.NewLine);
                foreach (DB.Goods good in buyer.cart.Keys)
                {
                    ikbm.Addrow(new List<InlineKbrdBtn>() { CartGoodButton(buyer, good) });
                }
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U0001F3C1 Заказ подтверждаю", token + StateToUID_Buyer_Dic[MakeCheckout]),
                                                        MainMenuBuyerButton(token)});
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U00002B05 К выбору категорий", StateToUID_Buyer_Dic[ChooseProducts] + "0                               ") });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U0000270F Изменить контактные данные", token + StateToUID_Buyer_Dic[ViewInfoBuyer]) });
                
                SendMessageWithInlineKeyBoard(buyer, sb.ToString(), ikbm);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }

        private static void SendKeyboardFor_InputQuantityBuyer(string goodUID, Buyer buyer)
        {
            try
            {
                string token = StateToUID_Buyer_Dic[InputQuantityBuyer];
                if (BOT.IsBasicLeave(goodUID))
                {
                    DB.Goods good = BOT.GetBasicLeave(goodUID);
                    string message = "Введите необходимое количество (" + BOT.GetUnitName5(good.UnitId) + ") для товара: " + good.Name +   " или вернитесь к списку покупок";
                    InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
                    ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U00002B05 Вернуться к списку покупок", token + StateToUID_Buyer_Dic[Checkout]) });
                    SendMessageWithInlineKeyBoard(buyer, message, ikbm);

                        //  формируем Reply Keyboard
                    //ReplyKeyboardMarkup rkbm = new ReplyKeyboardMarkup();
                    //rkbm.Addrow(new List<KbrdBtn>() { new KbrdBtn("1"), new KbrdBtn("2"), new KbrdBtn("3") });
                    //rkbm.Addrow(new List<KbrdBtn>() { new KbrdBtn("4"), new KbrdBtn("5"), new KbrdBtn("6") });
                    //rkbm.Addrow(new List<KbrdBtn>() { new KbrdBtn("7"), new KbrdBtn("8"), new KbrdBtn("9") });
                    //rkbm.Addrow(new List<KbrdBtn>() { new KbrdBtn("0"), new KbrdBtn(","), new KbrdBtn(" ") });
                    //TelegramRequest.SendMessage("1", buyer.chatId, rkbm);
                }
                else
                {
                    SendKeyboardFor_MakeCheckout("", buyer);
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }

        private static void SendKeyboard_MakeOffer(Buyer buyer)      // у клавиатуры нет метода
        {
            try
            {
                string message = "Заказ сделан! Как только я найду подходящего продавца, я вам сообщу";
                InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
                ikbm.Addrow(new List<InlineKbrdBtn>() { MainMenuBuyerButton("0                               ") });
                SendMessageWithInlineKeyBoard(buyer, message, ikbm);

            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }



//*************************************************************************************************************
        private static void SendNotimplementedKeyboardBuyer(string returnUID,  Buyer buyer)
        {
            string message = "Не реализовано";
            InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
            ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("\U0001F3E0 В Главное меню", StateToUID_Buyer_Dic[MainMenuBuyer] + "none") });
            SendMessageWithInlineKeyBoard(buyer, message, ikbm);
        }

// **************************************************************************************************************
// *******************      Готовые кнопки      ************************

            // кнопка корзины
        private static InlineKbrdBtn CartButton(Buyer buyer, string token)
        {
            int count = buyer.cart.Keys.Count;
            return new InlineKbrdBtn("\U0001F440 \U00002B05 Просмотреть корзину" + ((count > 0) ? "(" + count + " шт." + ")" : ""), token + StateToUID_Buyer_Dic[ViewCart]);
        }

            // Кнопка возвращения в главное меню покупателя
        private static InlineKbrdBtn MainMenuBuyerButton(string currentToken)
        {
            return new InlineKbrdBtn("\U0001F3E0 В главное меню", currentToken +  StateToUID_Buyer_Dic[MainMenuBuyer]);
        }

            // Кнопка товара в корзине
        private static InlineKbrdBtn CartGoodButton(Buyer buyer, DB.Goods good)
        {
            return new InlineKbrdBtn(good.Name + ((buyer.cart[good] == "") ? "" : " - " + buyer.cart[good] + " " + BOT.GetUnitShortName(good.UnitId)), StateToUID_Buyer_Dic[EditQuantityBuyer] + good.UID) ;
        }

            // кнопка возврата назад
        private static InlineKbrdBtn BackButton(string token, string returnUID)
        {
            return new InlineKbrdBtn("\U000021A9 Вернуться назад", token + returnUID);
        }

    }
}