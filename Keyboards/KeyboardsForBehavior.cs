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
        // Набор общих клавиатур
    partial class Behavior                // вторая часть класса с поведениями находится в папке Behaviors
    {
        private static void SendKeyboardFor_ChooseUserType(string none, User user)
        {
            try
            {
                string token = StateToUID_Dic[ChooseUserType];
                string message = "Приветствую! Я - бот, который помогает покупателям совершать покупки не выходя из дома, а продавцам  - находить покупателей";
                InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("Я - покупатель", token + "buyer") });
                ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn("Я - продавец", token + "seller") });

                // отправляем сообщение с клавиатурой и ожидаем ответа
                SendMessageWithInlineKeyBoard(user, message, ikbm);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }

        private static void SendKeyboardFor_ChooseCountry(string returnUID, User user)
        {
            try
            {
                string token = StateToUID_Dic[ChooseCountry];
                string message = "Выберите вашу страну";
                InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
                foreach (string countryUID in BOT.GetAllCountryUIDs())
                {
                    ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn(BOT.GetCountryName(countryUID), token + countryUID) });
                }
                ikbm.Addrow(new List<InlineKbrdBtn>() { BackButton(token, returnUID) });
                SendMessageWithInlineKeyBoard(user, message, ikbm);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }


        private static void SendKeyboardFor_ChooseCity(User user, string returnUID, string countryUID)
        {
            try
            {
                string token = StateToUID_Dic[ChooseCity];
                Dictionary<string, string> cities = new Dictionary<string, string>();
                using ( DB.DB_A566BC_xenichContext _context = new DB.DB_A566BC_xenichContext())
                {
                    cities = _context.Cities
                        .Where(c => c.CountryUID == countryUID)
                        .ToDictionary(c => c.UID, c => c.Name);
                }
                string message = "Выберите ваш город";
                InlineKeyboardMarkup ikbm = new InlineKeyboardMarkup();
                foreach (string cityUID in cities.Keys)
                {
                    ikbm.Addrow(new List<InlineKbrdBtn>() { new InlineKbrdBtn(cities[cityUID], token + cityUID) });
                }
                ikbm.Addrow(new List<InlineKbrdBtn>() { BackButton(token, returnUID) });
                SendMessageWithInlineKeyBoard(user, message, ikbm);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }

// *******************************************************************************************
// ***********************       Общее поведение всех юзеров     *****************************


//**************************************************************************************************************************

//***********************        Служебные методы генерации клавиатуры      ************************************************
//****************************************************************************************************************************************************************************************************************************
//****************************************************************************************************************************************************************************************************************************


        /// <summary>
        /// Посылает простое текстовое сообщение (например для того чтобы предложить юзеру ввести имя)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="user"></param>
        private static void SendSimpleMessage(string message, User user)
        {
            user.ClearMessagesWithKeyboardsList();
            TelegramRequest.SendSimpleMessage(message, user.telegramId);
        }




        /// <summary>
        /// Посылает сообщение с клавиатурой и получает  Id этого сообщения
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <param name="ikbm"></param>
        private static void SendMessageWithInlineKeyBoard(User user, string message, InlineKeyboardMarkup ikbm)
        {
            // если последнее сообщение было отправлено с inline - клавиатуры, то просто делаем замену               
            //if (user.isLastMessageKeyboard && user.message_id > 0)
            if (user.isLastMessageKeyboard && user.message_id > 0)
            //    TelegramRequest.EditMessageText(message, user.chatId, user.messagesWithKeyboardId.Peek(), ikbm);
            {
                TelegramRequest.EditMessageText(message, user.telegramId, user.message_id.ToString(), ikbm);
            }
            else
            {                // Удаляем у юзера клавиатуру
                user.ClearMessagesWithKeyboardsList();
                    // отправляем сообщение с клавиатурой и ожидаем ответа. В ответе - messageId, по которому мы сможем потом удалить клавиатуру
                string response = TelegramRequest.SendMessageWithResponse(message, user.telegramId, ikbm);
                Responsehandler(user, message, response);
            }
        }



        private static void SendMessageWithPhotoAndInlineKeyBoard(User user, string message, string photoId, InlineKeyboardMarkup ikbm)
        {
            user.ClearMessagesWithKeyboardsList();
            string response = TelegramRequest.SendMessageWithPhoto(message, photoId, user.telegramId, ikbm);
            Responsehandler(user, message, response);
        }

        private static void Responsehandler(User user, string message, string response)
        {
            if (response == "BADSTATUS")
            {
                ErrorHandler.Handle(DateTime.Now.ToString() + ": " + "Message: " + message + ". Response: " + response);
            }
            else
            {
                JObject jObject = JObject.Parse(response);
                if ((bool)jObject["ok"])
                    user.message_id = int.Parse(jObject["result"]["message_id"].ToString());       // устанавиваем, что сообщение с этим Id сейчас является текущим (его потом удалить)
                                                                                                   //user.messagesWithKeyboardId.Push(jObject["result"]["message_id"].ToString());      // устанавиваем, что сообщение с этим Id сейчас является текущим (его потом удалить)
                else
                    ErrorHandler.Handle(DateTime.Now.ToString() + ": " + "Message: " + message + ". Response: " + response);
            }
        }
    }
}