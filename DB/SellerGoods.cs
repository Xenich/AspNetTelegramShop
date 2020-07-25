using System;
using System.Collections.Generic;

namespace TelegramShopAsp.DB
{
    public partial class SellerGoods
    {
        public string GoodUID { get; set; }
        public string BasicGoodUID { get; set; }
        public int SellerTelegramId { get; set; }
        public bool isActive { get; set; }
        public string Description { get; set; }
        public string MainPhotoUID { get; set; }
        public string ShopName { get; set; }

        public Goods BasicGoodU { get; set; }
        public Sellers SellerTelegram { get; set; }
    }
}
