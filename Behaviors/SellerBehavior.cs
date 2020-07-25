using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramShopAsp.Responses;

namespace TelegramShopAsp
{
        // Поведение ПРОДАВЦА
    partial class Behavior       // вторая часть класса с клавиатурами находится в папке Keyboards, файл KeyboardsForBehavior
    {
        //public static void ChooseUserTypeSeller(string chooseUserType, Seller seller)
        //{
        //    ChooseUserType(chooseUserType, seller);
        //}

        public static void MainMenuSeller(string returnUID, Seller seller)
        {
            SendKeyboardFor_MainMenuSeller(returnUID, seller);
        }

        //public static void ChooseCountrySeller(string chooseCountry, Seller seller)
        //{
        //    ChooseCountry(chooseCountry, seller);
        //}

        //public static void ChooseCitySeller(string choosedCityUID, Seller seller)
        //{
        //    ChooseCity(choosedCityUID, seller);
        //}

            // Редактировать информацию о компании
        public static void ViewInfoSeller(string returnUID, Seller seller)
        {
            SendKeyboardFor_EditInfoSeller(returnUID, seller);
        }

            // Перейти к наполнению магазина
        public static void GotoStoreFilling(string returnUID, Seller seller)
        {
            SendKeyboardFor_ChooseProductsSeller("0                               ", seller);          // при начальном вызове выбора товаров передаётся токен самого родительского элемента
        }

            // Нажатие на товаре или категории товаров при создании товара- переход к следующей категории или переход к описанию товара
        public static void ChooseProductsSeller(string productUID, Seller seller)
        {
            if (BOT.IsParent(productUID))            // Выбрали какую-то категорию товаров
                SendKeyboardFor_ChooseProductsSeller(productUID, seller);
            if (BOT.IsBasicLeave(productUID))             // Выбрали конкретный товар - создаём товар и переходим к редактированию карточки
            {
                DB.SellerGoods newGood = (seller as Seller).CreateGood(productUID);
                if (newGood != null)
                {
                    TelegramRequest.AnswerCallbackQuery("Товар добавлен", seller.currentCallback_query_id);
                    SendKeyboardFor_EditGoodCard(newGood, seller);
                }
                else
                {
                    TelegramRequest.AnswerCallbackQuery("Что-то пошло не так", seller.currentCallback_query_id);

                }
            }
        }

            // выбор конкретного товара и переход к карточке товара
        public static void ChooseGoodSeller(string goodUID, Seller seller)
        {
            if(seller.sellerGoods.ContainsKey(goodUID))
                SendKeyboardFor_EditGoodCard(seller.sellerGoods[goodUID], seller);
            else
            {
                if (seller.sellerGoods.ContainsKey(seller.currentItemEdit))
                    SendKeyboardFor_EditGoodCard(seller.sellerGoods[seller.currentItemEdit], seller);
                else
                    SendKeyboardFor_ViewGoodsList("0                               ", seller);  // возможно только в случае какой-то ошибки
            }
        }

            



// --------------------------------------------------------------       Карточка товара     --------------------------------------------------------------------------------

           // редактирование наименования товара 
        public static void EditGoodName(string goodUID, Seller seller)
        {
            if (seller.sellerGoods.ContainsKey(goodUID))
            {
                seller.CurrentState = GoodNameEdited;
                seller.currentItemEdit = goodUID;
                SendSimpleMessage("Введите наименование товара: \U0000270F", seller);
            }
            else
            {
                if (seller.sellerGoods.ContainsKey(seller.currentItemEdit))
                {
                    seller.CurrentState = GoodNameEdited;
                    SendSimpleMessage("Введите наименование товара: \U0000270F", seller);
                }
                else
                    SendKeyboardFor_ViewGoodsList("0                               ", seller);  // возможно только в случае какой-то ошибки
            }
        }

            // редактирование наименования товара              // метода нет в словаре !  
        public static void GoodNameEdited(string goodName, Seller seller)
        {
            DB.SellerGoods good = (seller as Seller).SetGoodName(goodName);
            if (good != null)
                SendKeyboardFor_EditGoodCard(good, seller);
            else
                SendKeyboardFor_MainMenuSeller("none", seller);
        }

        public static void AddGoodPhoto(string goodUID, Seller seller)
        {
            if (seller.sellerGoods.ContainsKey(goodUID))
            {
                seller.CurrentState = AddedGoodPhoto;
                seller.currentItemEdit = goodUID;
                SendSimpleMessage("Добавьте фотографию \U0001F4CE ", seller);
            }
            else
            {
                if (seller.sellerGoods.ContainsKey(seller.currentItemEdit))
                {
                    seller.CurrentState = AddedGoodPhoto;
                    SendSimpleMessage("Добавьте фотографию \U0001F4CE ", seller);
                }
                else
                    SendKeyboardFor_ViewGoodsList("0                               ", seller);  // возможно только в случае какой-то ошибки
            }
        }
            // добавление фотографии к товару            // метода нет в словаре !
        public static void AddedGoodPhoto(string photo, Seller seller)
        {
            if (photo == "ERROR")
                SendKeyboardFor_EditGoodCard(seller.sellerGoods[seller.currentItemEdit], seller);
            else
            {
                string[] photos = photo.Split(new char[] { '*' });
                DB.SellerGoods good = (seller as Seller).SetGoodPhoto(photos[0]);
                if (good != null)
                    SendKeyboardFor_EditGoodCard(good, seller);
                else
                    SendKeyboardFor_ViewGoodsList("0                               ", seller);  // возможно только в случае какой-то ошибки
            }
        }

        public static void ViewGoodPhotoSeller(string goodUID, Seller seller)
        {
            if (seller.sellerGoods.ContainsKey(goodUID))
                SendKeyboardFor_ViewPhoto(goodUID, seller);
            else
            {
                if (seller.sellerGoods.ContainsKey(seller.currentItemEdit))
                    SendKeyboardFor_ViewPhoto(seller.currentItemEdit, seller);
                else
                    SendKeyboardFor_ViewGoodsList("0                               ", seller);  // возможно только в случае какой-то ошибки
            }

        }

        public static void CreateGoodDescription(string description, Seller seller)
        {
            DB.SellerGoods good = (seller as Seller).SetGoodDescription(description);
            if (good != null)
                SendKeyboardFor_EditGoodCard(good, seller);
            else
                SendKeyboardFor_MainMenuSeller("none", seller);
        }

            // активизация товара
        public static void SetGoodActivity(string GoodUID, Seller seller)
        {
            DB.SellerGoods good = (seller as Seller).SetGoodActivity(GoodUID);
            if (good != null)
                SendKeyboardFor_EditGoodCard(good, seller);
            else
                SendKeyboardFor_MainMenuSeller("none", seller);
        }

            // удаление товара из магазина
        public static void DeleteGood(string GoodUID, Seller seller)
        {
            if (seller.DeleteGood(GoodUID))
            {
                TelegramRequest.AnswerCallbackQuery("Товар удалён", seller.currentCallback_query_id);          // делаем всплывающее сообщение об удалении товара
                SendSimpleMessage("Товар удалён из магазина", seller);
                SendKeyboardFor_ViewGoodsList("0                               ", seller);                     // переход к просмотру товаров        TODO реализовать вопрос с подтверждением
            }
            else
            {
                TelegramRequest.AnswerCallbackQuery("Ошибка. Что-то пошло не так.", seller.currentCallback_query_id);          // делаем всплывающее сообщение об ошибке
                if (seller.sellerGoods.ContainsKey(GoodUID))
                    SendKeyboardFor_EditGoodCard(seller.sellerGoods[GoodUID], seller);
                else
                    SendKeyboardFor_ViewGoodsList("0                               ", seller);  // возможно только в случае какой-то ошибки
            }
            
        }

        public static void ViewGoodsList(string UID, Seller seller)
        {
            //seller.GenerateDic();
            if (seller.parentProductsDic.ContainsKey(UID))
                SendKeyboardFor_ViewGoodsList(UID, seller);     // если это базовый товар
            else
                SendKeyboardFor_ViewGoodsList("0                               ", seller);  // возможно только в случае какой-то ошибки
        }

// -----------------------------------------------------------------       Методы компании       ------------------------------------------------------------------------

        public static void EditCompanyDescription(string description, Seller seller)
        {
            seller.CurrentState = CompanyDescriptionEdited;
            SendKeyboardFor_CompanyDescriptionEdited("none", seller);
        }

            // метода нет в словаре !
        public static void CompanyDescriptionEdited(string description, Seller seller)
        {
            seller.SetDescription(description);
            seller.CurrentState = ViewInfoSeller;
            SendKeyboardFor_EditInfoSeller("", seller);
        }

            // редактирование - удаление телефонов
        public static void EditCompanyPhones(string phone, Seller seller)
        {
            (seller as Seller).DeletePhone(phone);
            SendKeyboardFor_EditCompanyPhones(StateToUID_Seller_Dic[ViewInfoSeller], seller);
        }

            // просьба ввести телефон
        public static void InputCompanyPhone(string none, Seller seller)
        {
            seller.CurrentState = AddCompanyPhone;
            SendSimpleMessage("Введите контактный телефон: ", seller);
        }

            // телефон введён, добавляем в инфу
        public static void AddCompanyPhone(string phone, Seller seller)
        {
            if (!string.IsNullOrEmpty(phone) && phone.Length < 15)
            {
                (seller as Seller).AddPhone(phone);
                seller.CurrentState = ViewInfoSeller;
                SendKeyboardFor_EditInfoSeller("", seller);
            }
            else
                SendKeyboardFor_EditCompanyPhones(StateToUID_Seller_Dic[ViewInfoSeller], seller);           
        }

            // полное удаление компании
        public static void DeleteCompany(string returnUID, Seller seller)
        {
            SendNotimplementedKeyboardSeller("none", seller);
        }
            
            // приостановить деятельность компании
        public static void StopCompany(string returnUID, Seller seller)
        {
            (seller as Seller).SetActivity();
            SendKeyboardFor_MainMenuSeller("none", seller);
        }

//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    }
}
