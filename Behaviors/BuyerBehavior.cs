using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramShopAsp.Responses;

namespace TelegramShopAsp
{
        // Поведение ПОКУПАТЕЛЯ
    partial class Behavior       // вторая часть класса с клавиатурами находится в папке Keyboards, файл KeyboardsForBehavior
    {

        // Вызов главного меню
        public static void MainMenuBuyer(string returnUID, Buyer buyer)
        {
            SendKeyboardFor_MainMenuBuyer(returnUID, buyer);
        }        
        
            // перейти к покупкам
        public static void GotoBuy(string parentCategoryUID,Buyer buyer)
        {
            SendKeyboardFor_ChooseProducts("0                               ", buyer);          // при начальном вызове выбора товаров передаётся токен самого родительского элемента
        }

            // Нажатие на товаре или категории товаров - переход к следующей категории или добавление в корзину
        public static void ChooseProducts(string productUID, Buyer buyer)
        {
            if (BOT.IsParent(productUID))            // Выбрали какую-то категорию товаров
                SendKeyboardFor_ChooseProducts(productUID, buyer);
            if (BOT.IsBasicLeave(productUID))             // Выбрали конкретный товар
            {
                DB.Goods good = BOT.GetBasicLeave(productUID);
                if (buyer.cart.Keys.Contains(good))
                {
                    TelegramRequest.AnswerCallbackQuery(BOT.GetBasicLeave(productUID).Name + " убрано из корзины", buyer.currentCallback_query_id);          // делаем всплывающее сообщение о добавлении в корзину товара
                    buyer.cart.Remove(BOT.GetBasicLeave(productUID));
                    SendKeyboardFor_ChooseProducts(BOT.GetBasicLeave(productUID).ParentUID, buyer);
                }
                else
                {
                    TelegramRequest.AnswerCallbackQuery(BOT.GetBasicLeave(productUID).Name + " добавлено в корзину", buyer.currentCallback_query_id);          // делаем всплывающее сообщение о добавлении в корзину товара
                    buyer.cart.Add(BOT.GetBasicLeave(productUID), "");
                    SendKeyboardFor_ChooseProducts(BOT.GetBasicLeave(productUID).ParentUID, buyer);
                }
            }
        }

            // убрать из корзины
        public static void RemooveFromCart(string UIDtoDeleteFromCart,Buyer buyer)
        {
            (buyer as Buyer).cart.Remove(BOT.GetBasicLeave(UIDtoDeleteFromCart));
            SendKeyboardFor_RemooveFromCart("", buyer);
        }

            // оформить заказ
        public static void Checkout(string none,Buyer buyer)
        {
            SendKeyboardFor_MakeCheckout("", buyer);
        }

            // изменение количества товара
        public static void EditQuantityBuyer(string goodUID,Buyer buyer)
        {
            buyer.CurrentState = InputQuantityBuyer;
            buyer.currentItemEdit = goodUID;
            SendKeyboardFor_InputQuantityBuyer(goodUID, buyer);
        }

            // ввод количества товара
        public static void InputQuantityBuyer(string quantity, Buyer buyer)
        {
            DB.Goods good = buyer.cart.Keys.Where(g => g.UID == buyer.currentItemEdit).FirstOrDefault();
            if (good != null)
            {
                buyer.cart[good] = quantity;
            }
            Checkout("", buyer);
        }

            // подтверждение заказа
        public static void MakeCheckout(string returnUID,Buyer buyer)
        {
            buyer.MakeOffer();
            SendKeyboard_MakeOffer(buyer);
        }


        public static void ViewInfoBuyer(string returnUID,Buyer buyer)
        {
            SendKeyboardFor_EditInfoBuyer(returnUID, buyer);
        }

        public static void ViewCart(string none,Buyer buyer)
        {
            SendKeyboardFor_RemooveFromCart("", buyer);
        }        
    }
}