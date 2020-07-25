using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramShopAsp.DB;
using TelegramShopAsp.Responses;

namespace TelegramShopAsp
{
    public interface IKeyboard
    {
        string Generate();
    }

    public interface IErrorHandler
    {
        void Handler(string message);
    }

    public interface IBot
    {
        User GetUser(int telegramId);
        bool ContainsSeller(int telegramId);
        bool ContainsBuyer(int telegramId);
        void RemoveBuyer(int telegramId);
        void RemoveSeller(int telegramId);
        void AddBuyer(int telegramId, Buyer newBuyer);
        void AddSeller(int telegramId, Seller newSeller);
        Seller GetSeller(int telegramId);
        Buyer GetBuyer(int telegramId);

        bool ContainsCity(string UID);
        string GetCityName(string UID);

        bool ContainsCountry(string UID);
        string GetCountryName(string UID);
        string[] GetAllCountryUIDs();

        bool IsBasicLeave(string UID);
        Goods GetBasicLeave(string UID);

        bool IsBasicProduct(string UID);
        Goods GetBasicProduct(string UID);

        bool IsParent(string UID);
        List<Goods> GetProductsByParent(string UID);

        string GetUnitShortName(int unitId);
        string GetUnitName5(int unitId);


    }

    public interface IRequests
    {
        void UpdateFromWebHook(Update result);
        void SendSimpleMessage(string message, int chatID);
        void SendMessage(string message, int chatID, IKeyboard keyboard);
        string SendMessageWithResponse(string message, int chatID, IKeyboard keyboard);
        string SendMessageWithPhoto(string message, string photoId, int chatID, IKeyboard keyboard);

    }
}
